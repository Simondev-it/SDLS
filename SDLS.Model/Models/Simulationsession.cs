using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class SimulationSession
{
    public Guid Id { get; set; }

    public Guid SimulationId { get; set; }

    public Guid UserId { get; set; }

    public int? DurationSecond { get; set; }

    public int? Score { get; set; }

    public bool IsPassed { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual SimulationScenario Simulation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
