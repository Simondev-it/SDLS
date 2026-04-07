using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ReportCategory;

namespace SDLS.Services.Interfaces
{
    public interface IReportCategoryService
    {
        Task<List<ReportCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<ReportCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<ReportCategoryDTO> GetByIdAsync(Guid id);
        Task<ReportCategoryDTO> CreateAsync(ReportCategoryCreateDTO dto);
        Task<ReportCategoryDTO> UpdateAsync(Guid id, ReportCategoryUpdateDTO dto);
        Task<ReportCategoryDTO> DeleteSoftAsync(Guid id);
        Task<ReportCategoryDTO> DeleteHardAsync(Guid id);
    }
}