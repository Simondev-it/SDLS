using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;

namespace SDLS.Repositories.Helper
{
    public class ExecutionStrategyRepository : IExecutionStrategyRepository
    {
        private readonly AppDbContext _context;

        public ExecutionStrategyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await action();
            });
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                return await action();
            });
        }
    }
}
