using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumComment;
using SDLS.Model.DTOs.Notification;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
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

        public ForumCommentService(
            IForumCommentRepository repository,
            INotificationService notificationService,
            IForumPostRepository forumPostRepository,
            IExecutionStrategyRepository executionStrategyService,
            IMapper mapper)
        {
            _repository = repository;
            _notificationService = notificationService;
            _forumPostRepository = forumPostRepository;
            _executionStrategy = executionStrategyService;
            _mapper = mapper;
        }

        public async Task<PagedResult<ForumCommentDTO>> GetAllAsync(
            Guid? id = null,
            Guid? forumPostId = null,
            Guid? userId = null,
            string? content = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var all = (await _repository.GetAllAsync()).ToList();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (forumPostId.HasValue)
                filtered = filtered.Where(x => x.ForumPostId == forumPostId.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(content))
            {
                var keyword = content.Trim();
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Content)
                    && x.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            var ordered = filtered
                .OrderBy(x => x.CreateAt)
                .ThenBy(x => x.Id)
                .ToList();

            var replyLookup = ordered
                .Where(x => x.ReplyId.HasValue)
                .GroupBy(x => x.ReplyId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.CreateAt).ThenBy(x => x.Id).ToList());

            var roots = ordered
                .Where(x => x.ReplyId == null)
                .ToList();

            var total = roots.Count;
            var pageRoots = roots
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = pageRoots
                .Select(x => BuildCommentTree(x, replyLookup))
                .ToList();

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
            var all = (await _repository.GetAllAsync()).ToList();
            var target = all.FirstOrDefault(x => x.Id == id);

            if (target == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            var replyLookup = all
                .Where(x => x.ReplyId.HasValue)
                .GroupBy(x => x.ReplyId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.CreateAt).ThenBy(x => x.Id).ToList());

            return BuildCommentTree(target, replyLookup);
        }

        public async Task<bool> CreateAsync(ForumCommentCreateDTO dto)
        {
            return await _executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _repository.BeginTransactionAsync();
                try
                {
                    Guid recipientUserId;
                    string notificationTitle;
                    string notificationContent;

                    if (dto.ReplyId.HasValue)
                    {
                        if (dto.ReplyId.Value == Guid.Empty)
                            throw new ArgumentException("ReplyId không hợp lệ.");

                        var forumPost = await _forumPostRepository.GetByIdAsync(dto.ForumPostId);
                        if (forumPost == null)
                            throw new KeyNotFoundException("Không tìm thấy ForumPost.");

                        var parent = await _repository.GetByIdAsync(dto.ReplyId.Value);
                        if (parent == null)
                            throw new KeyNotFoundException("Không tìm thấy comment cha.");

                        if (parent.ForumPostId != dto.ForumPostId)
                            throw new ArgumentException("Reply phải cùng ForumPostId với comment cha.");

                        recipientUserId = parent.UserId;
                        notificationTitle = "Trả lời bình luận";
                        notificationContent = "Có người đã trả lời bình luận '" + parent.Content + "' của bạn trong bài đăng '" + forumPost.Title + "'";
                    }
                    else
                    {
                        var forumPost = await _forumPostRepository.GetByIdAsync(dto.ForumPostId);
                        if (forumPost == null)
                            throw new KeyNotFoundException("Không tìm thấy ForumPost.");

                        recipientUserId = forumPost.UserId;
                        notificationTitle = "Bình luận bài viết";
                        notificationContent = "Có người đã bình luận vào bài đăng '" + forumPost.Title + "' của bạn";
                    }

                    var now = DateTime.UtcNow.ToLocalTime();
                    var entity = new ForumComment
                    {
                        Id = Guid.NewGuid(),
                        ReplyId = dto.ReplyId,
                        ForumPostId = dto.ForumPostId,
                        UserId = dto.UserId,
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
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<bool> UpdateAsync(Guid id, ForumCommentUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy ForumComment.");

            if (dto.ReplyId.HasValue)
            {
                if (dto.ReplyId.Value == Guid.Empty || dto.ReplyId.Value == id)
                    throw new ArgumentException("ReplyId không hợp lệ.");

                var parent = await _repository.GetByIdAsync(dto.ReplyId.Value);
                if (parent == null)
                    throw new KeyNotFoundException("Không tìm thấy comment cha.");

                if (parent.ForumPostId != dto.ForumPostId)
                    throw new ArgumentException("Reply phải cùng ForumPostId với comment cha.");
            }

            existing.ReplyId = dto.ReplyId;
            existing.ForumPostId = dto.ForumPostId;
            existing.UserId = dto.UserId;
            existing.Content = dto.Content.Trim();
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

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

        private ForumCommentDTO BuildCommentTree(
            ForumComment comment,
            Dictionary<Guid, List<ForumComment>> replyLookup)
        {
            var dto = _mapper.Map<ForumCommentDTO>(comment);

            if (replyLookup.TryGetValue(comment.Id, out var replies))
            {
                dto.Replies = replies
                    .Select(x => BuildCommentTree(x, replyLookup))
                    .ToList();
            }

            return dto;
        }
    }
}
