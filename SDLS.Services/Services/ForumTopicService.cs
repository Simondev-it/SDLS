using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumTopic;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class ForumTopicService : IForumTopicService
    {
        private readonly IForumTopicRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ForumTopicService(
            IForumTopicRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<ForumTopicDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var all = await _repository.GetAllAsync(id, name, description, status, role);
            return _mapper.Map<List<ForumTopicDTO>>(all);
        }

        public async Task<PagedResult<ForumTopicDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, description, status);
            var total = items.Count;

            return new PagedResult<ForumTopicDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ForumTopicDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<ForumTopicDTO>(entity);
        }

        public async Task<ForumTopicDTO> CreateAsync(ForumTopicCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = new ForumTopic
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<ForumTopicDTO>(entity);
        }

        public async Task<ForumTopicDTO> UpdateAsync(Guid id, ForumTopicUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy ForumTopic");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ForumTopicDTO>(existing);
        }

        public async Task<ForumTopicDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<ForumTopicDTO>(existing);
        }

        public async Task<ForumTopicDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<ForumTopicDTO>(existing);
            await _repository.DeleteHardAsync(id);
            return result;
        }

    }
}