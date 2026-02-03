using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("savedquestion")]
[Index("Userid", "Questionid", Name = "savedquestion_userid_questionid_key", IsUnique = true)]
public partial class Savedquestion
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("questionid")]
    public Guid? Questionid { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Questionid")]
    [InverseProperty("Savedquestions")]
    public virtual Question? Question { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Savedquestions")]
    public virtual User? User { get; set; }
}
