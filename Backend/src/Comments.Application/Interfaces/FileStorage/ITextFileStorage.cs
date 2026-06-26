namespace Comments.Application.Interfaces.FileStorage
{
    public interface ITextFileStorage
    {
        Task SaveAsync(Stream stream, string fileName, CancellationToken ct);
        string? GetFilePath(Guid fileId);
    }
}
