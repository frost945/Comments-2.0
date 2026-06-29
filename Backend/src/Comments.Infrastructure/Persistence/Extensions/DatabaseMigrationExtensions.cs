using Comments.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Comments.Infrastructure.Persistence.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CommentsDbContext>>();

            try
            {
                var db = scope.ServiceProvider.GetRequiredService<CommentsDbContext>();

                logger.LogInformation(
                    "DB Connection: {Connection}",
                    await db.Database.CanConnectAsync());

                // db.Database.EnsureDeleted();
                // db.Database.EnsureCreated();

                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying migrations.");
            }
        }
    }
}
