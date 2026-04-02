using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class LessonImage
{
    public Guid Id { get; set; }

    public Guid QuestionLessonId { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual QuestionLesson QuestionLesson { get; set; } = null!;
}
