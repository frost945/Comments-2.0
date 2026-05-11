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
        private readonly string _textFilesDirectory;

        public TextFileService(IWebHostEnvironment environment, ILogger<TextFileService> logger)
        {
            _environment = environment;
            _logger = logger;
            _textFilesDirectory = Path.Combine(_environment.ContentRootPath, "uploads", "textfiles");
        }
        

        public async Task<(Guid fileId, string originalFileName)> ProcessAndSaveTextFileAsync(IFormFile textFile, CancellationToken cancellationToken)
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

            // create unique file name
            var fileId = Guid.NewGuid();

            // save original file name
            var originalFileName = Path.GetFileName(textFile.FileName);

            Directory.CreateDirectory(_textFilesDirectory);

            var path = Path.Combine(_textFilesDirectory, $"{fileId}{extension}");

            try
            {
                // Save file
                await using var fileStream = new FileStream(
                    path,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);
              
                await textFile.CopyToAsync(fileStream, cancellationToken);

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

        public string GetTextFilePath(Guid fileId)
        {
            return Path.Combine(
                _textFilesDirectory,
                $"{fileId}.txt");
        }
    }
}
