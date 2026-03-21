using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("PostReact")]
[Index("ForumPostId", "UserId", Name = "PostReact_forumPostId_userId_key", IsUnique = true)]
public partial class PostReact
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("forumPostId")]
    public Guid ForumPostId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("reactType")]
    [StringLength(20)]
    public string ReactType { get; set; } = null!;

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("ForumPostId")]
    [InverseProperty("PostReacts")]
    public virtual ForumPost ForumPost { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("PostReacts")]
    public virtual User User { get; set; } = null!;
}
