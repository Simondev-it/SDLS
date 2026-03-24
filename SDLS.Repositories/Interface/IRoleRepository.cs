using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Role entity);
        Task UpdateAsync(Role entity);
        Task DeleteAsync(Guid id);
    }
}