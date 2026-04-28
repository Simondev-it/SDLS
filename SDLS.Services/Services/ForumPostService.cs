using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.Constants;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.Notification;
using SDLS.Model.DTOs.ForumPost;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using SDLS.Services.Utilities;
using ForumPostModel = SDLS.Model.Models.ForumPost;

namespace SDLS.Services.Services
{
    public class ForumPostService : IForumPostService
    {
        private readonly IForumPostRepository _repository;
        private readonly IForumTopicRepository _forumTopicRepository;
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForumPostService(
            IForumPostRepository repository,
            IForumTopicRepository forumTopicRepository,
            INotificationService notificationService,
            AppDbContext dbContext,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _forumTopicRepository = forumTopicRepository;
            _notificationService = notificationService;
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<ForumPostDTO>> GetAllAsync(
            Guid? id = null,
            Guid? forumTopicId = null,
            Guid? userId = null,
            string? name = null,
            string? title = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var currentUserId = UserContextHelper.GetCurrentUserId(_httpContextAccessor);
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            var filtered = await _repository.GetAllAsync(
                id, forumTopicId, userId, name, title, content, status, role);

            filtered = filtered.Where(x =>
                x.Status != 0 ||
                isPrivileged ||
                (currentUserId.HasValue && x.UserId == currentUserId.Value));

            var ordered = filtered.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var pageEntities = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var postIds = pageEntities.Select(x => x.Id).ToList();
            var postImages = await _repository.GetPostImagesByPostIdsAsync(postIds, role);

            var imageLookup = postImages.GroupBy(x => x.ForumPostId).ToDictionary(g => g.Key, g => g.ToList());

            var dtos = _mapper.Map<List<ForumPostDTO>>(pageEntities);
            foreach (var dto in dtos)
            {
                if (imageLookup.TryGetValue(dto.Id, out var images))
                    dto.PostImages = _mapper.Map<List<ForumPostImageDTO>>(images);
            }

            return new PagedResult<ForumPostDTO>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ForumPostDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var currentUserId = UserContextHelper.GetCurrentUserId(_httpContextAccessor);
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            var forumPost = await _repository.GetByIdAsync(id, role);
            if (forumPost == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var canView = forumPost.Status != 0 &&
                (
                    forumPost.Status == 1 ||
                    ((forumPost.Status == 4 || forumPost.Status == 5) && currentUserId.HasValue && forumPost.UserId == currentUserId.Value) ||
                    ((forumPost.Status == -1 || forumPost.Status == 2 || forumPost.Status == 3) &&
                        (isPrivileged || (currentUserId.HasValue && forumPost.UserId == currentUserId.Value)))
                );

            if (!canView)
                throw ApiException.NotFound($"Not found with ID {id}");

            var dto = _mapper.Map<ForumPostDTO>(forumPost);
            var images = await _repository.GetPostImagesByPostIdsAsync(new List<Guid> { id }, role);
            dto.PostImages = _mapper.Map<List<ForumPostImageDTO>>(images);

            return dto;
        }

        public async Task<ForumPostDTO> CreateAsync(ForumPostCreateDTO dto)
        {
            return await CreateInternalAsync(dto, -1);
        }

        public async Task<ForumPostDTO> CreateByInstructorAsync(ForumPostCreateDTO dto)
        {
            return await CreateInternalAsync(dto, 4);
        }

        private async Task<ForumPostDTO> CreateInternalAsync(ForumPostCreateDTO dto, int status)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTimeHelper.GetVietnamNow();

                if (dto.ForumTopicId == Guid.Empty)
                    throw ApiException.BadRequest("ForumTopicId khong hop le.");

                var forumTopic = await _forumTopicRepository.GetByIdAsync(dto.ForumTopicId);
                if (forumTopic == null)
                    throw ApiException.BadRequest("Khong tim thay ForumTopic voi ForumTopicId da cho.");

            var forumPost = new ForumPostModel
            {
                Id = Guid.NewGuid(),
                ForumTopicId = dto.ForumTopicId,
                UserId = currentUserId,
                Name = dto.Name,
                Title = dto.Title,
                Content = dto.Content,
                ViewCount = 0,
                CreateAt = now,
                UpdateAt = now,
                Status = status
            };

            await _repository.AddAsync(forumPost);
            await SyncPostImagesFromContentAsync(forumPost.Id, dto.Content, now);
            await _repository.UpdateAsync(forumPost);

            if (status == -1)
            {
                var instructorUserIds = await _dbContext.Users
                    .AsNoTracking()
                    .Where(x => x.RoleId == RoleConst.INSTRUCTOR_ROLE_ID && x.Status != 0)
                    .Select(x => x.Id)
                    .Distinct()
                    .ToListAsync();

                if (instructorUserIds.Any())
                {
                    var notification = new NotificationCreateDTO
                    {
                        Title = "Bài viết mới",
                        Content = $"Có bài viết mới cần duyệt: '{forumPost.Title}'.",
                        Status = 2,
                        UserNotifications = instructorUserIds
                            .Select(userId => new UserNotificationCreateDTO { UserId = userId })
                            .ToList()
                    };

                    await _notificationService.CreateAsync(notification);
                }
            }

            return _mapper.Map<ForumPostDTO>(forumPost);
        }

        public async Task<ForumPostDTO> UpdateAsync(Guid id, ForumPostUpdateDTO dto)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTimeHelper.GetVietnamNow();
            var changed = false;

            if (dto.ForumTopicId.HasValue)
            {
                if (dto.ForumTopicId.Value == Guid.Empty)
                    throw ApiException.BadRequest("ForumTopicId khong hop le.");

                var forumTopic = await _forumTopicRepository.GetByIdAsync(dto.ForumTopicId.Value);
                if (forumTopic == null)
                    throw ApiException.BadRequest("Khong tim thay ForumTopic voi ForumTopicId da cho.");

                if (forumPost.ForumTopicId != dto.ForumTopicId.Value)
                {
                    forumPost.ForumTopicId = dto.ForumTopicId.Value;
                    changed = true;
                }
            }

            if (forumPost.UserId != currentUserId)
            {
                forumPost.UserId = currentUserId;
                changed = true;
            }

            if (dto.Name != null && !string.Equals(forumPost.Name, dto.Name, StringComparison.Ordinal))
            {
                forumPost.Name = dto.Name;
                changed = true;
            }

            if (dto.Title != null)
            {
                var newTitle = dto.Title.Trim();
                if (string.IsNullOrWhiteSpace(newTitle))
                    throw ApiException.BadRequest("Title khong duoc de trong.");

                if (!string.Equals(forumPost.Title, newTitle, StringComparison.Ordinal))
                {
                    forumPost.Title = newTitle;
                    changed = true;
                }
            }

            if (dto.Content != null)
            {
                var newContent = dto.Content.Trim();
                if (string.IsNullOrWhiteSpace(newContent))
                    throw ApiException.BadRequest("Content khong duoc de trong.");

                if (!string.Equals(forumPost.Content, newContent, StringComparison.Ordinal))
                {
                    forumPost.Content = newContent;
                    await SyncPostImagesFromContentAsync(id, newContent, now);
                    changed = true;
                }
            }

            if (dto.ViewCount.HasValue && forumPost.ViewCount != dto.ViewCount.Value)
            {
                forumPost.ViewCount = dto.ViewCount.Value;
                changed = true;
            }

                forumPost.Status = dto.Status.Value;
                changed = true;
            

            if (!changed)
                return _mapper.Map<ForumPostDTO>(forumPost);

            forumPost.UpdateAt = now;
            await _repository.UpdateAsync(forumPost);

            return _mapper.Map<ForumPostDTO>(forumPost);
        }

        public async Task<ForumPostDTO> TogglePinStatusAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            if (forumPost.Status == 1)
            {
                forumPost.Status = 5;
            }
            else if (forumPost.Status == 5)
            {
                forumPost.Status = 1;
            }
            else
            {
                throw ApiException.BadRequest("Chỉ cho phép ghim/bỏ ghim khi trạng thái là 1 hoặc 5.");
            }

            forumPost.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _repository.UpdateAsync(forumPost);

            return _mapper.Map<ForumPostDTO>(forumPost);
        }

        public async Task<ForumPostDTO> ApproveAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            forumPost.Status = 1;
            forumPost.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _repository.UpdateAsync(forumPost);

            return _mapper.Map<ForumPostDTO>(forumPost);
        }

        public async Task<ForumPostDTO> ToggleStatusAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            if (forumPost.Status == 1 || forumPost.Status == 5)
            {
                forumPost.Status = 4;
            }
            else if (forumPost.Status == 4)
            {
                forumPost.Status = 1;
            }
            else
            {
                throw ApiException.BadRequest("Ch? cho phép chuy?n tr?ng thái gi?a 1 và 4.");
            }

            forumPost.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _repository.UpdateAsync(forumPost);

            return _mapper.Map<ForumPostDTO>(forumPost);
        }

        public async Task<ForumPostDTO> DisapproveAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            forumPost.Status = 3;
            forumPost.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _repository.UpdateAsync(forumPost);

            return _mapper.Map<ForumPostDTO>(forumPost);
        }


        public async Task<ForumPostDTO> DeleteSoftAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound($"Khong tim thay ForumPost voi Id {id}");

            var now = DateTimeHelper.GetVietnamNow();
            forumPost.Status = 0;
            forumPost.UpdateAt = now;

            await _repository.SoftDeletePostImagesAsync(id, now);
            await _repository.UpdateAsync(forumPost);

            return _mapper.Map<ForumPostDTO>(forumPost);
        }

        public async Task<ForumPostDTO> ForceDeleteAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound($"Khong tim thay ForumPost voi Id {id}");

            forumPost.Status = 2;
            forumPost.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(forumPost);
            return _mapper.Map<ForumPostDTO>(forumPost);
        }

        public async Task<ForumPostDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var forumPost = await _repository.GetByIdAsync(id, role);
            if (forumPost == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<ForumPostDTO>(forumPost);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private async Task SyncPostImagesFromContentAsync(Guid postId, string? content, DateTime now)
        {
            var newUrls = HtmlContentParser.ExtractImageUrls(content);
            var activeImages = await _repository.GetPostImagesByPostIdForUpdateAsync(postId);

            var activeUrlSet = activeImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .Select(x => x.Url!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var imagesToRemove = activeImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Url)
                    && !newUrls.Contains(x.Url!, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (imagesToRemove.Any())
            {
                _repository.RemovePostImages(imagesToRemove);
            }

            var imagesToAdd = new List<PostImage>();
            foreach (var url in newUrls)
            {
                if (activeUrlSet.Contains(url))
                    continue;

                imagesToAdd.Add(new PostImage
                {
                    Id = Guid.NewGuid(),
                    ForumPostId = postId,
                    Name = HtmlContentParser.ResolveImageNameFromUrl(url),
                    Url = url,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = 1
                });
            }

            if (imagesToAdd.Any())
            {
                _repository.AddPostImages(imagesToAdd);
            }
        }

    }
}
