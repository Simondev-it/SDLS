using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Exam;
using System;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IExamService
    {
        Task<PagedResult<ExamDTO>> GetAllAsync(
            Guid? userId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<ExamDTO> GetByIdAsync(Guid id);
        Task<ExamDTO> CreateAsync(ExamCreateDTO dto);
        Task<ExamDTO> UpdateAsync(Guid id, ExamUpdateDTO dto);
        Task<ExamDTO> DeleteSoftAsync(Guid id);
        Task<ExamDTO> DeleteHardAsync(Guid id);
    }
}
