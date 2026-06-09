using Comments.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileDownloadController : ControllerBase
    {
        private readonly ITextFileService _textFileService;

        public FileDownloadController(ITextFileService textFileService)
        {
            _textFileService = textFileService;
        }

        [HttpGet("text/{id:guid}")]
        public IActionResult DownloadTextFile(Guid id)
        {
            var path = _textFileService.GetTextFilePath(id);

            if (!System.IO.File.Exists(path))
                return NotFound();

            return PhysicalFile(path, "text/plain");
        }
    }
}