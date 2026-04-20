using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SystemConfig;

namespace SDLS.Services.Interfaces
{
    public interface ISystemConfigService
    {
        Task<List<SystemConfigDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<SystemConfigDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<SystemDashboardSummaryDTO> GetDashboardSummaryAsync();
        Task<(byte[] Content, string FileName, string ContentType)> ExportDashboardSummaryExcelAsync();

        Task<SystemConfigDTO> GetByIdAsync(Guid id);
        Task<SystemConfigDTO> CreateAsync(SystemConfigCreateDTO dto);
        Task<SystemConfigDTO> UpdateAsync(Guid id, SystemConfigUpdateDTO dto);
        Task<SystemConfigDTO> DeleteSoftAsync(Guid id);
        Task<SystemConfigDTO> DeleteHardAsync(Guid id);
    }
}
