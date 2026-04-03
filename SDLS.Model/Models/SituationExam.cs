using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class SituationExam
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public double? Duration { get; set; }

    public int? PassScore { get; set; }

    public bool IsRandom { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<SimulationExam> SimulationExams { get; set; } = new List<SimulationExam>();

    public virtual ICollection<SimulationSession> SimulationSessions { get; set; } = new List<SimulationSession>();
}
