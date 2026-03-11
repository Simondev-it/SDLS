using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class SimulationScenario
{
    public Guid Id { get; set; }

    public Guid SimulationChapterId { get; set; }

    public Guid SimulationCategoryId { get; set; }

    public Guid SimulationDifficultyLevelId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Video { get; set; }

    public int BaseScore { get; set; }

    public int TotalTime { get; set; }

    public int StartPoint { get; set; }

    public int EndPoint { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual SimulationCategory SimulationCategory { get; set; } = null!;

    public virtual SimulationChapter SimulationChapter { get; set; } = null!;

    public virtual SimulationDifficultyLevel SimulationDifficultyLevel { get; set; } = null!;

    public virtual ICollection<SimulationSession> SimulationSessions { get; set; } = new List<SimulationSession>();
}
