using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwt;

        public AuthService(IUserRepository userRepo, IJwtService jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt;
        }

        public async Task<object> Login(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Sai tài khoản hoặc mật khẩu");

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken(user);

            return new
            {
                accessToken,
                refreshToken,

                // ✅ thêm info user
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    roleId = user.RoleId,
                    roleName = user.Role?.Name
                }
            };
        }

        public async Task<object> Refresh(string refreshToken)
        {
            var principal = _jwt.GetPrincipalFromExpiredToken(refreshToken);

            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr))
                throw new Exception("Token không hợp lệ");

            // ✅ Convert đúng kiểu Guid
            var userId = Guid.Parse(userIdStr);

            // ✅ Dùng repo mới (có include Role)
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User không tồn tại");

            var newAccessToken = _jwt.GenerateAccessToken(user);

            return new
            {
                accessToken = newAccessToken
            };
        }
    }
}
