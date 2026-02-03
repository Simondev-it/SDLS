using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("event")]
public partial class Event
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("startdate", TypeName = "timestamp without time zone")]
    public DateTime? Startdate { get; set; }

    [Column("enddate", TypeName = "timestamp without time zone")]
    public DateTime? Enddate { get; set; }

    [Column("location")]
    [StringLength(255)]
    public string? Location { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<Eventquestion> Eventquestions { get; set; } = new List<Eventquestion>();

    [InverseProperty("Event")]
    public virtual ICollection<Eventsession> Eventsessions { get; set; } = new List<Eventsession>();
}
