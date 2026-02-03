using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("trafficsigncategory")]
public partial class Trafficsigncategory
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("starcategoryid")]
    public Guid? Starcategoryid { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string? Name { get; set; }

    [Column("code")]
    [StringLength(50)]
    public string? Code { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Starcategoryid")]
    [InverseProperty("Trafficsigncategories")]
    public virtual Starcategory? Starcategory { get; set; }

    [InverseProperty("Trafficsigncategory")]
    public virtual ICollection<Trafficsign> Trafficsigns { get; set; } = new List<Trafficsign>();
}
