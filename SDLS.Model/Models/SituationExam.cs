using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("SituationExam")]
public partial class SituationExam
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("duration")]
    public int? Duration { get; set; }

    [Column("passScore")]
    public int? PassScore { get; set; }

    [Column("isRandom")]
    public bool IsRandom { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("SituationExam")]
    public virtual ICollection<SimulationExam> SimulationExams { get; set; } = new List<SimulationExam>();

    [InverseProperty("SituationExam")]
    public virtual ICollection<SimulationSession> SimulationSessions { get; set; } = new List<SimulationSession>();
}
