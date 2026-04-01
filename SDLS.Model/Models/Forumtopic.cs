using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class ForumTopic
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
}
