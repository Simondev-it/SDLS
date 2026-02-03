using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("resolve")]
public partial class Resolve
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("reportid")]
    public Guid? Reportid { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string? Title { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Reportid")]
    [InverseProperty("Resolves")]
    public virtual Report? Report { get; set; }
}
