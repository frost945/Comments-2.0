using Microsoft.AspNetCore.Http;

namespace Comments.Application.Interfaces.Services
{
    public interface ITextFileService
    {
        Task<(Guid fileId, string originalFileName)> ProcessAndSaveAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
