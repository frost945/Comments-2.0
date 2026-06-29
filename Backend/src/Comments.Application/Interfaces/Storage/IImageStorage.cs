namespace Comments.Application.Interfaces.Storage
{
    public interface IImageStorage
    {
        Task SaveOriginalAsync(Stream stream, string fileName, CancellationToken ct);
        Task SavePreviewAsync(Stream stream, string fileName, CancellationToken ct);
        string? GetOriginalName(Guid imageId);
        string? GetPreviewName(Guid imageId);
    }
}
