using SDLS.Model.DTOs.Vehicle;

namespace SDLS.Model.DTOs.DrivingLicense
{
    public class DrivingLicenseCreateDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Binding { get; set; }

        public List<VehicleCreateDTO>? Vehicles { get; set; }
    }
}