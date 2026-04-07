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
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationSessionDTO> GetByIdAsync(Guid id);
        Task<SimulationSessionDTO> CreateAsync(SimulationSessionCreateDTO dto);
        Task<SimulationSessionDTO> UpdateAsync(Guid id, SimulationSessionUpdateDTO dto);
        Task<SimulationSessionDTO> DeleteSoftAsync(Guid id);
        Task<SimulationSessionDTO> DeleteHardAsync(Guid id);
    }
}