using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class ExamDetail
{
    public Guid Id { get; set; }

    public Guid AnswerId { get; set; }

    public Guid ExamSessionId { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual Answer Answer { get; set; } = null!;

    public virtual ExamSession ExamSession { get; set; } = null!;
}
