 using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class Question
{
    public Guid Id { get; set; }

    public Guid QuestionLessonId { get; set; }

    public Guid QuestionTopicId { get; set; }

    public Guid QuestionCategoryId { get; set; }

    public Guid? ParentId { get; set; }

    public int? Index { get; set; }

    public string Content { get; set; } = null!;

    public string? Image { get; set; }

    public string? Explanation { get; set; }

    public string? Type { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual ICollection<Question> InverseParent { get; set; } = new List<Question>();

    public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();

    public virtual Question? Parent { get; set; }

    public virtual QuestionCategory QuestionCategory { get; set; } = null!;

    public virtual QuestionLesson QuestionLesson { get; set; } = null!;

    public virtual ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();

    public virtual QuestionTopic QuestionTopic { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<SavedQuestion> SavedQuestions { get; set; } = new List<SavedQuestion>();
}
