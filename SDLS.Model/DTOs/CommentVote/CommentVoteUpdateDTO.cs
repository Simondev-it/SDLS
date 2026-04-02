using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.CommentVote
{
    public class CommentVoteUpdateDTO
    {
        [NotEmptyGuid]
        public Guid ForumCommentId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}