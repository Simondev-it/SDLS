using SDLS.Model.DTOs;
using SDLS.Model.DTOs.UserLicense;

namespace SDLS.Services.Interfaces
{
    public interface IUserLicenseService
    {
        Task<List<UserLicenseDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null);

        Task<PagedResult<UserLicenseDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null,
            int page = 1,
            int pageSize = 20);

        Task<UserLicenseDTO?> GetByIdAsync(Guid id);
        Task<UserLicenseDTO> CreateAsync(UserLicenseCreateDTO dto);
        Task<UserLicenseDTO?> UpdateAsync(Guid id, UserLicenseUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}