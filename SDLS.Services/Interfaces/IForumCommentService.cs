using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumComment;

namespace SDLS.Services.Interfaces
{
    public interface IForumCommentService
    {
        Task<PagedResult<ForumCommentDTO>> GetAllAsync(
            Guid? id = null,
            Guid? forumPostId = null,
            Guid? userId = null,
            string? content = null,
            int page = 1,
            int pageSize = 20);

        Task<ForumCommentDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(ForumCommentCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ForumCommentUpdateDTO dto);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}
