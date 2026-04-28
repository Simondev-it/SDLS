using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs.DrivingLicense;
using SDLS.Model.DTOs.UserLicense;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class UserLicenseRepository : GenericRepository<UserLicense>, IUserLicenseRepository
    {
        public async Task<List<UserLicense>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.UserLicenses
                .Include(x => x.DrivingLicense)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (drivingLicenseId.HasValue)
                query = query.Where(x => x.DrivingLicenseId == drivingLicenseId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.DrivingLicense == null || x.DrivingLicense.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<UserLicense?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.UserLicenses
                .Include(x => x.DrivingLicense)
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.DrivingLicense == null || x.DrivingLicense.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<UserLicense?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.UserLicenses
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<UserLicense>> GetByUserAndDrivingLicenseAsync(Guid? userId, Guid? drivingLicenseId)
        {
            IQueryable<UserLicense> query = _context.UserLicenses
                .Include(x => x.DrivingLicense);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (drivingLicenseId.HasValue)
                query = query.Where(x => x.DrivingLicenseId == drivingLicenseId.Value);

            return await query
                .AsNoTracking()
                .ToListAsync();
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

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.UserLicenses
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var entity = await _context.UserLicenses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _context.UserLicenses.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<UserLicenseDTO?> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserLicenses
                .Where(x => x.UserId == userId)
                .Include(x => x.DrivingLicense) // 🔥 bắt buộc
                .Select(x => new UserLicenseDTO
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    DrivingLicenseId = x.DrivingLicenseId,
                    Status = x.Status,
                    CreateAt = x.CreateAt,
                    UpdateAt = x.UpdateAt,

                    DrivingLicense = new DrivingLicenseDTO
                    {
                        Id = x.DrivingLicense.Id,
                        Name = x.DrivingLicense.Name
                    }
                })
                .FirstOrDefaultAsync();
        }
    }
}