using SDLS.Model.DTOs;
using SDLS.Model.DTOs.TrafficSign;
using Microsoft.AspNetCore.Http;

namespace SDLS.Services.Interfaces
{
    public interface ITrafficSignService
    {
        Task<PagedResult<TrafficSignDTO>> GetAllAsync(
            Guid? id = null,
            Guid? signCategoryId = null,
            string? name = null,
            string? code = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<TrafficSignDTO> GetByIdAsync(Guid id);
        Task<TrafficSignDTO> CreateAsync(TrafficSignCreateDTO dto);
        Task<List<TrafficSignDTO>> CreateManyAsync(List<TrafficSignCreateDTO> dtos);
        Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync();
        Task<List<TrafficSignDTO>> ImportAsync(IFormFile file);
        Task<TrafficSignDTO> UpdateAsync(Guid id, TrafficSignUpdateDTO dto);
        Task<TrafficSignDTO> DeleteSoftAsync(Guid id);
        Task<TrafficSignDTO> DeleteHardAsync(Guid id);
    }
}