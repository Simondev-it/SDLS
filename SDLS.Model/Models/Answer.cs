using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("Answer")]
public partial class Answer
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionId")]
    public Guid QuestionId { get; set; }

    [Column("content")]
    [StringLength(255)]
    public string Content { get; set; } = null!;

    [Column("isCorrect")]
    public bool IsCorrect { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Answer")]
    public virtual ICollection<ExamDetail> ExamDetails { get; set; } = new List<ExamDetail>();

    [ForeignKey("QuestionId")]
    [InverseProperty("Answers")]
    public virtual Question Question { get; set; } = null!;
}
