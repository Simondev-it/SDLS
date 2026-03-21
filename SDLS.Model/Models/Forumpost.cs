using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("ForumPost")]
public partial class ForumPost
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("forumTopicId")]
    public Guid ForumTopicId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("content")]
    [StringLength(255)]
    public string Content { get; set; } = null!;

    [Column("viewCount")]
    public int? ViewCount { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("ForumPost")]
    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    [ForeignKey("ForumTopicId")]
    [InverseProperty("ForumPosts")]
    public virtual ForumTopic ForumTopic { get; set; } = null!;

    [InverseProperty("ForumPost")]
    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();

    [InverseProperty("ForumPost")]
    public virtual ICollection<PostReact> PostReacts { get; set; } = new List<PostReact>();

    [InverseProperty("ForumPost")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("UserId")]
    [InverseProperty("ForumPosts")]
    public virtual User User { get; set; } = null!;
}
