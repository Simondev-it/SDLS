using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid id);
        Task CreateAsync(User user);
        Task<int> SaveAsync();

        Task<IEnumerable<User>> GetAllAsync();

        Task AddAsync(User user);

        Task UpdateAsync(User user);
    }
}
