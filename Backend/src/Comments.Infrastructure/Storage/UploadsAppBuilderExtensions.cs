using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Comments.Infrastructure.Storage
{
    public static class UploadsAppBuilderExtensions
    {
        public static IApplicationBuilder UseUploads(this IApplicationBuilder app)
        {
            var paths = app.ApplicationServices.GetRequiredService<StoragePathProvider>();

            Directory.CreateDirectory(paths.OriginalImagesPath);
            Directory.CreateDirectory(paths.PreviewImagesPath);
            Directory.CreateDirectory(paths.TextFilesPath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(paths.UploadsRoot),
                RequestPath = "/uploads"
            });

            return app;
        }
    }
}
