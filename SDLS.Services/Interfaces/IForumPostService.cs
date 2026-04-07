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
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<ForumPostDTO> GetByIdAsync(Guid id);
        Task<ForumPostDTO> CreateAsync(ForumPostCreateDTO dto);
        Task<ForumPostDTO> CreateByInstructorAsync(ForumPostCreateDTO dto);
        Task<ForumPostDTO> UpdateAsync(Guid id, ForumPostUpdateDTO dto);
        Task<ForumPostDTO> ApproveAsync(Guid id);
        Task<ForumPostDTO> DisapproveAsync(Guid id);
        Task<ForumPostDTO> DeleteSoftAsync(Guid id);
        Task<ForumPostDTO> DeleteHardAsync(Guid id);
    }
}
