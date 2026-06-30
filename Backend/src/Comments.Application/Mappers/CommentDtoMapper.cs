using Comments.Application.Dtos;
using Comments.Domain.Models;

namespace Comments.Application.Mappers
{
    public static class CommentDtoMapper
    {
        public static CommentDto ToCommentDto(Comment comment, int replyCount = 0)
        {
            return new CommentDto
            {
                Id = comment.Id,
                UserName = comment.UserName,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                ImageId = comment.ImageId,
                TextFileId = comment.TextFileId,
                TextFileName = comment.OriginalTextFileName,
                ReplyCount = replyCount
            };
        }
    }
}
