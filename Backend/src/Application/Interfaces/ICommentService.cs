using Comments.Contracts;
using Comments.Models.Filters;
using System.Threading;

namespace Comments.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponse> CreateCommentAsync(CommentRequest comment, CancellationToken cancellationToken, IFormFile? file=null);
        Task<List<CommentResponse>> GetCommentsAsync(CommentQuery query, CancellationToken cancellationToken, int? parentId=null);
        Task<CommentResponse> GetCommentById(int id, CancellationToken cancellationToken);
    }
}