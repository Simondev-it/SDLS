using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ForumTopicRepository : GenericRepository<ForumTopic>, IForumTopicRepository
    {
        public async Task<List<ForumTopic>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.ForumTopics.AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

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

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<ForumTopic?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.ForumTopics
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ForumTopic?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ForumTopics.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(ForumTopic entity)
        {
            await _context.ForumTopics.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ForumTopic entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.ForumTopics.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.ForumTopics.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return;

            _context.ForumTopics.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}