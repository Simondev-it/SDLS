using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs.ForumPost;

namespace SDLS.Services.Interfaces
{
    public interface IPostImageService
    {
        Task<IEnumerable<ForumPostImageDTO>> GetAllAsync();
        Task<ForumPostImageDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<ForumPostImageDTO>> GetByPostIdAsync(Guid postId);
        Task<ForumPostImageDTO> CreateAsync(IFormFile file, Guid postId, string? name = null);
        Task<bool> DeleteAsync(Guid id);
    }
}
