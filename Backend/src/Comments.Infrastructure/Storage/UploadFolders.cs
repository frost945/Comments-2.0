// Ensure upload directories exist (only necessary for distributing static files - images)
// text files send through the controller
using Microsoft.Extensions.Configuration;

namespace Comments.Infrastructure.Storage
{
    public class UploadFolders
    {
        private readonly IConfiguration _config;

        private readonly string _originalImagesDir;
        private readonly string _previewImagesDir;

        public UploadFolders(IConfiguration config)
        {
            _config = config;
            _originalImagesDir = _config["Storage:Images:OriginalPath"] ?? throw new InvalidOperationException("Storage path is not configured.");
            _previewImagesDir = _config["Storage:Images:PreviewPath"] ?? throw new InvalidOperationException("Storage path is not configured.");
        }
        
        public void EnsureCreated()
        {
            Directory.CreateDirectory(_originalImagesDir);
            Directory.CreateDirectory(_previewImagesDir);

        }
    }
}
