using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("examsession")]
[Index("Examid", Name = "idx_exam_session_exam")]
[Index("Userid", Name = "idx_exam_session_user")]
public partial class Examsession
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("examid")]
    public Guid? Examid { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("score")]
    public int? Score { get; set; }

    [Column("ispassed")]
    public bool? Ispassed { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Examid")]
    [InverseProperty("Examsessions")]
    public virtual Exam? Exam { get; set; }

    [InverseProperty("Examsession")]
    public virtual ICollection<Examdetail> Examdetails { get; set; } = new List<Examdetail>();

    [ForeignKey("Userid")]
    [InverseProperty("Examsessions")]
    public virtual User? User { get; set; }
}
