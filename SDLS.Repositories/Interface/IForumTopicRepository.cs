using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IForumTopicRepository
    {
        Task<List<ForumTopic>> GetAllAsync();
        Task<ForumTopic?> GetByIdAsync(Guid id);
        Task<ForumTopic?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ForumTopic entity);
        Task UpdateAsync(ForumTopic entity);
        Task DeleteAsync(Guid id);
    }
}