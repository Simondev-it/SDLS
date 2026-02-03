using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("usernotification")]
public partial class Usernotification
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("notificationid")]
    public Guid? Notificationid { get; set; }

    [Column("userid")]
    public Guid? Userid { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Notificationid")]
    [InverseProperty("Usernotifications")]
    public virtual Notification? Notification { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Usernotifications")]
    public virtual User? User { get; set; }
}
