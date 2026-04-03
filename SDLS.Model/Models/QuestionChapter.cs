using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class QuestionChapter
{
    public Guid Id { get; set; }

    public Guid DrivingLicenseId { get; set; }

    public int? Index { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual DrivingLicense DrivingLicense { get; set; } = null!;

    public virtual ICollection<QuestionLesson> QuestionLessons { get; set; } = new List<QuestionLesson>();
}
