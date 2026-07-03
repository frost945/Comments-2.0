using Comments.Api.Contracts;
using Comments.Api.UrlBuilders;
using Comments.Application.Dtos;
using Comments.Application.Interfaces.Storage;

namespace Comments.Api.Mappers
{
    public class CommentResponseMapper
    {
        private readonly IImageStorage _imageStorage;
        public CommentResponseMapper(IImageStorage imageStorage)
        {
            _imageStorage = imageStorage;
        }
        public CommentResponse CreateCommentResponse(CommentDto comment)
        {
            string? imagePreviewName = null;
            string? imageOriginalName = null;

            if (comment.ImageId != null)
            {
                var imageId = comment.ImageId.Value;

                imagePreviewName = _imageStorage.GetPreviewName(imageId);
                imageOriginalName = _imageStorage.GetOriginalName(imageId);
            }

            return new CommentResponse
            {
                Id = comment.Id,
                UserName = comment.UserName,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                TextFileId = comment.TextFileId,
                TextFileName = comment.TextFileName,
                ImagePreviewUrl = ImageUrlBuilder.GetPreviewUrl(imagePreviewName),
                ImageOriginalUrl = ImageUrlBuilder.GetOriginalUrl(imageOriginalName),
                ReplyCount = comment.ReplyCount
            };
        }
    }
}
