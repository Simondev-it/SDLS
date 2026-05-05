using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Notification
{
    public class NotificationCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        public int Status { get; set; } = 1;

        public List<UserNotificationCreateDTO> UserNotifications { get; set; } = new();
    }
}