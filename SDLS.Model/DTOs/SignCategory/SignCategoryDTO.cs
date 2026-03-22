namespace SDLS.Model.DTOs.SignCategory
{
    public class SignCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }
    }
}