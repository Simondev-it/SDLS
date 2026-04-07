using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumTopic;

namespace SDLS.Services.Interfaces
{
    public interface IForumTopicService
    {
        Task<List<ForumTopicDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<ForumTopicDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<ForumTopicDTO> GetByIdAsync(Guid id);
        Task<ForumTopicDTO> CreateAsync(ForumTopicCreateDTO dto);
        Task<ForumTopicDTO> UpdateAsync(Guid id, ForumTopicUpdateDTO dto);
        Task<ForumTopicDTO> DeleteSoftAsync(Guid id);
        Task<ForumTopicDTO> DeleteHardAsync(Guid id);
    }
}