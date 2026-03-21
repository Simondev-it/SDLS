using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("LearningProgress")]
[Index("QuestionId", "UserId", Name = "LearningProgress_questionId_userId_key", IsUnique = true)]
[Index("UserId", Name = "LearningProgress_userId_key", IsUnique = true)]
public partial class LearningProgress
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
    [InverseProperty("LearningProgresses")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("LearningProgress")]
    public virtual User User { get; set; } = null!;
}
