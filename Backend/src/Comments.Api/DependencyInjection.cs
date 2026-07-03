using Comments.Api.Mappers;

namespace Comments.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddScoped<CommentResponseMapper>();

            return services;
        }
    }
}
