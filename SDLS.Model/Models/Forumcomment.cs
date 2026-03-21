using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("ForumComment")]
public partial class ForumComment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("replyId")]
    public Guid? ReplyId { get; set; }

    [Column("forumPostId")]
    public Guid ForumPostId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("content")]
    [StringLength(1000)]
    public string Content { get; set; } = null!;

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("ForumComment")]
    public virtual ICollection<CommentVote> CommentVotes { get; set; } = new List<CommentVote>();

    [ForeignKey("ForumPostId")]
    [InverseProperty("ForumComments")]
    public virtual ForumPost ForumPost { get; set; } = null!;

    [InverseProperty("Reply")]
    public virtual ICollection<ForumComment> InverseReply { get; set; } = new List<ForumComment>();

    [ForeignKey("ReplyId")]
    [InverseProperty("InverseReply")]
    public virtual ForumComment? Reply { get; set; }

    [InverseProperty("ForumComment")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("UserId")]
    [InverseProperty("ForumComments")]
    public virtual User User { get; set; } = null!;
}
