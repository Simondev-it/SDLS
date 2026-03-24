using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Tag;

namespace SDLS.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null);

        Task<PagedResult<TagDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null,
            int page = 1,
            int pageSize = 20);

        Task<TagDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(TagCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, TagUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}