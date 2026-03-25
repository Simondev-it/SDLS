using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISignCategoryRepository
    {
        Task<List<SignCategory>> GetAllAsync();
        Task<SignCategory?> GetByIdAsync(Guid id);
        Task<SignCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SignCategory entity);
        Task UpdateAsync(SignCategory entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}