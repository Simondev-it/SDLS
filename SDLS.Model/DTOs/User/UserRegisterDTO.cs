using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.User
{
    public class UserRegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }
}
