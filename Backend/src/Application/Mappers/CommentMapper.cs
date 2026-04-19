using Comments.Contracts;
using Comments.Models;
using Comments.Application.Mappers;

namespace Comments.Application.Mappers
{
    public static class CommentMapper
    {
        public static CommentResponse ToResponse(Comment comment, string? imagePreviewUrl, string? imageOriginalUrl, int replyCount = 0)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                UserName = comment.UserName,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                ImagePreviewUrl = imagePreviewUrl,
                ImageOriginalUrl = imageOriginalUrl,
                TextFileId = comment.TextFileId,
                TextFileName = comment.OriginalTextFileName,
                ReplyCount = replyCount
            };
        }

        public static CommentResponse FromRaw(CommentRawDto c, string? imagePreviewUrl, string? imageOriginalUrl)
        {
            return new CommentResponse
            {
                Id = c.Id,
                UserName = c.UserName,
                Text = c.Text,
                CreatedAt = c.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                ImagePreviewUrl = imagePreviewUrl,
                ImageOriginalUrl = imageOriginalUrl,
                TextFileId = c.TextFileId,
                TextFileName = c.OriginalTextFileName,
                ReplyCount = c.ReplyCount
            };
        }
    }
}
