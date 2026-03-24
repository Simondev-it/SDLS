using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SignCategory;

namespace SDLS.Services.Interfaces
{
    public interface ISignCategoryService
    {
        Task<List<SignCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<SignCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<SignCategoryDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SignCategoryCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SignCategoryUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}