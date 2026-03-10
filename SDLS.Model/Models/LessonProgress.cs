using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class LessonProgress
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid QuestionLessonId { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual QuestionLesson QuestionLesson { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
