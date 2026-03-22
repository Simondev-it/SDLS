using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.Notification
{
    public class UserNotificationCreateDTO
    {
        [NotEmptyGuid]
        public Guid UserId { get; set; }
    }
}