using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISignCategoryRepository
    {
        Task<List<SignCategory>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<SignCategory?> GetByIdAsync(Guid id, string? role = null);
        Task<SignCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SignCategory entity);
        Task UpdateAsync(SignCategory entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}