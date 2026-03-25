using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.PostReact
{
    public class PostReactCreateDTO
    {
        [NotEmptyGuid]
        public Guid ForumPostId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Vượt quá độ dài tối đa 20 ký tự.")]
        public string ReactType { get; set; } = null!;
    }
}