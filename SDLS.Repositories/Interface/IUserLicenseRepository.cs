using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IUserLicenseRepository
    {
        Task<List<UserLicense>> GetAllAsync();
        Task<UserLicense?> GetByIdAsync(Guid id);
        Task<UserLicense?> GetByIdForUpdateAsync(Guid id);
        Task<List<UserLicense>> GetByUserAndDrivingLicenseAsync(Guid? userId, Guid? drivingLicenseId);
        Task AddAsync(UserLicense entity);
        Task UpdateAsync(UserLicense entity);

        Task DeleteAsync(Guid id);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}