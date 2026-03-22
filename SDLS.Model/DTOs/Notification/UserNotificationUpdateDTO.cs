using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Notification
{
    public class UserNotificationUpdateDTO
    {
        public Guid? Id { get; set; }

        [NotEmptyGuid]
        public Guid NotificationId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}