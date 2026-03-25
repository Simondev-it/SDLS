using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IReportCategoryRepository
    {
        Task<List<ReportCategory>> GetAllAsync();
        Task<ReportCategory?> GetByIdAsync(Guid id);
        Task<ReportCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ReportCategory entity);
        Task UpdateAsync(ReportCategory entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}