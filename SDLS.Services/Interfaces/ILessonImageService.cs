using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs.LessonImage;

namespace SDLS.Services.Interfaces
{
    public interface ILessonImageService
    {
        Task<IEnumerable<LessonImageDTO>> GetAllAsync();
        Task<LessonImageDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<LessonImageDTO>> GetByLessonIdAsync(Guid lessonId);
        Task<LessonImageDTO> CreateAsync(IFormFile file, Guid lessonId, string? name = null);
        Task<bool> DeleteAsync(Guid id);
    }
}
