using SDLS.Model.DTOs.UserLicense;
using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IUserLicenseRepository
    {
        Task<List<UserLicense>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null,
            int? status = null,
            string? role = null);

        Task<UserLicense?> GetByIdAsync(Guid id, string? role = null);
        Task<UserLicense?> GetByIdForUpdateAsync(Guid id);
        Task<List<UserLicense>> GetByUserAndDrivingLicenseAsync(Guid? userId, Guid? drivingLicenseId);
        Task AddAsync(UserLicense entity);
        Task UpdateAsync(UserLicense entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<UserLicenseDTO?> GetByUserIdAsync(Guid userId);
    }
}