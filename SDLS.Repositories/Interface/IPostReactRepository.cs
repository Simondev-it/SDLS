using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IPostReactRepository
    {
        Task<List<PostReact>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumPostId = null,
            int? status = null,
            string? role = null);

        Task<PostReact?> GetByIdAsync(Guid id, string? role = null);
        Task<List<PostReact>> GetByUserAndForumPostAsync(Guid? userId, Guid? forumPostId);
        Task AddAsync(PostReact entity);
        Task UpdateAsync(PostReact entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}