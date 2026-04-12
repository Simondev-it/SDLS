using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISystemConfigRepository
    {
        Task<List<SystemConfig>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<SystemConfig?> GetByIdAsync(Guid id, string? role = null);
        Task<SystemConfig?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SystemConfig entity);
        Task UpdateAsync(SystemConfig entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}
