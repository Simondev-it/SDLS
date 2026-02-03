using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("simulationscenario")]
[Index("Simulationcategoryid", Name = "idx_simulation_scenario_category")]
[Index("Simulationchapterid", Name = "idx_simulation_scenario_chapter")]
public partial class Simulationscenario
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("simulationchapterid")]
    public Guid? Simulationchapterid { get; set; }

    [Column("simulationcategoryid")]
    public Guid? Simulationcategoryid { get; set; }

    [Column("simulationdifficultylevelid")]
    public Guid? Simulationdifficultylevelid { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("video")]
    public string? Video { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Simulation")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("Simulationcategoryid")]
    [InverseProperty("Simulationscenarios")]
    public virtual Simulationcategory? Simulationcategory { get; set; }

    [ForeignKey("Simulationchapterid")]
    [InverseProperty("Simulationscenarios")]
    public virtual Simulationchapter? Simulationchapter { get; set; }

    [ForeignKey("Simulationdifficultylevelid")]
    [InverseProperty("Simulationscenarios")]
    public virtual Simulationdifficultylevel? Simulationdifficultylevel { get; set; }

    [InverseProperty("Simulation")]
    public virtual ICollection<Simulationsession> Simulationsessions { get; set; } = new List<Simulationsession>();
}
