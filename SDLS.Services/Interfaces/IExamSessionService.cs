using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ExamSession;

namespace SDLS.Services.Interfaces
{
    public interface IExamSessionService
    {
        Task<PagedResult<ExamSessionDTO>> GetAllAsync(
            Guid? examId = null,
            Guid? userId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<ExamSessionDTO> GetByIdAsync(Guid id);
        Task<ExamSessionDTO> CreateAsync(ExamSessionCreateDTO dto);
        Task<ExamSessionDTO> UpdateAsync(Guid id, ExamSessionUpdateDTO dto);
        Task<ExamSessionDTO> DeleteSoftAsync(Guid id);
        Task<ExamSessionDTO> DeleteHardAsync(Guid id);
    }
}