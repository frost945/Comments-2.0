using Comments.Api.Contracts;
using Comments.Application.Requests;

namespace Comments.Api.Mappers
{
    public static class CommentRequestMapper
    {
        public static CreateCommentRequest ToRequest(CommentRequest request)
        {
            return new CreateCommentRequest
            {
                ParentId = request.ParentId,
                UserName = request.UserName,
                Email = request.Email,
                Text = request.Text,
                File = request.File
            };
        }
    }
}
