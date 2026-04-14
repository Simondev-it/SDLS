using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Model.Helpers;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class DrivingLicenseRepository : GenericRepository<DrivingLicense>, IDrivingLicenseRepository
    {
        public async Task<IEnumerable<DrivingLicense>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? vehicleName = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<DrivingLicense> query = isPrivileged
                ? _context.DrivingLicenses.Include(x => x.Vehicles)
                : _context.DrivingLicenses.Include(x => x.Vehicles.Where(v => v.Status != 0));

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{name}%"));

            if (!string.IsNullOrWhiteSpace(description))
                query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{description}%"));

            if (!string.IsNullOrWhiteSpace(vehicleName))
                query = query.Where(x => x.Vehicles.Any(v => v.Status != 0 && v.Name != null && EF.Functions.ILike(v.Name, $"%{vehicleName}%")));

            query = query.ApplyRoleFilter(role);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<DrivingLicense?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<DrivingLicense> query = isPrivileged
                ? _context.DrivingLicenses.Include(x => x.Vehicles)
                : _context.DrivingLicenses.Include(x => x.Vehicles.Where(v => v.Status != 0));

            query = query.Where(x => x.Id == id)
                         .ApplyRoleFilter(role);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<DrivingLicense?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.DrivingLicenses
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.DrivingLicenses
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id && (x.Status == 1 || x.Status == 0));

            if (existing == null) return;

            var now = DateTimeHelper.GetVietnamNow();
            var nextStatus = existing.Status == 0 ? 1 : 0;

            existing.Status = nextStatus;
            existing.UpdateAt = now;

            if (nextStatus == 0)
            {
                foreach (var vehicle in existing.Vehicles.Where(v => v.Status == 1))
                {
                    vehicle.Status = 0;
                    vehicle.UpdateAt = now;
                }
            }
            else
            {
                foreach (var vehicle in existing.Vehicles.Where(v => v.Status == 0))
                {
                    vehicle.Status = 1;
                    vehicle.UpdateAt = now;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.DrivingLicenses
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null) return;

            if (existing.Vehicles.Any())
                _context.Vehicles.RemoveRange(existing.Vehicles);

            _context.DrivingLicenses.Remove(existing);
            await _context.SaveChangesAsync();
        }

    }
}