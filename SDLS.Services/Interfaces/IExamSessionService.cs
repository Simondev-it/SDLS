using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ExamSession;
using System;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IExamSessionService
    {
        Task<PagedResult<ExamSessionDTO>> GetAllAsync(
            Guid? examId = null,
            Guid? userId = null,
            int page = 1,
            int pageSize = 20);

        Task<ExamSessionDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(ExamSessionCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ExamSessionUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}