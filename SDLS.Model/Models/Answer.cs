using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class Answer
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string Content { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<ExamDetail> ExamDetails { get; set; } = new List<ExamDetail>();

    public virtual Question Question { get; set; } = null!;
}
