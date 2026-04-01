using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class SimulationExam
{
    public Guid Id { get; set; }

    public Guid SituationExamId { get; set; }

    public Guid SimulationId { get; set; }

    public int? BaseScore { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual SimulationScenario Simulation { get; set; } = null!;

    public virtual ICollection<SimulationSessionDetail> SimulationSessionDetails { get; set; } = new List<SimulationSessionDetail>();

    public virtual SituationExam SituationExam { get; set; } = null!;
}
