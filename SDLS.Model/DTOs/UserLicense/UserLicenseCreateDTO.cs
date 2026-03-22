using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.UserLicense
{
    public class UserLicenseCreateDTO
    {
        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [NotEmptyGuid]
        public Guid DrivingLicenseId { get; set; }
    }
}