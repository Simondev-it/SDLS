using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("LessonProgress")]
[Index("UserId", "QuestionLessonId", Name = "LessonProgress_userId_questionLessonId_key", IsUnique = true)]
public partial class LessonProgress
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("questionLessonId")]
    public Guid QuestionLessonId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("QuestionLessonId")]
    [InverseProperty("LessonProgresses")]
    public virtual QuestionLesson QuestionLesson { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("LessonProgresses")]
    public virtual User User { get; set; } = null!;
}
