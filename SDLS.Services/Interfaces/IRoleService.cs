using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Role;

namespace SDLS.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetListAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<RoleDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<RoleDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(RoleCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, RoleUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}