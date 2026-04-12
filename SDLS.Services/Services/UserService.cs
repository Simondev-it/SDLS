using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> UpdateAsync(Guid id, User user)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.RoleId = user.RoleId;

            await _userRepository.UpdateAsync(existing);
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            // Nếu bạn có soft delete thì sửa lại
            // user.IsDeleted = true;

            // Hard delete:
            await _userRepository.RemoveAsync(user);
            await _userRepository.SaveAsync();

            return true;
        }
    }
}
