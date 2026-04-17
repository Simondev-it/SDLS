using SDLS.Model.DTOs;
using SDLS.Model.DTOs.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync(
            Guid? id = null,
            Guid? roleId = null,
            string? email = null,
            string? name = null,
            int? status = null);
        Task<PagedResult<UserDTO>> GetAllWithPagingAsync(
            Guid? id = null,
            Guid? roleId = null,
            string? email = null,
            string? name = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);
        Task<UserDTO?> GetByIdAsync(Guid id);
        Task<UserDTO?> GetByEmailAsync(string email);
        Task<UserDTO> CreateAsync(UserCreateDTO user);
        Task<UserDTO?> UpdateAsync(Guid id, UserUpdateDTO user);
        Task<UserDTO?> ToggleActiveStatusAsync(Guid id);
        Task<UserDTO?> ToggleLockStatusAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
