
using Comments.Application.Interfaces.Services;
using Comments.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Comments.Application.Services
{
    public class TextFileService : ITextFileService
    {
        private readonly ITextFileStorage _storage;
        private readonly ILogger<TextFileService> _logger;
        private const long MaxFileSize = 100 * 1024; // 100 KB
        private readonly string[] _allowedExtensions = { ".txt" };

        public TextFileService(ILogger<TextFileService> logger, ITextFileStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }
        

        public async Task<(Guid fileId, string originalFileName)> ProcessAndSaveAsync(IFormFile textFile, CancellationToken ct)
        {
           if (textFile == null)
            {
                throw new ArgumentNullException(nameof(textFile));
            }

            if(textFile.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            if (textFile.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size must not exceed {MaxFileSize / 1024}KB");
            }

            var extension = Path.GetExtension(textFile.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Only TXT format is allowed");
            }

            var fileId = Guid.NewGuid();

            var originalFileName = Path.GetFileName(textFile.FileName);
            var storageFileName = $"{fileId}{extension}";

            try
             {
                await using var stream = textFile.OpenReadStream();

                await _storage.SaveAsync(stream, storageFileName, ct);

                return (fileId, originalFileName);
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex,
                    "Error saving text file. OriginalFileName: {OriginalFileName}",
                    originalFileName);

                 throw new InvalidOperationException(
                     "Error processing text file. Try again later.");
             }
        }
    }
}
