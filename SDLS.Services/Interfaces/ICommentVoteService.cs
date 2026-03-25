using SDLS.Model.DTOs;
using SDLS.Model.DTOs.CommentVote;

namespace SDLS.Services.Interfaces
{
    public interface ICommentVoteService
    {
        Task<List<CommentVoteDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? forumCommentId = null);
        Task<PagedResult<CommentVoteDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? forumCommentId = null, int page = 1, int pageSize = 20);
        Task<CommentVoteDTO?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(CommentVoteCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, CommentVoteUpdateDTO dto);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}