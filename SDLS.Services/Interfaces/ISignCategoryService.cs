using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SignCategory;

namespace SDLS.Services.Interfaces
{
    public interface ISignCategoryService
    {
        Task<List<SignCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<SignCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<SignCategoryDTO> GetByIdAsync(Guid id);
        Task<SignCategoryDTO> CreateAsync(SignCategoryCreateDTO dto);
        Task<SignCategoryDTO> UpdateAsync(Guid id, SignCategoryUpdateDTO dto);
        Task<SignCategoryDTO> DeleteSoftAsync(Guid id);
        Task<SignCategoryDTO> DeleteHardAsync(Guid id);
    }
}