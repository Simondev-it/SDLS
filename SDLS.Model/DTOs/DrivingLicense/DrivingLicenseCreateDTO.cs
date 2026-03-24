using SDLS.Model.DTOs.Vehicle;

namespace SDLS.Model.DTOs.DrivingLicense
{
    public class DrivingLicenseCreateDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // Có thể null theo yêu cầu
        public List<VehicleCreateDTO>? Vehicles { get; set; }
    }
}