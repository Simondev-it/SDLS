using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("CommentVote")]
[Index("ForumCommentId", "UserId", Name = "CommentVote_forumCommentId_userId_key", IsUnique = true)]
public partial class CommentVote
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("forumCommentId")]
    public Guid ForumCommentId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("ForumCommentId")]
    [InverseProperty("CommentVotes")]
    public virtual ForumComment ForumComment { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CommentVotes")]
    public virtual User User { get; set; } = null!;
}
