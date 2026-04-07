namespace SDLS.Model.DTOs.CommentVote
{
    public class CommentVoteDTO
    {
        public Guid Id { get; set; }
        public Guid ForumCommentId { get; set; }
        public Guid UserId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public ForumCommentBriefDTO? ForumComment { get; set; }
    }
}