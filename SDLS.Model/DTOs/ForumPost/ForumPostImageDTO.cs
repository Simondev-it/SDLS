namespace SDLS.Model.DTOs.ForumPost
{
    public class ForumPostImageDTO
    {
        public Guid Id { get; set; }
        public Guid ForumPostId { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
