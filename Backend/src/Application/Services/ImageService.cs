using Comments.Application.Interfaces.Services;
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
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int MaxPreviewWidth = 320;
        private const int MaxPreviewHeight = 240;
        private const long MaxFileSize = 5 * 1024 * 1024;
        private readonly string _originalImagesDir;
        private readonly string _previewImagesDir;
        //for png
        const int MaxImageWidth = 10000;
        const int MaxImageHeight = 10000;

        public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger )
        {
            _environment = environment;
            _logger = logger;
            _originalImagesDir = Path.Combine(_environment.ContentRootPath, "uploads", "images", "original");
            _previewImagesDir = Path.Combine(_environment.ContentRootPath, "uploads", "images", "preview");
        }

        public async Task<Guid> ProcessAndSaveImageAsync(IFormFile imageFile, CancellationToken cancellationToken)
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

            //original image path
            var nameOriginal = $"{imageId}{extension}";
            var originalPath = Path.Combine(_originalImagesDir, nameOriginal);
            Directory.CreateDirectory(_originalImagesDir);

            //preview image path
            var previewName = $"{imageId}_preview.webp";
            var previewPath = Path.Combine(_previewImagesDir, previewName);
            Directory.CreateDirectory(_previewImagesDir);

            try
            {
                await using var imageStream = imageFile.OpenReadStream();

                var imageFormat = await Image.DetectFormatAsync(imageStream, cancellationToken);

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
                imageStream.Position = 0;
                
                using var image = await Image.LoadAsync(imageStream, cancellationToken);

                //в основном для png, чтобы избежать нагрузки сервера при обработке слишком больших изображений
                if (image.Width > MaxImageWidth || image.Height > MaxImageHeight)
                {
                    throw new ArgumentException("Image dimensions are too large.");
                }

                // rewind after LoadAsync
                imageStream.Position = 0;

                // save original file without re-encoding
                await using (var originalStream = new FileStream(
                    originalPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true))
                {
                    // Save original image in its original format
                    await imageStream.CopyToAsync(originalStream, cancellationToken);
                }

                if (image.Width > MaxPreviewWidth || image.Height > MaxPreviewHeight)
                {
                    using var previewImage = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();

                    ResizeImage(previewImage);

                    await using var previewStream = new FileStream(
                        previewPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 81920,
                        useAsync: true);

                    // Save preview image in WebP format
                    await previewImage.SaveAsync(previewStream, new WebpEncoder
                    {
                        Quality = 75,
                        Method = WebpEncodingMethod.BestQuality
                    },
                    cancellationToken);
                }
                
                return imageId;
            }
            catch (Exception ex)
            {
                // If there is an error, delete the partially created file
                if (File.Exists(originalPath))
                    File.Delete(originalPath);

                if (File.Exists(previewPath))
                    File.Delete(previewPath);

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

        public string? GetImagePreviewUrl(Guid? imageId)
        {
            if (imageId == null)
                return null;

            var filePath = Path.Combine(_previewImagesDir, $"{imageId}_preview.webp");

            // If a preview exists, return its URL; otherwise, return the original image URL
            return (File.Exists(filePath))
                ? $"/uploads/images/preview/{imageId}_preview.webp"
                : GetImageOriginalUrl(imageId);
        }

        public string? GetImageOriginalUrl(Guid? imageId)
        {
            if (imageId == null)
                return null;

            foreach (var ext in _allowedExtensions)
            {
                var filePath = Path.Combine(_originalImagesDir, $"{imageId}{ext}");

                if (File.Exists(filePath))
                    return $"/uploads/images/original/{Path.GetFileName(filePath)}";
            }

            return null;
        }
    }
}