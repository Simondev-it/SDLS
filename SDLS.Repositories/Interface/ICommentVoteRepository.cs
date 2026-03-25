using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ICommentVoteRepository
    {
        Task<List<CommentVote>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumCommentId = null,
            int? status = null,
            string? role = null);

        Task<CommentVote?> GetByIdAsync(Guid id, string? role = null);
        Task<List<CommentVote>> GetByUserAndForumCommentAsync(Guid? userId, Guid? forumCommentId);
        Task AddAsync(CommentVote entity);
        Task UpdateAsync(CommentVote entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}