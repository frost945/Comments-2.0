namespace Comments.Application.Mappers
{
    public class CommentRawDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid? ImageId { get; set; }
        public Guid? TextFileId { get; set; }
        public string? OriginalTextFileName { get; set; }
        public int ReplyCount { get; set; }
    }
}
