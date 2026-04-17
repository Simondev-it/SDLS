using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.User
{
    public class UserChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
