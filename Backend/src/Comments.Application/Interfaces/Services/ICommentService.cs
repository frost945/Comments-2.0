using Comments.Application.Mappers;
using Comments.Contracts;
using Comments.Contracts.Filters;
using Microsoft.AspNetCore.Http;

namespace Comments.Application.Interfaces.Services
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CommentRequest comment, CancellationToken cancellationToken, IFormFile? file=null);
        Task<List<CommentDto>> GetCommentsAsync(CommentQuery query, CancellationToken cancellationToken, int? parentId=null);
        Task<CommentDto> GetCommentById(int id, CancellationToken cancellationToken);
    }
}