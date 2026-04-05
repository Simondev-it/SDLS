using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;

namespace SDLS.Model.DTOs.ForumComment;

public class ForumCommentDTO
{
    public Guid Id { get; set; }
    public Guid? ReplyId { get; set; }
    public Guid ForumPostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public int? Status { get; set; }

    public ForumCommentUserBriefDTO? User { get; set; }
    public ForumCommentPostBriefDTO? ForumPost { get; set; }
    public List<ForumCommentVoteBriefDTO> CommentVotes { get; set; } = new();
}
