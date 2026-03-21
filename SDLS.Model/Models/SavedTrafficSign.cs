using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("SavedTrafficSign")]
[Index("TrafficSignId", "UserId", Name = "SavedTrafficSign_trafficSignId_userId_key", IsUnique = true)]
public partial class SavedTrafficSign
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("trafficSignId")]
    public Guid TrafficSignId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("TrafficSignId")]
    [InverseProperty("SavedTrafficSigns")]
    public virtual TrafficSign TrafficSign { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SavedTrafficSigns")]
    public virtual User User { get; set; } = null!;
}
