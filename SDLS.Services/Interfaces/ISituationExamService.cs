using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SituationExam;

namespace SDLS.Services.Interfaces
{
    public interface ISituationExamService
    {
        Task<PagedResult<SituationExamDTO>> GetAllAsync(
            Guid? id = null,
            string? title = null,
            string? description = null,
            bool? isRandom = null,
            int page = 1,
            int pageSize = 20);

        Task<SituationExamDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SituationExamCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SituationExamUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}