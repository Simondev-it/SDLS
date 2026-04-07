using SDLS.Model.DTOs.DrivingLicense;

namespace SDLS.Model.DTOs.UserLicense
{
    public class UserLicenseDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DrivingLicenseId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public DrivingLicenseDTO? DrivingLicense { get; set; }
    }
}