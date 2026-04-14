using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Comments.Application.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int MaxWidth = 320;
        private const int MaxHeight = 240;
        private const long MaxFileSize = 5 * 1024 * 1024;
        private static readonly Dictionary<string, IImageEncoder> Encoders =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [".jpg"] = new JpegEncoder { Quality = 80 },
                [".jpeg"] = new JpegEncoder { Quality = 80 },
                [".png"] = new PngEncoder(),
                [".gif"] = new GifEncoder()
            };

        public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger )
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<Guid> ProcessAndSaveImageAsync(IFormFile imageFile)
        {
            // checking file size
            if (imageFile.Length > MaxFileSize)
            {
                throw new ArgumentException($"The file size must not exceed {MaxFileSize / 1024 / 1024}MB");
            }

            // checking file extension
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Acceptable formats: JPG, GIF, PNG");
            }

            var imageId = Guid.NewGuid();
            //original image path
            var nameOriginal = $"{imageId}{extension}";
            var originalDir = Path.Combine(_environment.ContentRootPath, "uploads", "images", "original");

            if (!Directory.Exists(originalDir))
            {
                Directory.CreateDirectory(originalDir);
            }

            var originalPath = Path.Combine(originalDir, nameOriginal);

            //preview image path
            var previewName = $"{imageId}_preview.webp";
            var previewDir = Path.Combine(_environment.ContentRootPath, "uploads", "images", "preview");

            if (!Directory.Exists(previewDir))
            {
                Directory.CreateDirectory(previewDir);
            }
            var previewPath = Path.Combine(previewDir, previewName);

            try
            {
                await using var imageStream = imageFile.OpenReadStream();
                using var image = await Image.LoadAsync(imageStream);
                await using var originalStream = new FileStream(originalPath, FileMode.Create);

                // Save original image in its original format
                await SaveImageAsync(image, originalStream, extension);

                if (image.Width > MaxWidth || image.Height > MaxHeight)
                {
                    using var previewImage = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();

                    ResizeImage(previewImage);
                  
                    await using var previewStream = new FileStream(previewPath, FileMode.Create);

                    // Save preview image in WebP format
                    await previewImage.SaveAsync(previewStream, new WebpEncoder
                    {
                        Quality = 75,
                        Method = WebpEncodingMethod.BestQuality
                    });
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
            var ratioX = (double)MaxWidth / image.Width;
            var ratioY = (double)MaxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            image.Mutate(x => x.Resize(newWidth, newHeight));
        }

        private async Task SaveImageAsync(Image image, Stream stream, string extension)
        {
            if (!Encoders.TryGetValue(extension, out var encoder))
                throw new ArgumentException($"Unsupported image format: {extension}");

            await image.SaveAsync(stream, encoder);
        }

        public string? GetImagePreviewUrl(Guid? imageId)
        {
            if (imageId == null)
                return null;

            var previewFileName = GetPreviewFileName(imageId.Value);

            return previewFileName != null
                ? $"/uploads/images/preview/{previewFileName}"
                : GetImageOriginalUrl(imageId);
        }

        public string? GetImageOriginalUrl(Guid? imageId)
        {
            if (imageId == null)
                return null;

            foreach (var ext in _allowedExtensions)
            {
                var filePath = Path.Combine(
                    _environment.ContentRootPath,
                    "uploads", "images", "original",
                    $"{imageId}{ext}");

                if (File.Exists(filePath))
                    return $"/uploads/images/original/{Path.GetFileName(filePath)}";
            }

            return null;
        }

        private string? GetPreviewFileName(Guid imageId)
        {
            foreach (var ext in _allowedExtensions)
            {
                var filePath = Path.Combine(
                    _environment.ContentRootPath,
                    "uploads", "images", "preview",
                    $"{imageId}_preview{ext}");

                if (File.Exists(filePath))
                    return Path.GetFileName(filePath);
            }

            return null;
        }

       /* public (string path, string contentType)? GetImagePreviewFile(Guid imageId)
        {
            foreach (var ext in _allowedExtensions)
            {
                var path = Path.Combine(
                    _environment.ContentRootPath,
                    "uploads", "images", "preview",
                    $"{imageId}_preview{ext}");

                if (File.Exists(path))
                {
                    var contentType = ext.TrimStart('.'); // Remove the dot from the extension for correct MIME type
                    return (path, contentType);
                }
            }
            return null;
        }*/
    }
}