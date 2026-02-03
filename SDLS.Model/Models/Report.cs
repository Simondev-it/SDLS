using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("report")]
[Index("Questionid", Name = "idx_report_question")]
[Index("Userid", Name = "idx_report_user")]
public partial class Report
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionid")]
    public Guid? Questionid { get; set; }

    [Column("reportcategoryid")]
    public Guid? Reportcategoryid { get; set; }

    [Column("simulationid")]
    public Guid? Simulationid { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string? Title { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Questionid")]
    [InverseProperty("Reports")]
    public virtual Question? Question { get; set; }

    [ForeignKey("Reportcategoryid")]
    [InverseProperty("Reports")]
    public virtual Reportcategory? Reportcategory { get; set; }

    [InverseProperty("Report")]
    public virtual ICollection<Resolve> Resolves { get; set; } = new List<Resolve>();

    [ForeignKey("Simulationid")]
    [InverseProperty("Reports")]
    public virtual Simulationscenario? Simulation { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Reports")]
    public virtual User? User { get; set; }
}
