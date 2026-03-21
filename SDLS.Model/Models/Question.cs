using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("Question")]
public partial class Question
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionLessonId")]
    public Guid QuestionLessonId { get; set; }

    [Column("questionTopicId")]
    public Guid QuestionTopicId { get; set; }

    [Column("questionCategoryId")]
    public Guid QuestionCategoryId { get; set; }

    [Column("parentId")]
    public Guid? ParentId { get; set; }

    [Column("content")]
    [StringLength(255)]
    public string Content { get; set; } = null!;

    [Column("image")]
    [StringLength(255)]
    public string? Image { get; set; }

    [Column("explanation")]
    [StringLength(255)]
    public string? Explanation { get; set; }

    [Column("type")]
    [StringLength(20)]
    public string? Type { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    [InverseProperty("Question")]
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    [InverseProperty("Parent")]
    public virtual ICollection<Question> InverseParent { get; set; } = new List<Question>();

    [InverseProperty("Question")]
    public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Question? Parent { get; set; }

    [ForeignKey("QuestionCategoryId")]
    [InverseProperty("Questions")]
    public virtual QuestionCategory QuestionCategory { get; set; } = null!;

    [ForeignKey("QuestionLessonId")]
    [InverseProperty("Questions")]
    public virtual QuestionLesson QuestionLesson { get; set; } = null!;

    [InverseProperty("Question")]
    public virtual ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();

    [ForeignKey("QuestionTopicId")]
    [InverseProperty("Questions")]
    public virtual QuestionTopic QuestionTopic { get; set; } = null!;

    [InverseProperty("Question")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [InverseProperty("Question")]
    public virtual ICollection<SavedQuestion> SavedQuestions { get; set; } = new List<SavedQuestion>();
}
