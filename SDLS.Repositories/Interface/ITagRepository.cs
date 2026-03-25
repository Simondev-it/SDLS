using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null,
            int? status = null,
            string? role = null);

        Task<Tag?> GetByIdAsync(Guid id, string? role = null);
        Task<Tag?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Tag entity);
        Task UpdateAsync(Tag entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}