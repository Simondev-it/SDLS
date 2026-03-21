using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("SavedQuestion")]
[Index("QuestionId", "UserId", Name = "SavedQuestion_questionId_userId_key", IsUnique = true)]
public partial class SavedQuestion
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionId")]
    public Guid QuestionId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("SavedQuestions")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SavedQuestions")]
    public virtual User User { get; set; } = null!;
}
