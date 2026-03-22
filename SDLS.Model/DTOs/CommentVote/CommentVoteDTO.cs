namespace SDLS.Model.DTOs.CommentVote
{
    public class CommentVoteDTO
    {
        public Guid Id { get; set; }
        public Guid ForumCommentId { get; set; }
        public Guid UserId { get; set; }
        public int? Status { get; set; }
    }
}