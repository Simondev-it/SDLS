using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class ExamQuestion
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public Guid ExamId { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
