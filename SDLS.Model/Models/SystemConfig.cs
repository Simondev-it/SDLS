using System;
namespace SDLS.Model.Models
{
    public class SystemConfig
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int Value { get; set; }

        public string? Description { get; set; }

        public DateTime? CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public int? Status { get; set; }
    }
}
