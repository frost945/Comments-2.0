using Comments.Application.Interfaces.FileStorage;
using Microsoft.Extensions.Configuration;

namespace Comments.Infrastructure.Storage
{
    public class LocalImageStorage : IImageStorage
    {
        private readonly IConfiguration _config;
        private readonly string _originalImagesDir;
        private readonly string _previewImagesDir;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public LocalImageStorage(IConfiguration config)
        {
            _config = config;
            _originalImagesDir = _config["Storage:Images:OriginalPath"] ?? throw new InvalidOperationException("Storage path is not configured.");
            _previewImagesDir = _config["Storage:Images:PreviewPath"] ?? throw new InvalidOperationException("Storage path is not configured.");
        }

        public async Task SaveOriginalAsync(Stream stream, string fileName, CancellationToken ct)
        {
            var originalPath = Path.Combine(_originalImagesDir, fileName);

            Directory.CreateDirectory(_originalImagesDir);

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
                await stream.CopyToAsync(originalStream, ct);
            }
        }

        public async Task SavePreviewAsync(Stream stream, string fileName, CancellationToken ct)
        {
            var previewPath = Path.Combine(_previewImagesDir, fileName);

            Directory.CreateDirectory(_previewImagesDir);

            await using var previewStream = new FileStream(
                previewPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await stream.CopyToAsync(previewStream, ct);
        }

        public string? GetOriginalName(Guid imageId)
        {
            foreach (var ext in _allowedExtensions)
            {
                var filePath = Path.Combine(_originalImagesDir, $"{imageId}{ext}");

                if (File.Exists(filePath))
                    return Path.GetFileName(filePath);
            }
            return null;
        }

        public string? GetPreviewName(Guid imageId)
        {
            var filePath = Path.Combine(_previewImagesDir, $"{imageId}_preview.webp");

            return File.Exists(filePath)
                ? Path.GetFileName(filePath)
                : null;
        }
    }
}