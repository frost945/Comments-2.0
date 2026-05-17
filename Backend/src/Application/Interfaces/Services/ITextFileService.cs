namespace Comments.Application.Interfaces.Services
{
    public interface ITextFileService
    {
        Task<(Guid fileId, string originalFileName)> ProcessAndSaveTextFileAsync(IFormFile file, CancellationToken cancellationToken);
        string GetTextFilePath(Guid id);
    }
}
