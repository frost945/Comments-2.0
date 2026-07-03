namespace Comments.Api.Contracts
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid? TextFileId { get; set; }
        public string? TextFileName { get; set; }
        public string? ImagePreviewUrl { get; set; }
        public string? ImageOriginalUrl { get; set; }
        public int ReplyCount { get; set; } = 0;
        public CommentResponse() { }
        public CommentResponse(int id, string userName, string text, DateTime createdAt, Guid textFileId, string textFileName, string imagePreviewUrl, string imageOriginalUrl, int replyCount)
        {
            Id = id;
            UserName = userName;
            Text = text;
            CreatedAt = createdAt;
            TextFileId = textFileId;
            TextFileName = textFileName;
            ImagePreviewUrl = imagePreviewUrl;
            ImageOriginalUrl = imageOriginalUrl;
            ReplyCount = replyCount;
        }
    }
}
