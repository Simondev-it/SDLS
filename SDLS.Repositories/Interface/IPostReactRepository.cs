using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IPostReactRepository
    {
        Task<List<PostReact>> GetAllAsync();
        Task<PostReact?> GetByIdAsync(Guid id);
        Task<List<PostReact>> GetByUserAndForumPostAsync(Guid? userId, Guid? forumPostId);
        Task AddAsync(PostReact entity);
        Task UpdateAsync(PostReact entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}