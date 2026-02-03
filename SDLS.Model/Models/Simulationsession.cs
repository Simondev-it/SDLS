using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("simulationsession")]
[Index("Userid", Name = "idx_simulation_session_user")]
public partial class Simulationsession
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("simulationid")]
    public Guid? Simulationid { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("durationsecond")]
    public int? Durationsecond { get; set; }

    [Column("score")]
    public int? Score { get; set; }

    [Column("ispassed")]
    public bool? Ispassed { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Simulationid")]
    [InverseProperty("Simulationsessions")]
    public virtual Simulationscenario? Simulation { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Simulationsessions")]
    public virtual User? User { get; set; }
}
