using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Tag;
using Microsoft.AspNetCore.Http;

namespace SDLS.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null,
            int? status = null);

        Task<PagedResult<TagDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<TagDTO> GetByIdAsync(Guid id);
        Task<TagDTO> CreateAsync(TagCreateDTO dto);
        Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync();
        Task<List<TagDTO>> ImportAsync(IFormFile file);
        Task<TagDTO> UpdateAsync(Guid id, TagUpdateDTO dto);
        Task<TagDTO> DeleteSoftAsync(Guid id);
        Task<TagDTO> DeleteHardAsync(Guid id);
    }
}