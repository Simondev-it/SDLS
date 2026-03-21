using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("Exam")]
public partial class Exam
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("duration")]
    public int? Duration { get; set; }

    [Column("passScore")]
    public int? PassScore { get; set; }

    [Column("isRandom")]
    public bool IsRandom { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    [InverseProperty("Exam")]
    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();

    [ForeignKey("UserId")]
    [InverseProperty("Exams")]
    public virtual User User { get; set; } = null!;
}
