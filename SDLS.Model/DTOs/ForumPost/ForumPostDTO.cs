namespace SDLS.Model.DTOs.ForumPost
{
    public class ForumPostDTO
    {
        public Guid Id { get; set; }
        public Guid ForumTopicId { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int? ViewCount { get; set; }
        public int? Status { get; set; }
        public List<ForumPostImageDTO> PostImages { get; set; } = new();
    }
}
