using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ForumComment
{
    public class ForumCommentUpdateDTO
    {
        public Guid? ReplyId { get; set; }

        [NotEmptyGuid]
        public Guid ForumPostId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(1000, ErrorMessage = "Vượt quá độ dài tối đa 1000 ký tự.")]
        public string Content { get; set; } = null!;

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}