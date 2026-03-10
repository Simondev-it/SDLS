using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class ForumPost
{
    public Guid Id { get; set; }

    public Guid ForumTopicId { get; set; }

    public Guid UserId { get; set; }

    public string? Name { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? ViewCount { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    public virtual ForumTopic ForumTopic { get; set; } = null!;

    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();

    public virtual ICollection<PostReact> PostReacts { get; set; } = new List<PostReact>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User User { get; set; } = null!;
}
