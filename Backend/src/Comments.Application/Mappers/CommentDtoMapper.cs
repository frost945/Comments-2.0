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

        public static CommentDto FromRaw(CommentRawDto c)
        {
            return new CommentDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                ImageId = c.ImageId,
                TextFileId = c.TextFileId,
                TextFileName = c.OriginalTextFileName,
                ReplyCount = c.ReplyCount
            };
        }
    }
}
