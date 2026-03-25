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
            int page = 1,
            int pageSize = 20);

        Task<ExamDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(ExamCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ExamUpdateDTO dto);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}
