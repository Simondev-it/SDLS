using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("answer")]
[Index("Questionid", Name = "idx_answer_question")]
public partial class Answer
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionid")]
    public Guid? Questionid { get; set; }

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("iscorrect")]
    public bool? Iscorrect { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Questionid")]
    [InverseProperty("Answers")]
    public virtual Question? Question { get; set; }
}
