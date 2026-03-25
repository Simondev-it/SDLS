using SDLS.Model.DTOs;
using SDLS.Model.DTOs.PostReact;

namespace SDLS.Services.Interfaces
{
    public interface IPostReactService
    {
        Task<List<PostReactDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? forumPostId = null, string? reactType = null);
        Task<PagedResult<PostReactDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? forumPostId = null, string? reactType = null, int page = 1, int pageSize = 20);
        Task<PostReactDTO?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(PostReactCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, PostReactUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}