using SDLS.Model.DTOs.SignCategory;

namespace SDLS.Model.DTOs.TrafficSign
{
    public class TrafficSignDTO
    {
        public Guid Id { get; set; }
        public Guid SignCategoryId { get; set; }
        public int? Index { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public string? VectorData { get; set; }
        public string? Image { get; set; }
        public int? Status { get; set; }

        public SignCategoryDTO? SignCategory { get; set; }
    }
}