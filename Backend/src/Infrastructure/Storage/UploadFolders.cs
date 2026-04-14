// Ensure upload directories exist (only necessary for distributing static files - images)
// text files send through the controller

namespace Comments.Infrastructure.Storage
{
    public class UploadFolders
    {
        private readonly IWebHostEnvironment _env;

        private readonly string BasePath;
        public string ImagesPath { get; }

        public UploadFolders(IWebHostEnvironment env)
        {
            _env = env;
            BasePath = Path.Combine(_env.ContentRootPath, "uploads");
            ImagesPath = Path.Combine(BasePath, "images");
        }
        
        public void EnsureCreated()
        {
            var paths = new[]
             {
                Path.Combine(ImagesPath, "original"),
                Path.Combine(ImagesPath, "preview"),
             };

            foreach (var path in paths)
            {
                Directory.CreateDirectory(path);
            }
         }
    }
}
