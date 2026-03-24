using SDLS.Model.DTOs.Vehicle;

namespace SDLS.Model.DTOs.DrivingLicense
{
    public class DrivingLicenseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }
        public List<VehicleDTO> Vehicles { get; set; } = new();
    }
}