using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("examdetail")]
public partial class Examdetail
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("examsessionid")]
    public Guid? Examsessionid { get; set; }

    [Column("questionid")]
    public Guid? Questionid { get; set; }

    [Column("useranswerid")]
    public Guid? Useranswerid { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Examsessionid")]
    [InverseProperty("Examdetails")]
    public virtual Examsession? Examsession { get; set; }

    [ForeignKey("Questionid")]
    [InverseProperty("Examdetails")]
    public virtual Question? Question { get; set; }
}
