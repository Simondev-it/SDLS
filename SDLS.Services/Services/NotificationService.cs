using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Notification;
using SDLS.Model.Enumerations;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository repository,
            IStorageService storageService,
            IMapper mapper)
        {
            _repository = repository;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<PagedResult<NotificationDTO>> GetAllAsync(
            Guid? userId = null,
            string? title = null,
            string? content = null,
            string? sortBy = "time",
            int page = 1,
            int pageSize = 20)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (userId.HasValue)
            {
                filtered = filtered.Where(n =>
                    n.UserNotifications != null &&
                    n.UserNotifications.Any(un => un.Status == 1 && un.UserId == userId.Value));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                var keyword = title.Trim();
                filtered = filtered.Where(n =>
                    !string.IsNullOrWhiteSpace(n.Title) &&
                    n.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var keyword = content.Trim();
                filtered = filtered.Where(n =>
                    !string.IsNullOrWhiteSpace(n.Content) &&
                    n.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            sortBy = (sortBy ?? "time").Trim().ToLowerInvariant();
            filtered = sortBy switch
            {
                "title_asc" => filtered.OrderBy(n => n.Title),
                "title_desc" => filtered.OrderByDescending(n => n.Title),
                _ => filtered.OrderByDescending(n => n.UpdateAt ?? n.CreateAt)
            };

            var total = filtered.Count();

            var pagedEntities = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = _mapper.Map<List<NotificationDTO>>(pagedEntities);

            return new PagedResult<NotificationDTO>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<NotificationDTO?> GetByIdAsync(Guid id)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                return null;

            existing.Status = 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _repository.UpdateAsync(existing);

            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<NotificationDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(NotificationCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<Notification>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = now;
            entity.UpdateAt = now;

            foreach (var userNotification in entity.UserNotifications)
            {
                userNotification.NotificationId = entity.Id;
                userNotification.CreateAt = now;
                userNotification.UpdateAt = now;
                userNotification.Status = 1;
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                entity.Image = await _storageService.UploadImageAsync(dto.ImageFile, ImageTarget.NotificationImage, entity.Id);
            }

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, NotificationUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null) return false;

            var now = DateTime.UtcNow.ToLocalTime();

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = now;

            if (dto.UserNotifications != null)
            {
                var byId = existing.UserNotifications.ToDictionary(x => x.Id, x => x);

                foreach (var item in dto.UserNotifications)
                {
                    if (item.NotificationId != id)
                        throw new ArgumentException($"UserNotification.NotificationId ({item.NotificationId}) không khớp Notification Id ({id}).");

                    if (item.Id.HasValue)
                    {
                        if (!byId.TryGetValue(item.Id.Value, out var un))
                            throw new KeyNotFoundException($"Không tìm thấy UserNotification với Id {item.Id.Value}");

                        un.UserId = item.UserId;
                        un.Status = item.Status ?? un.Status ?? 1;
                        un.UpdateAt = now;
                    }
                    else
                    {
                        existing.UserNotifications.Add(new UserNotification
                        {
                            NotificationId = id,
                            UserId = item.UserId,
                            CreateAt = now,
                            UpdateAt = now,
                            Status = item.Status ?? 1
                        });
                    }
                }
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                existing.Image = await _storageService.UploadImageAsync(dto.ImageFile, ImageTarget.NotificationImage, id);
            }

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