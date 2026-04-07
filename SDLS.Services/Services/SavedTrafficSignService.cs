using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedTrafficSign;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SavedTrafficSignService : ISavedTrafficSignService
    {
        private readonly ISavedTrafficSignRepository _repository;
        private readonly ITrafficSignRepository _trafficSignRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SavedTrafficSignService(
            ISavedTrafficSignRepository repository,
            ITrafficSignRepository trafficSignRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _trafficSignRepository = trafficSignRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<SavedTrafficSignDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? trafficSignId = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, userId, trafficSignId, status, role);
            return _mapper.Map<List<SavedTrafficSignDTO>>(entities);
        }

        public async Task<PagedResult<SavedTrafficSignDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? trafficSignId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, trafficSignId, status);
            var total = items.Count;

            return new PagedResult<SavedTrafficSignDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SavedTrafficSignDTO?> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<SavedTrafficSignDTO>(entity);
        }

        public async Task<SavedTrafficSignDTO> CreateAsync(SavedTrafficSignCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.TrafficSignId == Guid.Empty)
                throw ApiException.BadRequest("TrafficSignId không được rỗng");

            var trafficSign = await _trafficSignRepository.GetByIdAsync(dto.TrafficSignId);
            if (trafficSign == null)
                throw ApiException.BadRequest("TrafficSignId không tồn tại");

            var existing = await _repository.GetByUserAndTrafficSignAsync(currentUserId, dto.TrafficSignId);
            if (existing != null && existing.Any())
                throw ApiException.Conflict("SavedTrafficSign cho UserId và TrafficSignId này đã tồn tại.");

            var entity = new SavedTrafficSign
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                TrafficSignId = dto.TrafficSignId,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<SavedTrafficSignDTO>(entity);
        }

        public async Task<SavedTrafficSignDTO> UpdateAsync(Guid id, SavedTrafficSignUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy SavedTrafficSign");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var isChangingKeys = existing.UserId != currentUserId || existing.TrafficSignId != dto.TrafficSignId;

            var trafficSign = await _trafficSignRepository.GetByIdAsync(dto.TrafficSignId);
            if (trafficSign == null)
                throw ApiException.BadRequest("TrafficSignId không tồn tại");

            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndTrafficSignAsync(currentUserId, dto.TrafficSignId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw ApiException.Conflict("Cặp UserId và TrafficSignId mới đã tồn tại ở record khác.");
            }

            existing.UserId = currentUserId;
            existing.TrafficSignId = dto.TrafficSignId;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<SavedTrafficSignDTO>(existing);
        }

        public async Task<SavedTrafficSignDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTime.UtcNow.ToLocalTime();
            return _mapper.Map<SavedTrafficSignDTO>(entity);
        }

        public async Task<SavedTrafficSignDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SavedTrafficSignDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}