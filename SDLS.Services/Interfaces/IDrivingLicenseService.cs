using SDLS.Model.DTOs;
using SDLS.Model.DTOs.DrivingLicense;

namespace SDLS.Services.Interfaces
{
    public interface IDrivingLicenseService
    {
        Task<PagedResult<DrivingLicenseDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? vehicleName = null,
            int page = 1,
            int pageSize = 20);

        Task<DrivingLicenseDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(DrivingLicenseCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, DrivingLicenseUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}