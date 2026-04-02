using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IForumTopicRepository
    {
        Task<List<ForumTopic>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<ForumTopic?> GetByIdAsync(Guid id, string? role = null);
        Task<ForumTopic?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ForumTopic entity);
        Task UpdateAsync(ForumTopic entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}