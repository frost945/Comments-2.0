namespace Comments.Application.Interfaces.Logging
{
    public interface IAuditLogger
    {
        void LogCommentCreated(int commentId, string userName, bool hasFile, int? parentId);
    }
}
