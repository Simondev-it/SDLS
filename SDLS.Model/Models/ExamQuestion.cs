using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("ExamQuestion")]
[Index("QuestionId", "ExamId", Name = "ExamQuestion_questionId_examId_key", IsUnique = true)]
public partial class ExamQuestion
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionId")]
    public Guid QuestionId { get; set; }

    [Column("examId")]
    public Guid ExamId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("ExamQuestions")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("QuestionId")]
    [InverseProperty("ExamQuestions")]
    public virtual Question Question { get; set; } = null!;
}
