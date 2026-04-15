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
using SDLS.Model.Models;
using MailKit.Security;
using MimeKit;
using SDLS.Repositories.Repositories;
using Microsoft.Extensions.Caching.Memory;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.InkML;

namespace SDLS.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwt;
        private readonly EmailSettings _emailSettings;
        private readonly IMemoryCache _cache;

        public AuthService(IUserRepository userRepo, IJwtService jwt, IOptions<EmailSettings> emailSettings, IMemoryCache cache)
        {
            _userRepo = userRepo;
            _jwt = jwt;
            _emailSettings = emailSettings.Value;
            _cache = cache;
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
                //Avatar = dto.Avatar,
                Phone = dto.Phone,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,

                RoleId = RoleConst.GUEST_ROLE_ID, // mặc định GUEST (đã sửa)
                CreateAt = DateTime.Now,
                Status = 1
            };

            await _userRepo.CreateAsync(user);
            await _userRepo.SaveAsync();

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
        //////////


        // ✅ STEP 1: Gửi OTP
        public async Task<string> RegisterWithOtpAsync(UserRegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email không hợp lệ");

            var existing = await _userRepo.GetByEmailAsync(request.Email);
            if (existing != null)
                throw new Exception("Email đã tồn tại");

            var otp = new Random().Next(100000, 999999).ToString();

            var tempUser = new TempUserRegisterModel
            {
                Email = request.Email,
                Password = request.Password,
                Name = request.Name,
                Phone = request.Phone,
                Gender = request.Gender,
                Avatar = request.Avatar,
                RoleId = request.RoleId,
                Otp = otp
            };

            _cache.Set(request.Email, tempUser, TimeSpan.FromMinutes(5));

            await SendEmail(request.Email, request.Name, otp);

            return "Đã gửi OTP đến email";
        }

        // ✅ STEP 2: Confirm OTP
        public async Task<bool> ConfirmOtpAsync(ConfirmOtpModel model)
        {
            // 🔒 1. Check cache OTP
            if (!_cache.TryGetValue(model.Email, out TempUserRegisterModel temp))
                throw new Exception("OTP đã hết hạn hoặc không tồn tại");

            // 🔒 2. Check OTP đúng
            if (temp.Otp != model.Otp)
                throw new Exception("OTP không đúng");

            // 🔒 3. Check email đã tồn tại chưa (tránh spam)
            var existingUser = await _userRepo.GetByEmailAsync(temp.Email);
            if (existingUser != null)
                throw new Exception("Email đã được sử dụng");

            //// 🔥 4. Check Role tồn tại (FIX LỖI FK)
            //var roleExists = await _context.Roles.AnyAsync(r => r.Id == temp.RoleId);
            //if (!roleExists)
            //    throw new Exception("Role không tồn tại");

            // 🚀 5. Tạo user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = temp.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(temp.Password),
                Name = temp.Name,
                Phone = temp.Phone,
                Avatar = temp.Avatar,
                RoleId = temp.RoleId,
                Status = 1,
                CreateAt = DateTime.UtcNow
            };

            // 💾 6. Save DB
            await _userRepo.AddAsync(user);

            // 🧹 7. Xóa OTP
            _cache.Remove(model.Email);

            // 📧 8. Gửi mail thành công (KHÔNG làm fail flow)
            try
            {
                await SendRegisterSuccessEmail(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                // 👉 chỉ log, không throw
                Console.WriteLine("Send mail failed: " + ex.Message);
            }

            return true;
        }

        // ✅ STEP 3: gửi OTP riêng
        public async Task<string> SendOtpAsync(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            await SendEmail(email, "User", otp);
            return otp;
        }

        // 📧 SEND MAIL
        private async Task SendEmail(string to, string name, string otp)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "Mã OTP xác thực";

            email.Body = new TextPart("html")
            {
                Text = $@"
                    <!DOCTYPE html>
                    <html>
                    <body style='margin:0; background:#f4f6f8; font-family:Arial;'>

                    <div style='max-width:500px; margin:40px auto; background:white; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.05);'>

                        <div style='background:#1976d2; color:white; padding:20px; text-align:center;'>
                            <h2>🔐 OTP Verification</h2>
                        </div>

                        <div style='padding:30px; text-align:center;'>
                            <p>Hello <b>{name}</b>,</p>

                            <p>Your verification code is:</p>

                            <h1 style='letter-spacing:5px; color:#d32f2f; font-size:32px;'>
                                {otp}
                            </h1>

                            <p style='color:#777;'>Valid for 5 minutes</p>
                        </div>

                    </div>

                    </body>
                    </html>
                    "
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        private async Task SendRegisterSuccessEmail(string to, string name)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "🎉 Đăng ký thành công - SDLS";

            email.Body = new TextPart("html")
            {
                                Text = $@"
                <!DOCTYPE html>
                <html>
                <body style='margin:0; background:#f4f6f8; font-family:Arial;'>

                <table width='100%' cellpadding='0' cellspacing='0' style='padding:20px;'>
                <tr>
                <td align='center'>

                <table width='600' style='background:white; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.05);'>

                <tr>
                <td style='background:linear-gradient(90deg,#1976d2,#42a5f5); padding:20px; text-align:center; color:white;'>
                    <h2>🚗 SDLS System</h2>
                    <p>Smart Driving Learning</p>
                </td>
                </tr>

                <tr>
                <td style='padding:30px;'>

                <h2 style='color:#333;'>🎉 Welcome, {name}!</h2>

                <p style='color:#555;'>
                Bạn đã đăng ký tài khoản thành công.
                </p>

                <table width='100%' style='margin:20px 0; background:#f9fafc; border-radius:8px;'>
                <tr>
                <td style='padding:15px;'>
                <b>Email:</b> {to}<br/>
                <b>Status:</b> Active
                </td>
                </tr>
                </table>

                <div style='text-align:center; margin:30px 0;'>
                <a href='http://localhost:5173/login'
                   style='padding:12px 25px; background:#1976d2; color:white; text-decoration:none; border-radius:6px;'>
                   Đăng nhập ngay
                </a>
                </div>

                <p style='font-size:13px; color:#888;'>
                Nếu bạn không thực hiện đăng ký, hãy bỏ qua email này.
                </p>

                </td>
                </tr>

                <tr>
                <td style='background:#f4f6f8; padding:20px; text-align:center; font-size:12px; color:#999;'>
                © 2026 SDLS System
                </td>
                </tr>

                </table>

                </td>
                </tr>
                </table>

                </body>
                </html>"
             };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _emailSettings.SenderEmail,
                _emailSettings.SenderPassword);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }


        ///forget pass ///
        ///

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Email không tồn tại");

            var otp = new Random().Next(100000, 999999).ToString();

            var temp = new TempForgotPasswordModel
            {
                Email = email,
                Otp = otp
            };

            _cache.Set(email + "_fp", temp, TimeSpan.FromMinutes(5));

            await SendOtpEmail(email, user.Name, otp);

            return "Đã gửi OTP";
        }
        public async Task<bool> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request)
        {
            if (!_cache.TryGetValue(request.Email + "_fp", out TempForgotPasswordModel temp))
                throw new Exception("OTP đã hết hạn");

            if (temp.Otp != request.Otp)
                throw new Exception("OTP không đúng");

            temp.IsVerified = true;

            _cache.Set(request.Email + "_fp", temp, TimeSpan.FromMinutes(5));

            return true;
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (!_cache.TryGetValue(request.Email + "_fp", out TempForgotPasswordModel temp))
                throw new Exception("Chưa xác thực OTP");

            if (!temp.IsVerified)
                throw new Exception("OTP chưa được xác thực");

            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("User không tồn tại");

            // 🔐 Hash password
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdateAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            // 🧹 Xóa cache
            _cache.Remove(request.Email + "_fp");

            // 📧 gửi mail thông báo đổi pass
            try
            {
                await SendResetPasswordSuccessEmail(user.Email, user.Name);
            }
            catch { }

            return true;
        }


        private async Task SendOtpEmail(string to, string name, string otp)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "🔐 OTP Reset Password";

            email.Body = new TextPart("html")
            {
                Text = $@"
            <div style='text-align:center'>
                <h2>Xin chào {name}</h2>
                <h1 style='color:red'>{otp}</h1>
                <p>OTP có hiệu lực 5 phút</p>
            </div>"
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private async Task SendResetPasswordSuccessEmail(string to, string name)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "🔐 Password Changed Successfully";

            email.Body = new TextPart("html")
            {
                Text = $@"
            <div style='font-family:Arial'>
                <h2>Hi {name}</h2>
                <p>Your password has been changed successfully.</p>
            </div>"
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
