using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IForumPostRepository
    {
        Task<IEnumerable<ForumPost>> GetAllAsync(
            Guid? id = null,
            Guid? forumTopicId = null,
            Guid? userId = null,
            string? name = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? role = null);

        Task<ForumPost?> GetByIdAsync(Guid id, string? role = null);
        Task<ForumPost?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ForumPost forumPost);
        Task UpdateAsync(ForumPost forumPost);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<List<PostImage>> GetPostImagesByPostIdsAsync(List<Guid> postIds, string? role = null);
        Task<List<PostImage>> GetPostImagesByPostIdForUpdateAsync(Guid postId);
        void AddPostImages(IEnumerable<PostImage> images);
        void RemovePostImages(IEnumerable<PostImage> images);
        Task SoftDeletePostImagesAsync(Guid postId, DateTime now);
    }
}
