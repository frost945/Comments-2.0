using Comments.Contracts.Filters;
using Comments.Domain.Models;

namespace Comments.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        public Task AddAsync(Comment comment, CancellationToken ct);
        public Task<bool> ParentExistsAsync(int id, CancellationToken ct);
        public Task<List<CommentRawDto>> GetListOffsetAsync(CommentQuery commentQuery, CancellationToken ct, int? parentId = null);
        public Task<List<CommentRawDto>> GetListKeysetAsync(CommentQuery commentQuery, CancellationToken ct, int? parentId = null);
        public Task<CommentRawDto?> GetByIdAsync(int id, CancellationToken ct);
    }
}
