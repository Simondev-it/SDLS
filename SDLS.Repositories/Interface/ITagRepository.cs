using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(Guid id);
        Task<Tag?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Tag entity);
        Task UpdateAsync(Tag entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}