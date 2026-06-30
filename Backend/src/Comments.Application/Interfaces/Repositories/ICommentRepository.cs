using Comments.Application.Dtos;
using Comments.Application.Requests;
using Comments.Domain.Models;

namespace Comments.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        public Task AddAsync(Comment comment, CancellationToken ct);
        public Task<bool> ParentExistsAsync(int id, CancellationToken ct);
        public Task<List<CommentDto>> GetListOffsetAsync(CommentQuery commentQuery, CancellationToken ct, int? parentId = null);
        public Task<List<CommentDto>> GetListKeysetAsync(CommentQuery commentQuery, CancellationToken ct, int? parentId = null);
        public Task<CommentDto?> GetByIdAsync(int id, CancellationToken ct);
    }
}
