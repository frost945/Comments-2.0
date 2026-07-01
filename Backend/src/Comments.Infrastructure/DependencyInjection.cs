using Comments.Application.Interfaces.Logging;
using Comments.Application.Interfaces.ImageProcessing;
using Comments.Application.Interfaces.Repositories;
using Comments.Application.Interfaces.Sanitization;
using Comments.Application.Interfaces.Storage;
using Comments.Infrastructure.Logging;
using Comments.Infrastructure.Persistence.Repositories;
using Comments.Infrastructure.Storage;
using Comments.Infrastructure.ImageProcessing;
using Comments.Infrastructure.Sanitization;

using Microsoft.Extensions.DependencyInjection;

namespace Comments.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IImageStorage, LocalImageStorage>();
            services.AddScoped<ITextFileStorage, TextFileStorage>();

            services.AddSingleton<StoragePathProvider>();
            services.AddSingleton<IAuditLogger, AuditLogger>();
            services.AddSingleton<IImageProcessor, ImageProcessor>();
            services.AddSingleton<IInputSanitizer, InputSanitizer>();


            return services;
        }
    }
}
