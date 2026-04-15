using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.User
{
    public class TempUserRegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public string? Gender { get; set; }
        public string Avatar { get; set; }
        public Guid RoleId { get; set; }

        public string Otp { get; set; }
    }

    public class TempForgotPasswordModel
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public bool IsVerified { get; set; } = false;
    }

}
