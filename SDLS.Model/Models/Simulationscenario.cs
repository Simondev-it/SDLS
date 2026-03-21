using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("SimulationScenario")]
[Index("Name", Name = "SimulationScenario_name_key", IsUnique = true)]
public partial class SimulationScenario
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("simulationChapterId")]
    public Guid SimulationChapterId { get; set; }

    [Column("simulationCategoryId")]
    public Guid SimulationCategoryId { get; set; }

    [Column("simulationDifficultyLevelId")]
    public Guid SimulationDifficultyLevelId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("video")]
    [StringLength(255)]
    public string? Video { get; set; }

    [Column("totalTime")]
    public int TotalTime { get; set; }

    [Column("startPoint")]
    public int StartPoint { get; set; }

    [Column("endPoint")]
    public int EndPoint { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Simulation")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("SimulationCategoryId")]
    [InverseProperty("SimulationScenarios")]
    public virtual SimulationCategory SimulationCategory { get; set; } = null!;

    [ForeignKey("SimulationChapterId")]
    [InverseProperty("SimulationScenarios")]
    public virtual SimulationChapter SimulationChapter { get; set; } = null!;

    [ForeignKey("SimulationDifficultyLevelId")]
    [InverseProperty("SimulationScenarios")]
    public virtual SimulationDifficultyLevel SimulationDifficultyLevel { get; set; } = null!;

    [InverseProperty("Simulation")]
    public virtual ICollection<SimulationExam> SimulationExams { get; set; } = new List<SimulationExam>();
}
