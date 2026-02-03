using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("eventsession")]
public partial class Eventsession
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("eventid")]
    public Guid? Eventid { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Eventid")]
    [InverseProperty("Eventsessions")]
    public virtual Event? Event { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Eventsessions")]
    public virtual User? User { get; set; }
}
