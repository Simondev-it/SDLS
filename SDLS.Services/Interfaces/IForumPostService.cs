using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumPost;

namespace SDLS.Services.Interfaces
{
    public interface IForumPostService
    {
        Task<PagedResult<ForumPostDTO>> GetAllAsync(
            Guid? id = null,
            Guid? forumTopicId = null,
            Guid? userId = null,
            string? name = null,
            string? title = null,
            string? content = null,
            int? status = 1,
            int page = 1,
            int pageSize = 20);

        Task<ForumPostDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(ForumPostCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ForumPostUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
