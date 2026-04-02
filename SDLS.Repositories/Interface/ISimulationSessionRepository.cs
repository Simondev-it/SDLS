using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationSessionRepository
    {
        Task<List<SimulationSession>> GetAllAsync(
            Guid? id = null,
            Guid? situationExamId = null,
            Guid? userId = null,
            int? status = null,
            string? role = null);

        Task<SimulationSession?> GetByIdAsync(Guid id, string? role = null);
        Task<SimulationSession?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationSession entity);
        Task UpdateAsync(SimulationSession entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}