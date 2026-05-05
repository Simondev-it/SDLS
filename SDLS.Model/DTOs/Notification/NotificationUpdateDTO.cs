using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Notification
{
    public class NotificationUpdateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }

        public List<UserNotificationUpdateDTO> UserNotifications { get; set; } = new();
    }
}