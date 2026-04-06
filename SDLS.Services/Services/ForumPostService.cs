using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
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
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForumPostService(IForumPostRepository repository, IForumTopicRepository forumTopicRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _forumTopicRepository = forumTopicRepository;
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

            var filtered = await _repository.GetAllAsync(
                id, forumTopicId, userId, name, title, content, status, role);

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

            var forumPost = await _repository.GetByIdAsync(id, role);
            if (forumPost == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var dto = _mapper.Map<ForumPostDTO>(forumPost);
            var images = await _repository.GetPostImagesByPostIdsAsync(new List<Guid> { id }, role);
            dto.PostImages = _mapper.Map<List<ForumPostImageDTO>>(images);

            return dto;
        }

        public async Task<bool> CreateAsync(ForumPostCreateDTO dto)
        {
            return await CreateInternalAsync(dto, -1);
        }

        public async Task<bool> CreateByInstructorAsync(ForumPostCreateDTO dto)
        {
            return await CreateInternalAsync(dto, 1);
        }

        private async Task<bool> CreateInternalAsync(ForumPostCreateDTO dto, int status)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();

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

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, ForumPostUpdateDTO dto)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();
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

            if (dto.Status.HasValue && forumPost.Status != dto.Status.Value)
            {
                forumPost.Status = dto.Status.Value;
                changed = true;
            }

            if (!changed)
                return true;

            forumPost.UpdateAt = now;
            await _repository.UpdateAsync(forumPost);

            return true;
        }

        public async Task<bool> ApproveAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            forumPost.Status = 1;
            forumPost.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _repository.UpdateAsync(forumPost);

            return true;
        }

        public async Task<bool> DisapproveAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound("Khong tim thay ForumPost");

            forumPost.Status = 3;
            forumPost.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _repository.UpdateAsync(forumPost);

            return true;
        }


        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            var forumPost = await _repository.GetByIdForUpdateAsync(id);
            if (forumPost == null)
                throw ApiException.NotFound($"Khong tim thay ForumPost voi Id {id}");

            var now = DateTime.UtcNow.ToLocalTime();
            forumPost.Status = 0;
            forumPost.UpdateAt = now;

            await _repository.SoftDeletePostImagesAsync(id, now);
            await _repository.UpdateAsync(forumPost);

            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var forumPost = await _repository.GetByIdAsync(id, role);
            if (forumPost == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteHardAsync(id);
            return true;
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
