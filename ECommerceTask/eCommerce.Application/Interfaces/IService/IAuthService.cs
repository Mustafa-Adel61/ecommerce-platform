using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Application.Interfaces.IService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user, string remoteIp = "");
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string remoteIp = "");
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, string remoteIp = "");
    }
}
