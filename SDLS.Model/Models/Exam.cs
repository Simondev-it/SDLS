using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("exam")]
[Index("Userid", Name = "idx_exam_user")]
public partial class Exam
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("owndone")]
    public int? Owndone { get; set; }

    [Column("totalquestion")]
    public int? Totalquestion { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<Examsession> Examsessions { get; set; } = new List<Examsession>();

    [ForeignKey("Userid")]
    [InverseProperty("Exams")]
    public virtual User? User { get; set; }
}
