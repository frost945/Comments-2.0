using Comments.Application.Interfaces.FileStorage;

namespace Comments.Infrastructure.Storage
{
    public class TextFileStorage : ITextFileStorage
    {
        private readonly string _textFilesDirectory;

        public TextFileStorage(StoragePathProvider paths)
        {
            _textFilesDirectory = paths.TextFilesPath;
        }

        public async Task SaveAsync(Stream stream, string fileName, CancellationToken ct)
        {
            Directory.CreateDirectory(_textFilesDirectory);

            var path = Path.Combine(_textFilesDirectory, fileName);

            await using var fileStream = new FileStream(
                path,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await stream.CopyToAsync(fileStream, ct);
        }

        public string? GetFilePath(Guid fileId)
        {
            Console.WriteLine($"Getting file path for : {_textFilesDirectory}");
            return Path.Combine(
                _textFilesDirectory,
                $"{fileId}.txt");
        }
    }
}
