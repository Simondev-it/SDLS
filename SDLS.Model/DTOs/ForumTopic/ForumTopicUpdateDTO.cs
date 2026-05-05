using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ForumTopic
{
    public class ForumTopicUpdateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}