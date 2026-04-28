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
            int? status = null,
            Guid? userId = null,
            int page = 1,
            int pageSize = 20);

        Task<SituationExamDTO> GetByIdAsync(Guid id);
        Task<SituationExamDTO> CreateAsync(SituationExamCreateDTO dto);
        Task<SituationExamDTO> UpdateAsync(Guid id, SituationExamUpdateDTO dto);
        Task<SituationExamDTO> DeleteSoftAsync(Guid id);
        Task<SituationExamDTO> DeleteHardAsync(Guid id);
    }
}