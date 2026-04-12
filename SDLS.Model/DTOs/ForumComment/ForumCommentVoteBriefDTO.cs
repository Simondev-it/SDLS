using System;

using System;

namespace SDLS.Model.DTOs.ForumComment;

public class ForumCommentVoteBriefDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int? Status { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
