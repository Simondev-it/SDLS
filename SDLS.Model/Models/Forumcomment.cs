using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class ForumComment
{
    public Guid Id { get; set; }

    public Guid? ReplyId { get; set; }

    public Guid ForumPostId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<CommentVote> CommentVotes { get; set; } = new List<CommentVote>();

    public virtual ForumPost ForumPost { get; set; } = null!;

    public virtual ICollection<ForumComment> InverseReply { get; set; } = new List<ForumComment>();

    public virtual ForumComment? Reply { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User User { get; set; } = null!;
}
