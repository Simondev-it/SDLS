using SDLS.Model.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<object> Login(string email, string password);
        Task<object> Refresh(string refreshToken);
        Task<object> Register(UserRegisterDTO dto);

        Task<string> RegisterWithOtpAsync(UserRegisterRequest request);
        Task<bool> ConfirmOtpAsync(ConfirmOtpModel model);
        Task<string> SendOtpAsync(string email);

        // Gửi OTP quên mật khẩu
        Task<string> ForgotPasswordAsync(string email);

        // Xác thực OTP quên mật khẩu
        Task<bool> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request);

        // Reset password
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
