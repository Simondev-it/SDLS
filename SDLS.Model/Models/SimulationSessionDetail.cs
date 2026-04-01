using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class SimulationSessionDetail
{
    public Guid Id { get; set; }

    public Guid SimulationExamId { get; set; }

    public Guid SimulationSessionId { get; set; }

    public double? DurationSecond { get; set; }

    public int? Score { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual SimulationExam SimulationExam { get; set; } = null!;

    public virtual SimulationSession SimulationSession { get; set; } = null!;
}
