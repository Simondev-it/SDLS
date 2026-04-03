using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IDrivingLicenseRepository
    {
        Task<IEnumerable<DrivingLicense>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? vehicleName = null,
            int? status = null,
            string? role = null);

        Task<DrivingLicense?> GetByIdAsync(Guid id, string? role = null);
        Task<DrivingLicense?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(DrivingLicense entity);
        Task UpdateAsync(DrivingLicense entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}