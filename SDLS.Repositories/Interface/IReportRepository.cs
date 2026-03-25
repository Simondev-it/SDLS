using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? reportCategoryId = null,
            Guid? simulationId = null,
            Guid? forumPostId = null,
            Guid? forumCommentId = null,
            Guid? questionId = null,
            string? title = null,
            string? content = null,
            int? status = null);

        Task<Report?> GetByIdAsync(Guid id);
        Task<Report?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Report entity);
        Task UpdateAsync(Report entity);

        Task DeleteAsync(Guid id);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}