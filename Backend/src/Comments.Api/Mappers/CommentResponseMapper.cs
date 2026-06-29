using Comments.Api.URLs;
using Comments.Application.Dtos;
using Comments.Application.Interfaces.Storage;
using Comments.Contracts;

namespace Comments.Api.Mappers
{
    public class CommentResponseMapper
    {
        private readonly IImageStorage _imageStorage;
        private readonly ImageUrlBuilder _imageUrlBuilder;
        public CommentResponseMapper(IImageStorage imageStorage, ImageUrlBuilder imageUrlBuilder)
        {
            _imageStorage = imageStorage;
            _imageUrlBuilder = imageUrlBuilder;
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
                ImagePreviewUrl = _imageUrlBuilder.GetPreviewUrl(imagePreviewName),
                ImageOriginalUrl = _imageUrlBuilder.GetOriginalUrl(imageOriginalName),
                ReplyCount = comment.ReplyCount
            };
        }
    }
}
