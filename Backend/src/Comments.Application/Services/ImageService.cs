using Comments.Application.Interfaces.FileStorage;
using Comments.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Comments.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageStorage _storage;
        private readonly ILogger<ImageService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int MaxPreviewWidth = 320;
        private const int MaxPreviewHeight = 240;
        private const long MaxFileSize = 5 * 1024 * 1024;
        //for png
        const int MaxImageWidth = 10000;
        const int MaxImageHeight = 10000;

        public ImageService(IImageStorage storage, ILogger<ImageService> logger )
        {
            _storage = storage;
            _logger = logger;
        }

        public async Task<Guid> ProcessAndSaveAsync(IFormFile imageFile, CancellationToken cancellationToken)
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
            if (!_allowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Unsupported image format: {extension}");
            }

            var imageId = Guid.NewGuid();

            var originalName = $"{imageId}{extension}";
            var previewName = $"{imageId}_preview.webp";

            try
            {
                await using var imageOriginalStream = imageFile.OpenReadStream();

                var imageFormat = await Image.DetectFormatAsync(imageOriginalStream, cancellationToken);

                if (imageFormat is null)
                {
                    throw new ArgumentException(
                        "Unknown image format.");
                }

                // real format validation (not only extension)
                if (imageFormat is not JpegFormat
                    && imageFormat is not PngFormat
                    && imageFormat is not GifFormat)
                {
                    throw new ArgumentException(
                        "Unsupported image format.");
                }

                // rewind after DetectFormatAsync
                imageOriginalStream.Position = 0;
                
                using var image = await Image.LoadAsync(imageOriginalStream, cancellationToken);

                //в основном для png, чтобы избежать нагрузки сервера при обработке слишком больших изображений
                if (image.Width > MaxImageWidth || image.Height > MaxImageHeight)
                {
                    throw new ArgumentException("Image dimensions are too large.");
                }

                // rewind after LoadAsync
                imageOriginalStream.Position = 0;

                await _storage.SaveOriginalAsync(imageOriginalStream, originalName, cancellationToken);

                if (image.Width > MaxPreviewWidth || image.Height > MaxPreviewHeight)
                {
                    using var previewImage = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();

                    ResizeImage(previewImage);

                    using var imagePreviewStream = new MemoryStream();

                    await previewImage.SaveAsync(imagePreviewStream, new WebpEncoder
                    {
                        Quality = 75,
                        Method = WebpEncodingMethod.BestQuality
                    }
                    , cancellationToken);

                    imagePreviewStream.Position = 0;

                    await _storage.SavePreviewAsync(imagePreviewStream, previewName, cancellationToken);
                }
                
                return imageId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Image processing failed. ImageId: {ImageId}, Extension: {Extension}",
                    imageId,
                    extension);

                throw new InvalidOperationException("Image processing failed. Please try again later.");
            }
        }

        private void ResizeImage(Image image)
        {
            var ratioX = (double)MaxPreviewWidth / image.Width;
            var ratioY = (double)MaxPreviewHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            image.Mutate(x => x.Resize(newWidth, newHeight));
        }
    }
}