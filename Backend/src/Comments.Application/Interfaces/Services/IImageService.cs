using Microsoft.AspNetCore.Http;

namespace Comments.Application.Interfaces.Services
{
    public interface IImageService
    {
        Task<Guid> ProcessAndSaveAsync(IFormFile imageFile, CancellationToken cancellationToken);
    }
}
