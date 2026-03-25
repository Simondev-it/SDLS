using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.CommentVote;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class CommentVoteService : ICommentVoteService
    {
        private readonly ICommentVoteRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CommentVoteService(
            ICommentVoteRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
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
            return entity != null ? _mapper.Map<CommentVoteDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(CommentVoteCreateDTO dto) { /* giữ nguyên */ throw new NotImplementedException(); }
        public async Task<bool> UpdateAsync(Guid id, CommentVoteUpdateDTO dto) { /* giữ nguyên */ throw new NotImplementedException(); }
        public async Task<bool> DeleteSoftAsync(Guid id) { await _repository.DeleteSoftAsync(id); return true; }
        public async Task<bool> DeleteHardAsync(Guid id) { await _repository.DeleteHardAsync(id); return true; }
    }
}