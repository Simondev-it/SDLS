using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumComment;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.Notification;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class ForumCommentService : IForumCommentService
    {
        private readonly IForumCommentRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly IForumPostRepository _forumPostRepository;
        private readonly IExecutionStrategyRepository _executionStrategy;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForumCommentService(
            IForumCommentRepository repository,
            INotificationService notificationService,
            IForumPostRepository forumPostRepository,
            IExecutionStrategyRepository executionStrategyService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _notificationService = notificationService;
            _forumPostRepository = forumPostRepository;
            _executionStrategy = executionStrategyService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<ForumCommentDTO>> GetAllAsync(
            Guid? id = null,
            Guid? forumPostId = null,
            Guid? userId = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var all = (await _repository.GetAllAsync(id, forumPostId, userId, content, status, role)).ToList();

            var ordered = all.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;
            var pageItems = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var dtos = _mapper.Map<List<ForumCommentDTO>>(pageItems);

            return new PagedResult<ForumCommentDTO>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ForumCommentDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var target = await _repository.GetByIdAsync(id, role);
            if (target == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<ForumCommentDTO>(target);
        }

        public async Task<ForumCommentDTO> CreateAsync(ForumCommentCreateDTO dto)
        {
            return await _executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _repository.BeginTransactionAsync();
                try
                {
                    var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
                    Guid recipientUserId;
                    string notificationTitle;
                    string notificationContent;

                    if (dto.ReplyId.HasValue)
                    {
                        if (dto.ReplyId.Value == Guid.Empty)
                            throw ApiException.BadRequest("ReplyId không hợp lệ.");

                        var forumPost = await _forumPostRepository.GetByIdAsync(dto.ForumPostId);
                        if (forumPost == null)
                            throw ApiException.NotFound("Không tìm thấy ForumPost.");

                        var parent = await _repository.GetByIdAsync(dto.ReplyId.Value);
                        if (parent == null)
                            throw ApiException.NotFound("Không tìm thấy comment cha.");

                        if (parent.ForumPostId != dto.ForumPostId)
                            throw ApiException.BadRequest("Reply phải cùng ForumPostId với comment cha.");

                        recipientUserId = parent.UserId;
                        notificationTitle = "Trả lời bình luận";
                        notificationContent = "Có người đã trả lời bình luận '" + parent.Content + "' của bạn trong bài đăng '" + forumPost.Title + "'";
                    }
                    else
                    {
                        var forumPost = await _forumPostRepository.GetByIdAsync(dto.ForumPostId);
                        if (forumPost == null)
                            throw ApiException.NotFound("Không tìm thấy ForumPost.");

                        recipientUserId = forumPost.UserId;
                        notificationTitle = "Bình luận bài viết";
                        notificationContent = "Có người đã bình luận vào bài đăng '" + forumPost.Title + "' của bạn";
                    }

                    var now = DateTimeHelper.GetVietnamNow();
                    var entity = new ForumComment
                    {
                        Id = Guid.NewGuid(),
                        ReplyId = dto.ReplyId,
                        ForumPostId = dto.ForumPostId,
                        UserId = currentUserId,
                        Content = dto.Content.Trim(),
                        CreateAt = now,
                        UpdateAt = now,
                        Status = 1
                    };

                    await _repository.AddAsync(entity);

                    var notificationDto = new NotificationCreateDTO
                    {
                        Title = notificationTitle,
                        Content = notificationContent,
                        Status = 2,
                        UserNotifications = new List<UserNotificationCreateDTO>
                        {
                            new UserNotificationCreateDTO
                            {
                                UserId = recipientUserId
                            }
                        }
                    };

                    await _notificationService.CreateAsync(notificationDto);

                    await transaction.CommitAsync();
                    return _mapper.Map<ForumCommentDTO>(entity);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<ForumCommentDTO> UpdateAsync(Guid id, ForumCommentUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy ForumComment.");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.ReplyId.HasValue)
            {
                if (dto.ReplyId.Value == Guid.Empty || dto.ReplyId.Value == id)
                    throw ApiException.BadRequest("ReplyId không hợp lệ.");

                var parent = await _repository.GetByIdAsync(dto.ReplyId.Value);
                if (parent == null)
                    throw ApiException.NotFound("Không tìm thấy comment cha.");

                if (parent.ForumPostId != dto.ForumPostId)
                    throw ApiException.BadRequest("Reply phải cùng ForumPostId với comment cha.");
            }

            existing.ReplyId = dto.ReplyId;
            existing.ForumPostId = dto.ForumPostId;
            existing.UserId = currentUserId;
            existing.Content = dto.Content.Trim();
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ForumCommentDTO>(existing);
        }

        public async Task<ForumCommentDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var target = await _repository.GetByIdAsync(id, role);
            if (target == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            target.Status = 0;
            target.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<ForumCommentDTO>(target);
        }

        public async Task<ForumCommentDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var target = await _repository.GetByIdAsync(id, role);
            if (target == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<ForumCommentDTO>(target);
            await _repository.DeleteHardAsync(id);
            return result;
        }

    }
}
