using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IReportCategoryRepository
    {
        Task<List<ReportCategory>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<ReportCategory?> GetByIdAsync(Guid id, string? role = null);
        Task<ReportCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ReportCategory entity);
        Task UpdateAsync(ReportCategory entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}