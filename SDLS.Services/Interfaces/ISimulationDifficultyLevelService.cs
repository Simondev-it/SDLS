using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationDifficultyLevel;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationDifficultyLevelService
    {
        Task<List<SimulationDifficultyLevelDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<SimulationDifficultyLevelDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<(byte[] Content, string FileName, string ContentType)> ExportToExcelAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<SimulationDifficultyLevelDTO> GetByIdAsync(Guid id);
        Task<SimulationDifficultyLevelDTO> CreateAsync(SimulationDifficultyLevelCreateDTO dto);
        Task<SimulationDifficultyLevelDTO> UpdateAsync(Guid id, SimulationDifficultyLevelUpdateDTO dto);
        Task<SimulationDifficultyLevelDTO> DeleteSoftAsync(Guid id);
        Task<SimulationDifficultyLevelDTO> DeleteHardAsync(Guid id);
    }
}