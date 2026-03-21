using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class DrivingLicenseRepository : GenericRepository<DrivingLicense>, IDrivingLicenseRepository
    {
        public async Task<IEnumerable<DrivingLicense>> GetAllAsync()
        {
            return await _context.DrivingLicenses
                .Include(x => x.Vehicles.Where(v => v.Status == 1))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DrivingLicense?> GetByIdAsync(Guid id)
        {
            return await _context.DrivingLicenses
                .Include(x => x.Vehicles.Where(v => v.Status == 1))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<DrivingLicense?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.DrivingLicenses
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(DrivingLicense entity)
        {
            await _context.DrivingLicenses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DrivingLicense entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.DrivingLicenses
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            var now = DateTime.UtcNow.ToLocalTime();

            existing.Status = 0;
            existing.UpdateAt = now;

            foreach (var vehicle in existing.Vehicles.Where(v => v.Status == 1))
            {
                vehicle.Status = 0;
                vehicle.UpdateAt = now;
            }

            await _context.SaveChangesAsync();
        }
    }
}