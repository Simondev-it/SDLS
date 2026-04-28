using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            List<Guid>? reportCategoryIds = null,
            string? roleName = null,
            Guid? simulationId = null,
            Guid? forumPostId = null,
            Guid? forumCommentId = null,
            Guid? questionId = null,
            bool? hasSimulation = null,
            bool? hasForumPost = null,
            bool? hasForumComment = null,
            bool? hasQuestion = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? role = null);

        Task<Report?> GetByIdAsync(Guid id, string? role = null);
        Task<Report?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Report entity);
        Task UpdateAsync(Report entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}