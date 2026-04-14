using Serilog.Core;

using Serilog;

namespace Comments.Infrastructure.Logging
{
    public static class AuditLoggerExtensions
    {
        private const string AuditSource = "Comments.Audit";
        public static void LogAuditUser(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
        {
            Log.ForContext(Constants.SourceContextPropertyName, AuditSource)
                .ForContext("AuditUser", true)
                .Information(message, args);
        }
    }
}
