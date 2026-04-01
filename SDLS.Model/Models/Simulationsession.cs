using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class SimulationSession
{
    public Guid Id { get; set; }

    public Guid SituationExamId { get; set; }

    public Guid UserId { get; set; }

    public int? TotalScore { get; set; }

    public double? TotalDuration { get; set; }

    public bool IsPassed { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<SimulationSessionDetail> SimulationSessionDetails { get; set; } = new List<SimulationSessionDetail>();

    public virtual SituationExam SituationExam { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
