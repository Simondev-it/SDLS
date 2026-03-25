using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.CommentVote;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class CommentVoteService : ICommentVoteService
    {
        private readonly ICommentVoteRepository _repository;
        private readonly IMapper _mapper;

        public CommentVoteService(ICommentVoteRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CommentVoteDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? forumCommentId = null)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            if (forumCommentId.HasValue)
                filtered = filtered.Where(x => x.ForumCommentId == forumCommentId.Value);

            return _mapper.Map<List<CommentVoteDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<CommentVoteDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? forumCommentId = null, int page = 1, int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, forumCommentId);
            var total = items.Count;

            return new PagedResult<CommentVoteDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<CommentVoteDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<CommentVoteDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(CommentVoteCreateDTO dto)
        {
            if (dto.UserId == Guid.Empty || dto.ForumCommentId == Guid.Empty)
                throw new ArgumentException("UserId và ForumCommentId không được rỗng");

            var existing = await _repository.GetByUserAndForumCommentAsync(dto.UserId, dto.ForumCommentId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("CommentVote cho UserId và ForumCommentId này đã tồn tại.");

            var entity = new CommentVote
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ForumCommentId = dto.ForumCommentId,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, CommentVoteUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            var isChangingKeys = existing.UserId != dto.UserId || existing.ForumCommentId != dto.ForumCommentId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndForumCommentAsync(dto.UserId, dto.ForumCommentId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và ForumCommentId mới đã tồn tại ở record khác.");
            }

            existing.UserId = dto.UserId;
            existing.ForumCommentId = dto.ForumCommentId;
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