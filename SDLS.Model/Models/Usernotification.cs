using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("UserNotification")]
public partial class UserNotification
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("notificationId")]
    public Guid NotificationId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("NotificationId")]
    [InverseProperty("UserNotifications")]
    public virtual Notification Notification { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserNotifications")]
    public virtual User User { get; set; } = null!;
}
