using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Report;

namespace SDLS.Services.Interfaces
{
    public interface IReportService
    {
        Task<PagedResult<ReportDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? reportCategoryId = null,
            Guid? simulationId = null,
            Guid? forumPostId = null,
            Guid? forumCommentId = null,
            Guid? questionId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<ReportDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(ReportCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ReportUpdateDTO dto);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}