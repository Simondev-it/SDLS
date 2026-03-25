using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.UserLicense
{
    public class UserLicenseUpdateDTO
    {
        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [NotEmptyGuid]
        public Guid DrivingLicenseId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}