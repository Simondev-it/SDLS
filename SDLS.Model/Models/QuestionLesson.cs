using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class QuestionLesson
{
    public Guid Id { get; set; }

    public Guid QuestionChapterId { get; set; }

    public int? Index { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public string? Content { get; set; }

    public virtual ICollection<LessonImage> LessonImages { get; set; } = new List<LessonImage>();

    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public virtual QuestionChapter QuestionChapter { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
