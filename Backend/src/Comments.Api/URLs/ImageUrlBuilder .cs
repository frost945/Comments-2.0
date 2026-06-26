namespace Comments.Api.URLs
{
    public class ImageUrlBuilder
    {
        public string? GetOriginalUrl(string? fileName)
        {
            return fileName == null
                ? null
                : $"/uploads/images/original/{fileName}";
        }

        public string? GetPreviewUrl(string? fileName)
        {
            return fileName == null
                ? null
                : $"/uploads/images/preview/{fileName}";
        }
    }
}
