using Comments.Application.Dtos;
using Comments.Application.Requests;
using Microsoft.AspNetCore.Http;

namespace Comments.Application.Interfaces.Services
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CreateCommentRequest comment, CancellationToken cancellationToken, IFormFile? file=null);
        Task<List<CommentDto>> GetCommentsAsync(CommentQuery query, CancellationToken cancellationToken, int? parentId=null);
        Task<CommentDto> GetCommentById(int id, CancellationToken cancellationToken);
        Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}