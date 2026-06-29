using Comments.Application.Constants;
using Comments.Application.Interfaces.Services;
using Comments.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Comments.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageStorage _storage;
        private readonly IImageProcessor _processor;
        private readonly ILogger<ImageService> _logger;
        private const long MaxFileSize = 5 * 1024 * 1024;

        public ImageService(IImageStorage storage, IImageProcessor processor, ILogger<ImageService> logger)
        {
            _storage = storage;
            _processor = processor;
            _logger = logger;
        }

        public async Task<Guid> ProcessAndSaveAsync(IFormFile imageFile, CancellationToken ct)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile), "No image was uploaded.");
            }

            if (imageFile.Length == 0)
            {
                throw new ArgumentException("The uploaded image is empty.");
            }

            if (imageFile.Length > MaxFileSize)
            {
                throw new ArgumentException($"The file size must not exceed {MaxFileSize / 1024 / 1024}MB");
            }

            // checking file extension
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!ImageConstants.AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Unsupported image format: {extension}");
            }

            var imageId = Guid.NewGuid();
            var originalName = $"{imageId}{extension}";
            var previewName = $"{imageId}_preview.webp";

            try
            {
                await using var stream = imageFile.OpenReadStream();
                var result = await _processor.ProcessAsync(stream, ct);

                await _storage.SaveOriginalAsync(result.OriginalStream, originalName, ct);

                if(result.PreviewStream != null)
                await _storage.SavePreviewAsync(result.PreviewStream, previewName, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Image processing failed. ImageId: {ImageId}, Extension: {Extension}",
                    imageId,
                    extension);

                throw new InvalidOperationException("Image processing failed. Please try again later.", ex);
            }

            return imageId;
        }
    }
}