using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.User
{
    public class UserCreateDTO
    {
        [NotEmptyGuid]
        public Guid RoleId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Description { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? LicenseType { get; set; }
        public int? Status { get; set; }
    }
}
