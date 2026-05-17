namespace Comments.Application.Interfaces.Services
{
    public interface IImageService
    {
        Task<Guid> ProcessAndSaveImageAsync(IFormFile imageFile, CancellationToken cancellationToken);
        string? GetImagePreviewUrl(Guid? imageId);
        string? GetImageOriginalUrl(Guid? imageId);

    }
}
