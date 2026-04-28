using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Report;

namespace SDLS.Services.Interfaces
{
    public interface IReportService
    {
        Task<PagedResult<ReportDTO>> GetAllAsync(
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
            int page = 1,
            int pageSize = 20);

        Task<ReportDTO> GetByIdAsync(Guid id);
        Task<ReportDTO> CreateAsync(ReportCreateDTO dto);
        Task<ReportDTO> UpdateAsync(Guid id, ReportUpdateDTO dto);
        Task<ReportDTO> ApproveAsync(Guid id, ReportResolveActionDTO dto);
        Task<ReportDTO> DisapproveAsync(Guid id, ReportResolveActionDTO dto);
        Task<ReportDTO> DeleteSoftAsync(Guid id);
        Task<ReportDTO> DeleteHardAsync(Guid id);
    }
}