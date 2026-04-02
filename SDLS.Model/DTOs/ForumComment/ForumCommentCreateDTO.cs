using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ForumComment
{
    public class ForumCommentCreateDTO
    {
        public Guid? ReplyId { get; set; }

        [NotEmptyGuid]
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public Guid ForumPostId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(1000, ErrorMessage = "Vượt quá độ dài tối đa 1000 ký tự.")]
        public string Content { get; set; } = null!;
    }
}