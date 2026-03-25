using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IAnswerRepository
    {
        Task AddAsync(Answer answer);
        Task UpdateAsync(Answer answer);

        // Giữ hành vi cũ (hard delete)
        Task DeleteAsync(Guid id);

        // Mới
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}
