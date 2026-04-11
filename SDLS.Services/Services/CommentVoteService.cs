using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.CommentVote;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class CommentVoteService : ICommentVoteService
    {
        private readonly ICommentVoteRepository _repository;
        private readonly IForumCommentRepository _forumCommentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CommentVoteService(
            ICommentVoteRepository repository,
            IForumCommentRepository forumCommentRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _forumCommentRepository = forumCommentRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<CommentVoteDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumCommentId = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, userId, forumCommentId, status, role);
            return _mapper.Map<List<CommentVoteDTO>>(entities);
        }

        public async Task<PagedResult<CommentVoteDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumCommentId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, forumCommentId, status);
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
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);

            if (entity == null)
            throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<CommentVoteDTO>(entity);
        }

        public async Task<CommentVoteDTO> CreateAsync(CommentVoteCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.ForumCommentId == Guid.Empty)
                throw ApiException.BadRequest("ForumCommentId không được rỗng");

            var comment = await _forumCommentRepository.GetByIdAsync(dto.ForumCommentId);

            if (comment == null)
                throw ApiException.NotFound("ForumComment không tồn tại");

            var existing = await _repository.GetByUserAndForumCommentAsync(currentUserId, dto.ForumCommentId);
            if (existing != null && existing.Any())
                throw ApiException.Conflict("CommentVote cho User và ForumComment này đã tồn tại.");

            var entity = new CommentVote
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                ForumCommentId = dto.ForumCommentId,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<CommentVoteDTO>(entity);
        }

        public async Task<CommentVoteDTO> UpdateAsync(Guid id, CommentVoteUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id, null);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy CommentVote");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var comment = await _forumCommentRepository.GetByIdAsync(dto.ForumCommentId);

            if (comment == null)
                throw ApiException.NotFound("ForumComment không tồn tại");

            var isChangingKeys = existing.UserId != currentUserId || existing.ForumCommentId != dto.ForumCommentId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndForumCommentAsync(currentUserId, dto.ForumCommentId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw ApiException.Conflict("Cặp UserId và ForumCommentId mới đã tồn tại ở record khác.");
            }

            existing.UserId = currentUserId;
            existing.ForumCommentId = dto.ForumCommentId;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<CommentVoteDTO>(existing);
        }

        public async Task<CommentVoteDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);

            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTime.UtcNow.ToLocalTime();
            return _mapper.Map<CommentVoteDTO>(entity);
        }

        public async Task<CommentVoteDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);

            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<CommentVoteDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}