using Comments.Contracts;
using Comments.Models;
using Comments.Application.Mappers;

namespace Comments.Application.Mappers
{
    public static class CommentMapper
    {
        public static CommentResponse ToResponse(Comment comment, string? imageUrl, int replyCount = 0)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                UserName = comment.UserName,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                ImageId = comment.ImageId,
                ImageOriginalUrl = imageUrl,
                TextFileId = comment.TextFileId,
                TextFileName = comment.OriginalTextFileName,
                ReplyCount = replyCount
            };
        }

        public static CommentResponse FromRaw(CommentRawDto c, string? imageUrl)
        {
            return new CommentResponse
            {
                Id = c.Id,
                UserName = c.UserName,
                Text = c.Text,
                CreatedAt = c.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                ImageId = c.ImageId,
                ImageOriginalUrl = imageUrl,
                TextFileId = c.TextFileId,
                TextFileName = c.OriginalTextFileName,
                ReplyCount = c.ReplyCount
            };
        }
    }
}
