using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("QuestionLesson")]
//[Index("Name", Name = "QuestionLesson_name_key", IsUnique = true)]
public partial class QuestionLesson
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionChapterId")]
    public Guid QuestionChapterId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [InverseProperty("QuestionLesson")]
    public virtual ICollection<LessonImage> LessonImages { get; set; } = new List<LessonImage>();

    [InverseProperty("QuestionLesson")]
    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    [ForeignKey("QuestionChapterId")]
    [InverseProperty("QuestionLessons")]
    public virtual QuestionChapter QuestionChapter { get; set; } = null!;

    [InverseProperty("QuestionLesson")]
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
