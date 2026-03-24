using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SDLS.Model.Constants;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs.User;

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
        public async Task<object> Register(UserRegisterDTO dto)
        {
            var exist = await _userRepo.GetByEmailAsync(dto.Email);

            if (exist != null)
                throw new Exception("Email đã tồn tại");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Name = dto.Name,
                Avatar = dto.Avatar,
                Phone = dto.Phone,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,

                RoleId = RoleConst.USER_ROLE_ID, // mặc định USER
                CreateAt = DateTime.Now,
                Status = 1
            };

            await _userRepo.CreateAsync(user);
            await _context.SaveChangesAsync();

            return new
            {
                message = "Đăng ký thành công",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name
                }
            };
        }
    }
}
