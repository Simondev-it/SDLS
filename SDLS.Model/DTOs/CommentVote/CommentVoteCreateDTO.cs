using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.CommentVote
{
    public class CommentVoteCreateDTO
    {
        [NotEmptyGuid]
        public Guid ForumCommentId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }
    }
}