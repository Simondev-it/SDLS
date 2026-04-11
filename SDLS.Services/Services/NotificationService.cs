using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Notification;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<NotificationDTO>> GetAllAsync(
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? sortBy = "time",
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var filtered = (await _repository.GetAllAsync(userId, title, content, status, role)).AsEnumerable();

            sortBy = (sortBy ?? "time").Trim().ToLowerInvariant();
            filtered = sortBy switch
            {
                "title_asc" => filtered.OrderBy(n => n.Title),
                "title_desc" => filtered.OrderByDescending(n => n.Title),
                _ => filtered.OrderByDescending(n => n.UpdateAt ?? n.CreateAt)
            };

            var total = filtered.Count();
            var pagedEntities = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<NotificationDTO>
            {
                Items = _mapper.Map<List<NotificationDTO>>(pagedEntities),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<NotificationDTO?> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<NotificationDTO>(entity);
        }

        public async Task<NotificationDTO> CreateAsync(NotificationCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = _mapper.Map<Notification>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Image = dto.Image;

            foreach (var userNotification in entity.UserNotifications)
            {
                userNotification.NotificationId = entity.Id;
                userNotification.CreateAt = now;
                userNotification.UpdateAt = now;
                userNotification.Status = 1;
            }

            await _repository.AddAsync(entity);
            return _mapper.Map<NotificationDTO>(entity);
        }

        public async Task<NotificationDTO> UpdateAsync(Guid id, NotificationUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy Notification");

            var now = DateTimeHelper.GetVietnamNow();

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Image = dto.Image;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = now;

            if (dto.UserNotifications != null)
            {
                var byId = existing.UserNotifications.ToDictionary(x => x.Id, x => x);

                foreach (var item in dto.UserNotifications)
                {
                    if (item.NotificationId != id)
                        throw ApiException.BadRequest($"UserNotification.NotificationId ({item.NotificationId}) không khớp Notification Id ({id}).");

                    if (item.Id.HasValue)
                    {
                        if (!byId.TryGetValue(item.Id.Value, out var un))
                            throw ApiException.NotFound($"Không tìm thấy UserNotification với Id {item.Id.Value}");

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

            await _repository.UpdateAsync(existing);
            return _mapper.Map<NotificationDTO>(existing);
        }

        public async Task<NotificationDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<NotificationDTO>(entity);
        }

        public async Task<NotificationDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteHardAsync(id);
            return _mapper.Map<NotificationDTO>(entity);
        }
    }
}