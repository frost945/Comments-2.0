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

      /* [HttpGet("image/preview/{id:guid}")]
        public IActionResult DownloadImage(Guid id)
        {
            Console.WriteLine("start DownloadImage controller");
           var result = _imageService.GetImagePreviewFile(id);

            if (result == null || !System.IO.File.Exists(result.Value.path))
                return NotFound();

            //var provider = new FileExtensionContentTypeProvider();
            //provider.TryGetContentType(result.Value.path, out var contentType);

           // if (contentType == null)
             //   contentType = "application/octet-stream";

            var contentType = $"image/{result.Value.contentType}";
            Console.WriteLine($"Content-Type: {contentType}");
            Console.WriteLine($"File Path: {Path.GetFileName(result.Value.path)}");

            return PhysicalFile(result.Value.path, contentType);
        }*/
    }
}