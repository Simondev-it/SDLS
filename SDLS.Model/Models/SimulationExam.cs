using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("SimulationExam")]
[Index("SituationExamId", "SimulationId", Name = "SimulationExam_situationExamId_simulationId_key", IsUnique = true)]
public partial class SimulationExam
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("situationExamId")]
    public Guid SituationExamId { get; set; }

    [Column("simulationId")]
    public Guid SimulationId { get; set; }

    [Column("baseScore")]
    public int? BaseScore { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("SimulationId")]
    [InverseProperty("SimulationExams")]
    public virtual SimulationScenario Simulation { get; set; } = null!;

    [InverseProperty("SimulationExam")]
    public virtual ICollection<SimulationSessionDetail> SimulationSessionDetails { get; set; } = new List<SimulationSessionDetail>();

    [ForeignKey("SituationExamId")]
    [InverseProperty("SimulationExams")]
    public virtual SituationExam SituationExam { get; set; } = null!;
}
