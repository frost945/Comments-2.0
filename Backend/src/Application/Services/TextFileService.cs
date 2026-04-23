using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Comments.Application.Services
{
    public class TextFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TextFileService> _logger;
        private const long MaxFileSize = 100 * 1024; // 100 KB
        private readonly string[] _allowedExtensions = { ".txt" };

        public TextFileService(IWebHostEnvironment environment, ILogger<TextFileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<(Guid fileId, string originalFileName)> ProcessAndSaveTextFileAsync(IFormFile textFile, CancellationToken cancellationToken)
        {
            if (textFile.Length > MaxFileSize)
            {
                throw new ArgumentException($"File size must not exceed {MaxFileSize / 1024}KB");
            }

            var extension = Path.GetExtension(textFile.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Only TXT format is allowed");
            }

            // Reading the contents of the file
            string content;
            await using (var stream = textFile.OpenReadStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync(cancellationToken);
            }

            // Check encoding and content
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("File is empty or contains only spaces.");
            }

            // create unique file name
            var fileId = Guid.NewGuid();

            // save original file name
            var originalFileName = textFile.FileName;

            var dir = Path.Combine(_environment.ContentRootPath, "uploads", "textfiles");
            var path = Path.Combine(dir, $"{fileId}{extension}");

            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Save file
                await using var fileStream = new FileStream(path, FileMode.Create);
                await using var writer = new StreamWriter(fileStream, Encoding.UTF8);
                await writer.WriteAsync(content.AsMemory(), cancellationToken);

                return (fileId, originalFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
            "Error saving text file. Path: {Path}, OriginalFileName: {OriginalFileName}",
            path,
            originalFileName);

                throw new InvalidOperationException(
                    "Error processing text file. Try again later.");
            }
        }

        public string? GetTextFileUrl(Guid? fileId)
        {
            if (fileId is null)
                return null;

            var path = Path.Combine(
                _environment.ContentRootPath,
               "uploads", "textfiles",
               $"{fileId}.txt");

            if (!File.Exists(path))
                return null;

            return $"/uploads/textfiles/{fileId}.txt";
        }

        public string GetTextFilePath(Guid fileId)
        {
            string path = Path.Combine(
                _environment.ContentRootPath,
               "uploads", "textfiles",
               $"{fileId}.txt");

            return path;
        }
    }
}
