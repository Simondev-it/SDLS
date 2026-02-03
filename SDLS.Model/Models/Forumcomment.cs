using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("forumcomment")]
[Index("Forumpostid", Name = "idx_forum_comment_post")]
public partial class Forumcomment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("replyid")]
    public Guid? Replyid { get; set; }

    [Column("forumpostid")]
    public Guid? Forumpostid { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Forumpostid")]
    [InverseProperty("Forumcomments")]
    public virtual Forumpost? Forumpost { get; set; }

    [InverseProperty("Reply")]
    public virtual ICollection<Forumcomment> InverseReply { get; set; } = new List<Forumcomment>();

    [ForeignKey("Replyid")]
    [InverseProperty("InverseReply")]
    public virtual Forumcomment? Reply { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Forumcomments")]
    public virtual User? User { get; set; }
}
