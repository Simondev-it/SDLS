using SDLS.Model.DTOs;
using SDLS.Model.DTOs.CommentVote;

namespace SDLS.Services.Interfaces
{
    public interface ICommentVoteService
    {
        Task<List<CommentVoteDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumCommentId = null,
            int? status = null);

        Task<PagedResult<CommentVoteDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? forumCommentId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<CommentVoteDTO?> GetByIdAsync(Guid id);
        Task<CommentVoteDTO> CreateAsync(CommentVoteCreateDTO dto);
        Task<CommentVoteDTO> UpdateAsync(Guid id, CommentVoteUpdateDTO dto);
        Task<CommentVoteDTO> DeleteSoftAsync(Guid id);
        Task<CommentVoteDTO> DeleteHardAsync(Guid id);
    }
}