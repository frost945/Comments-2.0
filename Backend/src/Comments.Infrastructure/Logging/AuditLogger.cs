using Serilog;
using Comments.Application.Interfaces.Logging;

namespace Comments.Infrastructure.Logging
{
    public class AuditLogger : IAuditLogger
    {
        public void LogCommentCreated(int commentId, string userName, bool hasFile, int? parentId)
        {
            Log.ForContext("AuditUser", true)
               .Information(
                    "Created comment {CommentId}, UserName: {UserName}, HasFile: {HasFile}, ParentId: {ParentId}",
                    commentId,
                    userName,
                    hasFile,
                    parentId);
        }
    }
}
