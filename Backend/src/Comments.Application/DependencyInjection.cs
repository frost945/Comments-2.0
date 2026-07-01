using Comments.Application.Interfaces.Services;
using Comments.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Comments.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ITextFileService, TextFileService>();

            return services;
        }
    }
}
