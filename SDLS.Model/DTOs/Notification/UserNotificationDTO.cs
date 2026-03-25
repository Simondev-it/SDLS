namespace SDLS.Model.DTOs.Notification
{
    public class UserNotificationDTO
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public int? Status { get; set; }
        public UserNotificationUserDTO? User { get; set; }
    }
}