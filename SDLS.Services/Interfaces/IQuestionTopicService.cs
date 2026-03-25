using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionTopic;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionTopicService
    {
        Task<List<QuestionTopicDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<QuestionTopicDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<QuestionTopicDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(QuestionTopicCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, QuestionTopicUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}