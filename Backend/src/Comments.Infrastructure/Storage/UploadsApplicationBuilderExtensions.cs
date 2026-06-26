using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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
                
                FileProvider = new PhysicalFileProvider(@"D:\programming\projects\VisualStudio\csharp\Comments-v2\Backend\src\uploads"),
                RequestPath = "/uploads"
            });

            return app;
        }
    }
}
