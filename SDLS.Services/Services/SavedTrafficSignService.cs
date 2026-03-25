using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedTrafficSign;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SavedTrafficSignService : ISavedTrafficSignService
    {
        private readonly ISavedTrafficSignRepository _repository;
        private readonly IMapper _mapper;

        public SavedTrafficSignService(ISavedTrafficSignRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SavedTrafficSignDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? trafficSignId = null)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            if (trafficSignId.HasValue)
                filtered = filtered.Where(x => x.TrafficSignId == trafficSignId.Value);

            return _mapper.Map<List<SavedTrafficSignDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<SavedTrafficSignDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? trafficSignId = null, int page = 1, int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, trafficSignId);
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
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<SavedTrafficSignDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(SavedTrafficSignCreateDTO dto)
        {
            if (dto.UserId == Guid.Empty || dto.TrafficSignId == Guid.Empty)
                throw new ArgumentException("UserId và TrafficSignId không được rỗng");

            var existing = await _repository.GetByUserAndTrafficSignAsync(dto.UserId, dto.TrafficSignId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("SavedTrafficSign cho UserId và TrafficSignId này đã tồn tại.");

            var entity = new SavedTrafficSign
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                TrafficSignId = dto.TrafficSignId,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, SavedTrafficSignUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            var isChangingKeys = existing.UserId != dto.UserId || existing.TrafficSignId != dto.TrafficSignId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndTrafficSignAsync(dto.UserId, dto.TrafficSignId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và TrafficSignId mới đã tồn tại ở record khác.");
            }

            existing.UserId = dto.UserId;
            existing.TrafficSignId = dto.TrafficSignId;
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