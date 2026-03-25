using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ICommentVoteRepository
    {
        Task<List<CommentVote>> GetAllAsync();
        Task<CommentVote?> GetByIdAsync(Guid id);
        Task<List<CommentVote>> GetByUserAndForumCommentAsync(Guid? userId, Guid? forumCommentId);
        Task AddAsync(CommentVote entity);
        Task UpdateAsync(CommentVote entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}