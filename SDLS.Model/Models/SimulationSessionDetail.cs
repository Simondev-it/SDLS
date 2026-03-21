using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("SimulationSessionDetail")]
[Index("SimulationExamId", "SimulationSessionId", Name = "SimulationSessionDetail_simulationExamId_simulationSessionI_key", IsUnique = true)]
public partial class SimulationSessionDetail
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("simulationExamId")]
    public Guid SimulationExamId { get; set; }

    [Column("simulationSessionId")]
    public Guid SimulationSessionId { get; set; }

    [Column("durationSecond")]
    public int? DurationSecond { get; set; }

    [Column("score")]
    public int? Score { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("SimulationExamId")]
    [InverseProperty("SimulationSessionDetails")]
    public virtual SimulationExam SimulationExam { get; set; } = null!;

    [ForeignKey("SimulationSessionId")]
    [InverseProperty("SimulationSessionDetails")]
    public virtual SimulationSession SimulationSession { get; set; } = null!;
}
