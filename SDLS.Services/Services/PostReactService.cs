using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.PostReact;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class PostReactService : IPostReactService
    {
        private readonly IPostReactRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostReactService(
            IPostReactRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<PostReactDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumPostId = null,
            string? reactType = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filtered = (await _repository.GetAllAsync(id, userId, forumPostId, status, role)).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(reactType))
            {
                var keyword = reactType.Trim();
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.ReactType)
                    && x.ReactType.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            return _mapper.Map<List<PostReactDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<PostReactDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumPostId = null,
            string? reactType = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, forumPostId, reactType, status);
            var total = items.Count;

            return new PagedResult<PostReactDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<PostReactDTO?> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            return entity != null ? _mapper.Map<PostReactDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(PostReactCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.ForumPostId == Guid.Empty)
                throw new ArgumentException("ForumPostId không được rỗng");

            var existing = await _repository.GetByUserAndForumPostAsync(currentUserId, dto.ForumPostId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("PostReact cho UserId và ForumPostId này đã tồn tại.");

            var entity = new PostReact
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                ForumPostId = dto.ForumPostId,
                ReactType = dto.ReactType,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, PostReactUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var isChangingKeys = existing.UserId != currentUserId || existing.ForumPostId != dto.ForumPostId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndForumPostAsync(currentUserId, dto.ForumPostId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và ForumPostId mới đã tồn tại ở record khác.");
            }

            existing.UserId = currentUserId;
            existing.ForumPostId = dto.ForumPostId;
            existing.ReactType = dto.ReactType;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            await _repository.DeleteSoftAsync(id);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _repository.DeleteHardAsync(id);
            return true;
        }
    }
}