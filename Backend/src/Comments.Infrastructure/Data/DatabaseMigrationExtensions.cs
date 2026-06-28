using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Comments.Infrastructure.Data
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CommentsDbContext>();

            // db.Database.EnsureDeleted();
            // db.Database.EnsureCreated();

            if (app.Environment.IsDevelopment() ||
                app.Environment.IsEnvironment("Docker"))
            {
                await db.Database.MigrateAsync();
            }

            Log.Information(
                "DB Connection: {Connection}",
                await db.Database.CanConnectAsync());
        }
    }
}
