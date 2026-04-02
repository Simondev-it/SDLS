using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Helper
{
    public interface IExecutionStrategyRepository
    {
        Task ExecuteAsync(Func<Task> action);
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
}
