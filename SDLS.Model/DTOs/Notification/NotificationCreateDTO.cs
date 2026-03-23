using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Notification
{
    public class NotificationCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt qua độ dài tối đa 255 ký tự.")]
        public string Content { get; set; } = null!;

        public IFormFile? ImageFile { get; set; }

        public int Status { get; set; } = 1;

        public List<UserNotificationCreateDTO> UserNotifications { get; set; } = new();
    }
}