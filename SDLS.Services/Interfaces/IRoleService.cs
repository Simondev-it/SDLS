using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Role;

namespace SDLS.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetListAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<RoleDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<RoleDTO> GetByIdAsync(Guid id);
        Task<RoleDTO> CreateAsync(RoleCreateDTO dto);
        Task<RoleDTO> UpdateAsync(Guid id, RoleUpdateDTO dto);
        Task<RoleDTO> DeleteSoftAsync(Guid id);
        Task<RoleDTO> DeleteHardAsync(Guid id);
    }
}