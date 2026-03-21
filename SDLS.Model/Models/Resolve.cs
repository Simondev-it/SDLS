using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("Resolve")]
public partial class Resolve
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("reportId")]
    public Guid ReportId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("content")]
    [StringLength(255)]
    public string Content { get; set; } = null!;

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("ReportId")]
    [InverseProperty("Resolves")]
    public virtual Report Report { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Resolves")]
    public virtual User User { get; set; } = null!;
}
