using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("payment")]
public partial class Payment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("method")]
    [StringLength(100)]
    public string? Method { get; set; }

    [Column("amount")]
    [Precision(10, 2)]
    public decimal? Amount { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("response")]
    public string? Response { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Payments")]
    public virtual User? User { get; set; }
}
