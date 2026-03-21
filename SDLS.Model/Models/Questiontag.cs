using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("QuestionTag")]
[Index("QuestionId", "TagId", Name = "QuestionTag_questionId_tagId_key", IsUnique = true)]
public partial class QuestionTag
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionId")]
    public Guid QuestionId { get; set; }

    [Column("tagId")]
    public Guid TagId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("QuestionTags")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("QuestionTags")]
    public virtual Tag Tag { get; set; } = null!;
}
