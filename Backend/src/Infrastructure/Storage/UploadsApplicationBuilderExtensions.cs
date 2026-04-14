using Microsoft.Extensions.FileProviders;

namespace Comments.Infrastructure.Storage
{
    public static class UploadsApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUploads(this IApplicationBuilder app)
        {
            var uploadFolders = app.ApplicationServices.GetRequiredService<UploadFolders>();

            uploadFolders.EnsureCreated();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadFolders.ImagesPath),
                RequestPath = "/uploads/images"
            });

            return app;
        }
    }
}
