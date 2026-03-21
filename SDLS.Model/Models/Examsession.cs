using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("ExamSession")]
public partial class ExamSession
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("examId")]
    public Guid ExamId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("score")]
    public int? Score { get; set; }

    [Column("totalDuration")]
    public int? TotalDuration { get; set; }

    [Column("isPassed")]
    public bool IsPassed { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("ExamSessions")]
    public virtual Exam Exam { get; set; } = null!;

    [InverseProperty("ExamSession")]
    public virtual ICollection<ExamDetail> ExamDetails { get; set; } = new List<ExamDetail>();

    [ForeignKey("UserId")]
    [InverseProperty("ExamSessions")]
    public virtual User User { get; set; } = null!;
}
