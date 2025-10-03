using System.Security.Cryptography;

namespace eCommerce.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork; // optional if you want to SaveChanges via unit of work

        public AuthService(IConfiguration config,
                           IRefreshTokenRepository refreshRepo,
                           UserManager<ApplicationUser> userManager,
                           IUnitOfWork unitOfWork)
        {
            _config = config;
            _refreshRepo = refreshRepo;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user, string remoteIp = "")
        {
            var jwtSection = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"]!);
            var accessTokenExpires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // add roles if required:
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: accessTokenExpires,
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Create refresh token
            var refreshTokenString = GenerateSecureToken();
            var refreshExpiryDays = int.Parse(jwtSection["RefreshTokenExpiryDays"]!);
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                Expires = DateTime.UtcNow.AddDays(refreshExpiryDays),
                Created = DateTime.UtcNow,
                RemoteIpAddress = remoteIp,
                UserId = user.Id
            };

            await _refreshRepo.AddAsync(refreshToken);
            await _refreshRepo.SaveChangesAsync();

            // mark last login
            user.LastLoginTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpires,
                RefreshToken = refreshTokenString,
                RefreshTokenExpiration = refreshToken.Expires
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string token, string remoteIp = "")
        {
            var existing = await _refreshRepo.GetByTokenAsync(token);
            if (existing == null || !existing.IsActive) return null;

            var user = existing.User!;
            // Rotate: revoke current and create a new refresh token
            existing.Revoked = DateTime.UtcNow;
            existing.ReplacedByToken = GenerateSecureToken();

            _refreshRepo.Update(existing);

            var newRefresh = new RefreshToken
            {
                Token = existing.ReplacedByToken!,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!)),
                Created = DateTime.UtcNow,
                RemoteIpAddress = remoteIp,
                UserId = existing.UserId
            };

            await _refreshRepo.AddAsync(newRefresh);
            await _refreshRepo.SaveChangesAsync();

            // generate new access token
            var authResponse = await GenerateTokensForUserWithoutCreatingRefresh(user, newRefresh.Token);

            return authResponse;
        }

        // helper used by refresh flow to avoid creating another refresh token - we already created above
        private async Task<AuthResponseDto> GenerateTokensForUserWithoutCreatingRefresh(ApplicationUser user, string refreshTokenValue)
        {
            var jwtSection = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"]!);
            var accessTokenExpires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: accessTokenExpires,
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpires,
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(int.Parse(jwtSection["RefreshTokenExpiryDays"]!))
            };
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string remoteIp = "")
        {
            var existing = await _refreshRepo.GetByTokenAsync(token);
            if (existing == null || !existing.IsActive) return false;

            existing.Revoked = DateTime.UtcNow;
            _refreshRepo.Update(existing);
            await _refreshRepo.SaveChangesAsync();
            return true;
        }

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
