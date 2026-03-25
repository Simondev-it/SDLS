using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationSession;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationSessionService
    {
        Task<PagedResult<SimulationSessionDTO>> GetAllAsync(
            Guid? id = null,
            Guid? situationExamId = null,
            Guid? userId = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationSessionDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SimulationSessionCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SimulationSessionUpdateDTO dto);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}