namespace Comments.Api.UrlBuilders
{
    public static class ImageUrlBuilder
    {
        public static string? GetOriginalUrl(string? fileName)
        {
            return fileName == null
                ? null
                : $"/uploads/images/original/{fileName}";
        }

        public static string? GetPreviewUrl(string? fileName)
        {
            return fileName == null
                ? null
                : $"/uploads/images/preview/{fileName}";
        }
    }
}
