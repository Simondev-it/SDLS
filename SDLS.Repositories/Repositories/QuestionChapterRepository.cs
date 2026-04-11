using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class QuestionChapterRepository : GenericRepository<QuestionChapter>, IQuestionChapterRepository
    {
        public async Task<IEnumerable<QuestionChapter>> GetAllAsync(
            Guid? id = null,
            Guid? drivingLicenseId = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.QuestionChapters
                .Include(x => x.DrivingLicense)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (drivingLicenseId.HasValue)
                query = query.Where(x => x.DrivingLicenseId == drivingLicenseId.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var keyword = description.Trim();
                query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.DrivingLicense == null || x.DrivingLicense.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<QuestionChapter?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.QuestionChapters
                .Include(x => x.DrivingLicense)
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.DrivingLicense == null || x.DrivingLicense.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<QuestionChapter?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.QuestionChapters
                .Include(x => x.DrivingLicense)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(QuestionChapter chapter)
        {
            await _context.QuestionChapters.AddAsync(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionChapter chapter)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var chapter = await _context.QuestionChapters.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (chapter == null) return;

            chapter.Status = 0;
            chapter.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var chapter = await _context.QuestionChapters.FirstOrDefaultAsync(x => x.Id == id);
            if (chapter == null) return;

            _context.QuestionChapters.Remove(chapter);
            await _context.SaveChangesAsync();
        }
    }
}