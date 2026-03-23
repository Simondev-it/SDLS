using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationSessionRepository
    {
        Task<List<SimulationSession>> GetAllAsync(
            Guid? id = null,
            Guid? situationExamId = null,
            Guid? userId = null);

        Task<SimulationSession?> GetByIdAsync(Guid id);
        Task<SimulationSession?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationSession entity);
        Task UpdateAsync(SimulationSession entity);
        Task DeleteAsync(Guid id);
    }
}