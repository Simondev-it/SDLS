using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("PostImage")]
public partial class PostImage
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("forumPostId")]
    public Guid ForumPostId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("url")]
    [StringLength(255)]
    public string? Url { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("ForumPostId")]
    [InverseProperty("PostImages")]
    public virtual ForumPost ForumPost { get; set; } = null!;
}
