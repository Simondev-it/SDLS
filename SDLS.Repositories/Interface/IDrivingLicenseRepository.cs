using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IDrivingLicenseRepository
    {
        Task<IEnumerable<DrivingLicense>> GetAllAsync();
        Task<DrivingLicense?> GetByIdAsync(Guid id);
        Task<DrivingLicense?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(DrivingLicense entity);
        Task UpdateAsync(DrivingLicense entity);

        // Thêm lại để tương thích service hiện tại
        Task DeleteAsync(Guid id);

        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}