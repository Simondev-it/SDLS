namespace SDLS.Model.DTOs.Vehicle
{
    public class VehicleDTO
    {
        public Guid Id { get; set; }
        public Guid DrivingLicenseId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }
    }
}