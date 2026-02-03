using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("questiontag")]
public partial class Questiontag
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionid")]
    public Guid? Questionid { get; set; }

    [Column("tagid")]
    public Guid? Tagid { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Questionid")]
    [InverseProperty("Questiontags")]
    public virtual Question? Question { get; set; }
}
