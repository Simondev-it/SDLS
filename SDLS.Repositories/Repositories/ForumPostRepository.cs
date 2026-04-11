using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using SDLS.Repositories.Helper;

namespace SDLS.Repositories.Repositories
{
    public class ForumPostRepository : GenericRepository<ForumPost>, IForumPostRepository
    {
        public async Task<IEnumerable<ForumPost>> GetAllAsync(
            Guid? id = null,
            Guid? forumTopicId = null,
            Guid? userId = null,
            string? name = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<ForumPost> query = isPrivileged
                ? _context.ForumPosts
                    .Include(x => x.User)
                        .ThenInclude(x => x.Role)
                    .Include(x => x.ForumComments)
                : _context.ForumPosts
                    .Include(x => x.User)
                        .ThenInclude(x => x.Role)
                    .Include(x => x.ForumComments.Where(c => c.Status != 0));

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (forumTopicId.HasValue)
                query = query.Where(x => x.ForumTopicId == forumTopicId.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                var keyword = title.Trim();
                query = query.Where(x => x.Title != null && EF.Functions.ILike(x.Title, $"%{keyword}%"));
            }

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
                query = query.Where(x => x.User == null || x.User.Status != 0);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<ForumPost?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<ForumPost> query = isPrivileged
                ? _context.ForumPosts
                    .Include(x => x.User)
                        .ThenInclude(x => x.Role)
                    .Include(x => x.ForumComments)
                : _context.ForumPosts
                    .Include(x => x.User)
                        .ThenInclude(x => x.Role)
                    .Include(x => x.ForumComments.Where(c => c.Status != 0));

            query = query.Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x => x.User == null || x.User.Status != 0);
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ForumPost?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ForumPosts
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(ForumPost forumPost)
        {
            await _context.ForumPosts.AddAsync(forumPost);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ForumPost forumPost)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var forumPost = await _context.ForumPosts
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (forumPost == null)
                return;

            forumPost.Status = 0;
            forumPost.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var forumPost = await _context.ForumPosts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (forumPost == null)
                return;

            _context.ForumPosts.Remove(forumPost);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PostImage>> GetPostImagesByPostIdsAsync(List<Guid> postIds, string? role = null)
        {
            if (postIds == null || postIds.Count == 0)
                return new List<PostImage>();

            var query = _context.PostImages.Where(x => postIds.Contains(x.ForumPostId));

            query = query.ApplyRoleFilter(role);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<PostImage>> GetPostImagesByPostIdForUpdateAsync(Guid postId)
        {
            return await _context.PostImages
                .Where(x => x.ForumPostId == postId)
                .ToListAsync();
        }

        public void AddPostImages(IEnumerable<PostImage> images)
        {
            _context.PostImages.AddRange(images);
        }

        public void RemovePostImages(IEnumerable<PostImage> images)
        {
            _context.PostImages.RemoveRange(images);
        }

        public async Task SoftDeletePostImagesAsync(Guid postId, DateTime now)
        {
            await _context.PostImages
                .Where(x => x.ForumPostId == postId && x.Status == 1)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, 0)
                    .SetProperty(x => x.UpdateAt, now));
        }
    }
}
