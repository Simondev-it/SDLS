using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.UserLicense
{
    public class UserLicenseCreateDTO
    {
        [NotEmptyGuid]
        public Guid DrivingLicenseId { get; set; }
    }
}