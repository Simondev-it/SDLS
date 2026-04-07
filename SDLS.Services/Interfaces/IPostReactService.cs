using SDLS.Model.DTOs;
using SDLS.Model.DTOs.PostReact;

namespace SDLS.Services.Interfaces
{
    public interface IPostReactService
    {
        Task<List<PostReactDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumPostId = null,
            string? reactType = null,
            int? status = null);

        Task<PagedResult<PostReactDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumPostId = null,
            string? reactType = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<PostReactDTO?> GetByIdAsync(Guid id);
        Task<PostReactDTO> CreateAsync(PostReactCreateDTO dto);
        Task<PostReactDTO> UpdateAsync(Guid id, PostReactUpdateDTO dto);
        Task<PostReactDTO> DeleteSoftAsync(Guid id);
        Task<PostReactDTO> DeleteHardAsync(Guid id);
    }
}