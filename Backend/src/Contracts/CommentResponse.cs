namespace Comments.Contracts
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public Guid? ImageId { get; set; }
        public Guid? TextFileId { get; set; }
        public string? TextFileName { get; set; }
        public string? ImageOriginalUrl { get; set; }
        public int ReplyCount { get; set; } = 0;
        public CommentResponse() { }
        public CommentResponse(int id, string userName, string text, DateTime createdAt, Guid imageId, Guid textFileId, string textFileName, int replyCount)
        {
            Id = id;
            UserName = userName;
            Text = text;
            CreatedAt = createdAt.ToLocalTime().ToString("dd-MM-yyyy HH:mm");
            ImageId = imageId;
            TextFileId = textFileId;
            TextFileName = textFileName;
            ReplyCount = replyCount;
        }
    }
}
