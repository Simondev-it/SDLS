using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ITrafficSignRepository
    {
        Task<IEnumerable<TrafficSign>> GetAllAsync(
            Guid? id = null,
            Guid? signCategoryId = null,
            string? name = null,
            string? code = null,
            string? description = null,
            int? status = 1);

        Task<TrafficSign?> GetByIdAsync(Guid id);
        Task<TrafficSign?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(TrafficSign entity);
        Task UpdateAsync(TrafficSign entity);
        Task DeleteAsync(Guid id);
    }
}