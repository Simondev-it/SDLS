using SDLS.Model.DTOs;
using SDLS.Model.DTOs.DrivingLicense;

namespace SDLS.Services.Interfaces
{
    public interface IDrivingLicenseService
    {
        Task<List<DrivingLicenseDTO>> GetAllNoPagingAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? vehicleName = null);

        Task<PagedResult<DrivingLicenseDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? vehicleName = null,
            int page = 1,
            int pageSize = 20);

        Task<DrivingLicenseDTO> GetByIdAsync(Guid id);
        Task<DrivingLicenseDTO> CreateAsync(DrivingLicenseCreateDTO dto);
        Task<DrivingLicenseDTO> UpdateAsync(Guid id, DrivingLicenseUpdateDTO dto);
        Task<DrivingLicenseDTO> DeleteSoftAsync(Guid id);
        Task<DrivingLicenseDTO> DeleteHardAsync(Guid id);
    }
}