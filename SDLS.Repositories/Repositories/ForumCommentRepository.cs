using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Model.Helpers;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ForumCommentRepository : GenericRepository<ForumComment>, IForumCommentRepository
    {
        public async Task<IEnumerable<ForumComment>> GetAllAsync(
            Guid? id = null,
            Guid? forumPostId = null,
            Guid? userId = null,
            string? content = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<ForumComment> query = isPrivileged
                ? _context.ForumComments
                    .Include(x => x.User)
                    .Include(x => x.ForumPost)
                    .Include(x => x.CommentVotes)
                : _context.ForumComments
                    .Include(x => x.User)
                    .Include(x => x.ForumPost)
                    .Include(x => x.CommentVotes.Where(v => v.Status != 0));

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (forumPostId.HasValue)
                query = query.Where(x => x.ForumPostId == forumPostId.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(content))
            {
                var keyword = content.Trim();
                query = query.Where(x => x.Content != null && EF.Functions.ILike(x.Content, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    (x.User == null || x.User.Status != 0) &&
                    (x.ForumPost == null || x.ForumPost.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<ForumComment?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<ForumComment> query = isPrivileged
                ? _context.ForumComments
                    .Include(x => x.User)
                    .Include(x => x.ForumPost)
                    .Include(x => x.CommentVotes)
                : _context.ForumComments
                    .Include(x => x.User)
                    .Include(x => x.ForumPost)
                    .Include(x => x.CommentVotes.Where(v => v.Status != 0));

            query = query.Where(x => x.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    (x.User == null || x.User.Status != 0) &&
                    (x.ForumPost == null || x.ForumPost.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ForumComment?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ForumComments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(ForumComment entity)
        {
            await _context.ForumComments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ForumComment entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.ForumComments.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.ForumComments.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return;

            _context.ForumComments.Remove(existing);
            await _context.SaveChangesAsync();
        }

    }
}
