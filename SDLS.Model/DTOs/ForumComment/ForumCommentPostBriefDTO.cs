using System;

using System;

namespace SDLS.Model.DTOs.ForumComment;

public class ForumCommentPostBriefDTO
{
    public Guid Id { get; set; }
    public Guid ForumTopicId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int? Status { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
