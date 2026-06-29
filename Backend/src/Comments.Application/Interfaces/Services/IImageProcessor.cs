namespace Comments.Application.Interfaces.Services
{
    public interface IImageProcessor
    {
        Task<ImageProcessingResult> ProcessAsync(Stream originalStream, CancellationToken ct);
    }

    public sealed class ImageProcessingResult
    {
        public ImageProcessingResult(Stream original, Stream? preview)
        {
            OriginalStream = original;
            PreviewStream = preview;
        }

        public Stream OriginalStream { get; }
        public Stream? PreviewStream { get; }
    }
}
