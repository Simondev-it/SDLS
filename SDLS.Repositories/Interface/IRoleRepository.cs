using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<Role?> GetByIdAsync(Guid id, string? role = null);
        Task<Role?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Role entity);
        Task UpdateAsync(Role entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<Role?> GetByIdAsync(Guid id);

        Task<Role?> GetByNameAsync(string name);

        Task<IEnumerable<Role>> GetAllAsync();
    }
}