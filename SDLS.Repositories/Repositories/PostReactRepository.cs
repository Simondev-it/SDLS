using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class PostReactRepository : GenericRepository<PostReact>, IPostReactRepository
    {
        public async Task<List<PostReact>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumPostId = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<PostReact> query = isPrivileged
                ? _context.PostReacts
                    .Include(x => x.ForumPost)
                        .ThenInclude(fp => fp.PostImages)
                : _context.PostReacts
                    .Include(x => x.ForumPost)
                        .ThenInclude(fp => fp.PostImages.Where(pi => pi.Status != 0));

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (forumPostId.HasValue)
                query = query.Where(x => x.ForumPostId == forumPostId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
                query = query.Where(x => x.ForumPost == null || x.ForumPost.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<PostReact?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<PostReact> query = isPrivileged
                ? _context.PostReacts
                    .Include(x => x.ForumPost)
                        .ThenInclude(fp => fp.PostImages)
                : _context.PostReacts
                    .Include(x => x.ForumPost)
                        .ThenInclude(fp => fp.PostImages.Where(pi => pi.Status != 0));

            query = query.Where(x => x.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
                query = query.Where(x => x.ForumPost == null || x.ForumPost.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<List<PostReact>> GetByUserAndForumPostAsync(Guid? userId, Guid? forumPostId)
        {
            IQueryable<PostReact> query = _context.PostReacts.Include(x => x.ForumPost);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (forumPostId.HasValue)
                query = query.Where(x => x.ForumPostId == forumPostId.Value);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(PostReact entity)
        {
            await _context.PostReacts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PostReact entity)
        {
            _context.PostReacts.Attach(entity);
            _context.Entry(entity).Property(x => x.UserId).IsModified = true;
            _context.Entry(entity).Property(x => x.ForumPostId).IsModified = true;
            _context.Entry(entity).Property(x => x.ReactType).IsModified = true;
            _context.Entry(entity).Property(x => x.UpdateAt).IsModified = true;
            _context.Entry(entity).Property(x => x.Status).IsModified = true;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.PostReacts
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var entity = await _context.PostReacts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _context.PostReacts.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}