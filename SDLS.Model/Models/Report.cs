using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("Report")]
public partial class Report
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("simulationId")]
    public Guid? SimulationId { get; set; }

    [Column("forumPostId")]
    public Guid? ForumPostId { get; set; }

    [Column("forumCommentId")]
    public Guid? ForumCommentId { get; set; }

    [Column("questionId")]
    public Guid? QuestionId { get; set; }

    [Column("reportCategoryId")]
    public Guid ReportCategoryId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("content")]
    [StringLength(1000)]
    public string Content { get; set; } = null!;

    [Column("image")]
    [StringLength(255)]
    public string? Image { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("ForumCommentId")]
    [InverseProperty("Reports")]
    public virtual ForumComment? ForumComment { get; set; }

    [ForeignKey("ForumPostId")]
    [InverseProperty("Reports")]
    public virtual ForumPost? ForumPost { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("Reports")]
    public virtual Question? Question { get; set; }

    [ForeignKey("ReportCategoryId")]
    [InverseProperty("Reports")]
    public virtual ReportCategory ReportCategory { get; set; } = null!;

    [InverseProperty("Report")]
    public virtual ICollection<Resolve> Resolves { get; set; } = new List<Resolve>();

    [ForeignKey("SimulationId")]
    [InverseProperty("Reports")]
    public virtual SimulationScenario? Simulation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Reports")]
    public virtual User User { get; set; } = null!;
}
