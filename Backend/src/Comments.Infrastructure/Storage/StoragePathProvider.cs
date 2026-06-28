using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Comments.Infrastructure.Storage
{
    public class StoragePathProvider
    {
        public string UploadsRoot { get; }
        public string OriginalImagesPath { get; }
        public string PreviewImagesPath { get; }
        public string TextFilesPath { get; }

        public StoragePathProvider(IOptions<StorageOptions> options, IWebHostEnvironment env) 
        {
            var configuredPath = options.Value.UploadsRoot;

            UploadsRoot = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(env.ContentRootPath, configuredPath);

            OriginalImagesPath = Path.Combine(UploadsRoot, "images", "original");
            PreviewImagesPath = Path.Combine(UploadsRoot, "images", "preview");
            TextFilesPath = Path.Combine(UploadsRoot, "textfiles");
        }
    }
}
