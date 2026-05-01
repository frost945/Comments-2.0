using Comments.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileDownloadController : ControllerBase
    {
        private readonly TextFileService _textFileService;
        private readonly ImageService _imageService;

        public FileDownloadController(TextFileService textFileService, ImageService imageService)
        {
            _textFileService = textFileService;
            _imageService = imageService;
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