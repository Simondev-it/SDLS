using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IForumCommentRepository
    {
        Task<IEnumerable<ForumComment>> GetAllAsync(
            Guid? id = null,
            Guid? forumPostId = null,
            Guid? userId = null,
            string? content = null,
            int? status = null,
            string? role = null);

        Task<ForumComment?> GetByIdAsync(Guid id, string? role = null);
        Task<ForumComment?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ForumComment entity);
        Task UpdateAsync(ForumComment entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
