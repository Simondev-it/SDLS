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
        Task<bool> CreateAsync(ResolveCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ResolveUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}