using Comments.Application.Constants;
using Comments.Application.Interfaces.Storage;

namespace Comments.Infrastructure.Storage
{
    public class LocalImageStorage : IImageStorage
    {
        private readonly string _originalImagesDir;
        private readonly string _previewImagesDir;

        public LocalImageStorage(StoragePathProvider path)
        {
            _originalImagesDir = path.OriginalImagesPath;
            _previewImagesDir = path.PreviewImagesPath;
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
            foreach (var ext in ImageConstants.AllowedExtensions)
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