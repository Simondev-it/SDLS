using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Notification
{
    public class NotificationDTO
    {
        public Guid Id { get; set; }

        [StringLength(255)]
        public string Title { get; set; } = null!;

        [StringLength(255)]
        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public List<UserNotificationDTO> UserNotifications { get; set; } = new();
    }
}