namespace eCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingByName = await _userManager.FindByNameAsync(dto.UserName);
            if (existingByName != null) return BadRequest("User name already taken.");

            var existingByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingByEmail != null) return BadRequest("Email already taken.");

            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null) return Unauthorized("Invalid credentials");

            var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!check.Succeeded) return Unauthorized("Invalid credentials");

            var tokens = await _authService.GenerateTokensAsync(user, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto req)
        {
            var newTokens = await _authService.RefreshTokenAsync(req.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            if (newTokens == null) return Unauthorized("Invalid or expired refresh token");

            return Ok(newTokens);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequestDto req)
        {
            var revoked = await _authService.RevokeRefreshTokenAsync(req.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            if (!revoked) return NotFound("Token not found or already revoked");
            return Ok("Revoked");
        }
    }

}
