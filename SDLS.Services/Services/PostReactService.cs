using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.PostReact;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class PostReactService : IPostReactService
    {
        private readonly IPostReactRepository _repository;
        private readonly IMapper _mapper;

        public PostReactService(IPostReactRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PostReactDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? forumPostId = null, string? reactType = null)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            if (forumPostId.HasValue)
                filtered = filtered.Where(x => x.ForumPostId == forumPostId.Value);

            if (!string.IsNullOrWhiteSpace(reactType))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.ReactType)
                    && x.ReactType.Contains(reactType.Trim(), StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<PostReactDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<PostReactDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? forumPostId = null, string? reactType = null, int page = 1, int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, forumPostId, reactType);
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
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<PostReactDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(PostReactCreateDTO dto)
        {
            if (dto.UserId == Guid.Empty || dto.ForumPostId == Guid.Empty)
                throw new ArgumentException("UserId và ForumPostId không được rỗng");

            var existing = await _repository.GetByUserAndForumPostAsync(dto.UserId, dto.ForumPostId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("PostReact cho UserId và ForumPostId này đã tồn tại.");

            var entity = new PostReact
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
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

            var isChangingKeys = existing.UserId != dto.UserId || existing.ForumPostId != dto.ForumPostId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndForumPostAsync(dto.UserId, dto.ForumPostId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và ForumPostId mới đã tồn tại ở record khác.");
            }

            existing.UserId = dto.UserId;
            existing.ForumPostId = dto.ForumPostId;
            existing.ReactType = dto.ReactType;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
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