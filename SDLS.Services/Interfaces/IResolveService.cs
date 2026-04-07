using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Resolve;

namespace SDLS.Services.Interfaces
{
    public interface IResolveService
    {
        Task<PagedResult<ResolveDTO>> GetAllAsync(
            Guid? id = null,
            Guid? reportId = null,
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<ResolveDTO?> GetByIdAsync(Guid id);
        Task<ResolveDTO> CreateAsync(ResolveCreateDTO dto);
        Task<ResolveDTO> UpdateAsync(Guid id, ResolveUpdateDTO dto);
        Task<ResolveDTO> DeleteSoftAsync(Guid id);
        Task<ResolveDTO> DeleteHardAsync(Guid id);
    }
}