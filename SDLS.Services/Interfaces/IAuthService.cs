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
    }
}
