using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ReportCategory;

namespace SDLS.Services.Interfaces
{
    public interface IReportCategoryService
    {
        Task<List<ReportCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<ReportCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<ReportCategoryDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(ReportCategoryCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ReportCategoryUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}