using Comments.Application.Interfaces.FileStorage;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileDownloadController : ControllerBase
    {
        private readonly ITextFileStorage _storage;

        public FileDownloadController(ITextFileStorage storage)
        {
            _storage = storage;
        }

        [HttpGet("text/{id:guid}")]
        public IActionResult DownloadTextFile(Guid id)
        {
            var path = _storage.GetFilePath(id);

            if (!System.IO.File.Exists(path))
                return NotFound();

            return PhysicalFile(path, "text/plain");
        }
    }
}