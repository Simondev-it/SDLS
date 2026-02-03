using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("trafficsign")]
public partial class Trafficsign
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("trafficsigncategoryid")]
    public Guid? Trafficsigncategoryid { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string? Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Trafficsigncategoryid")]
    [InverseProperty("Trafficsigns")]
    public virtual Trafficsigncategory? Trafficsigncategory { get; set; }
}
