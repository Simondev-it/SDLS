using System;

using System;

namespace SDLS.Model.DTOs.ForumComment;

public class ForumCommentUserBriefDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public int? Status { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
