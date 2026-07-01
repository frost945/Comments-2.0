using Comments.Api.Mappers;
using Comments.Api.URLs;

namespace Comments.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddScoped<CommentResponseMapper>();
            services.AddSingleton<ImageUrlBuilder>();

            return services;
        }
    }
}
