using SDLS.Model.DTOs.Vehicle;

namespace SDLS.Model.DTOs.DrivingLicense
{
    public class DrivingLicenseUpdateDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }
        public string? Binding { get; set; }

        public List<VehicleUpdateDTO>? Vehicles { get; set; }
    }
}