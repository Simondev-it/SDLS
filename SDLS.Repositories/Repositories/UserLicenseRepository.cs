using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class UserLicenseRepository : GenericRepository<UserLicense>, IUserLicenseRepository
    {
        public async Task<List<UserLicense>> GetAllAsync()
        {
            return await _context.UserLicenses
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserLicense?> GetByIdAsync(Guid id)
        {
            return await _context.UserLicenses
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<UserLicense?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.UserLicenses
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<UserLicense>> GetByUserAndDrivingLicenseAsync(Guid? userId, Guid? drivingLicenseId)
        {
            var query = _context.UserLicenses.Where(x => x.Status == 1);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (drivingLicenseId.HasValue)
                query = query.Where(x => x.DrivingLicenseId == drivingLicenseId.Value);

            return await query.ToListAsync();
        }

        public async Task AddAsync(UserLicense entity)
        {
            await _context.UserLicenses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserLicense entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.UserLicenses.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}