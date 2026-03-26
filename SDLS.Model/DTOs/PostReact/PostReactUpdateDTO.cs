using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.PostReact
{
    public class PostReactUpdateDTO
    {
        [NotEmptyGuid]
        public Guid ForumPostId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Vượt quá độ dài tối đa 20 ký tự.")]
        public string ReactType { get; set; } = null!;

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}