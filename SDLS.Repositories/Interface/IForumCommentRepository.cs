using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Models;
using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IForumCommentRepository
    {
        Task<IEnumerable<ForumComment>> GetAllAsync();
        Task<ForumComment?> GetByIdAsync(Guid id);
        Task<ForumComment?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ForumComment entity);
        Task UpdateAsync(ForumComment entity);
        Task DeleteAsync(Guid id);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
