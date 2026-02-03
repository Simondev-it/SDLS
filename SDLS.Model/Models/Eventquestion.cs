using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("eventquestion")]
public partial class Eventquestion
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("eventid")]
    public Guid? Eventid { get; set; }

    [Column("questionid")]
    public Guid? Questionid { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Eventid")]
    [InverseProperty("Eventquestions")]
    public virtual Event? Event { get; set; }

    [ForeignKey("Questionid")]
    [InverseProperty("Eventquestions")]
    public virtual Question? Question { get; set; }
}
