using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Filters;

namespace Comments.Infrastructure.Logging
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()

                // Audit logs (file only)
                .WriteTo.Logger(audit => audit
                    .MinimumLevel.Information()
                    .Filter.ByIncludingOnly(Matching.WithProperty("AuditUser"))
                    .WriteTo.Async(a => a.File(
                        "logs/audit-user/audit-user-.txt",
                        rollingInterval: RollingInterval.Day,
                        buffered: false)))

                // Technical logs
                .WriteTo.Logger(tech => tech
                    .Filter.ByExcluding(Matching.WithProperty("AuditUser"))
                    .Filter.ByExcluding(logEvent =>
                        logEvent.MessageTemplate.Text.Contains("An unhandled exception has occurred"))
                    .WriteTo.Async(a => a.File(
                        "logs/tech/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        buffered: true))
                    .WriteTo.Console())

                .CreateLogger();
        }
    }
}
