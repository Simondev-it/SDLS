using SDLS.Model.DTOs.ForumPost;

namespace SDLS.Model.DTOs.PostReact
{
    public class PostReactDTO
    {
        public Guid Id { get; set; }
        public Guid ForumPostId { get; set; }
        public Guid UserId { get; set; }
        public string ReactType { get; set; } = null!;
        public int? Status { get; set; }

        public ForumPostDTO? ForumPost { get; set; }
    }
}