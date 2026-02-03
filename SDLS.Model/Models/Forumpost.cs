using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("forumpost")]
[Index("Forumtopicid", Name = "idx_forum_post_topic")]
[Index("Userid", Name = "idx_forum_post_user")]
public partial class Forumpost
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("forumtopicid")]
    public Guid? Forumtopicid { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string? Name { get; set; }

    [Column("title")]
    [StringLength(300)]
    public string? Title { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("viewcount")]
    public int? Viewcount { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Forumpost")]
    public virtual ICollection<Forumcomment> Forumcomments { get; set; } = new List<Forumcomment>();

    [ForeignKey("Forumtopicid")]
    [InverseProperty("Forumposts")]
    public virtual Forumtopic? Forumtopic { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Forumposts")]
    public virtual User? User { get; set; }
}
