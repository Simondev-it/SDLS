namespace SDLS.Model.DTOs.CommentVote
{
    public class ForumCommentBriefDTO
    {
        public Guid Id { get; set; }
        public Guid ForumPostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = null!;
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}