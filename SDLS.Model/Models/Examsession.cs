using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class ExamSession
{
    public Guid Id { get; set; }

    public Guid ExamId { get; set; }

    public Guid UserId { get; set; }

    public int? Score { get; set; }

    public bool IsPassed { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual ICollection<ExamDetail> ExamDetails { get; set; } = new List<ExamDetail>();

    public virtual User User { get; set; } = null!;
}
