using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("ExamDetail")]
[Index("AnswerId", "ExamSessionId", Name = "ExamDetail_answerId_examSessionId_key", IsUnique = true)]
public partial class ExamDetail
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("answerId")]
    public Guid AnswerId { get; set; }

    [Column("examSessionId")]
    public Guid ExamSessionId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("AnswerId")]
    [InverseProperty("ExamDetails")]
    public virtual Answer Answer { get; set; } = null!;

    [ForeignKey("ExamSessionId")]
    [InverseProperty("ExamDetails")]
    public virtual ExamSession ExamSession { get; set; } = null!;
}
