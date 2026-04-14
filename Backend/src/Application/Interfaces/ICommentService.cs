using Comments.Contracts;
using Comments.Models.Filters;

namespace Comments.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponse> CreateCommentAsync(CommentRequest comment, IFormFile? file);
        Task<List<CommentResponse>> GetCommentsAsync(CommentQuery query, int? parentId=null);
        Task<CommentResponse> GetCommentById(int id);
    }
}