using Comments.Application.Interfaces.Services;
using Comments.Application.Constants;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;


namespace Comments.Infrastructure.ImageProcessing;

public class ImageProcessor : IImageProcessor
{
    public async Task<ImageProcessingResult> ProcessAsync(
        Stream originalStream,
        CancellationToken cancellationToken)
    {
        var format = await Image.DetectFormatAsync(originalStream, cancellationToken);

        if (format is null)
            throw new ArgumentException("Unknown image format.");

        if (format is not JpegFormat &&
            format is not PngFormat &&
            format is not GifFormat)
        {
            throw new ArgumentException("Unsupported image format.");
        }

        originalStream.Position = 0;

        using var image = await Image.LoadAsync(originalStream, cancellationToken);

        if (image.Width > ImageConstants.MaxImageWidth || image.Height > ImageConstants.MaxImageHeight)
            throw new ArgumentException("Image dimensions are too large.");

        originalStream.Position = 0;

        MemoryStream? previewStream = null;

        if (image.Width > ImageConstants.MaxPreviewWidth || image.Height > ImageConstants.MaxPreviewHeight)
        {
            using var previewImage =
                image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();

            Resize(previewImage);

            previewStream = new MemoryStream();

            await previewImage.SaveAsync(
                previewStream,
                new WebpEncoder
                {
                    Quality = ImageConstants.Quality,
                    Method = WebpEncodingMethod.BestQuality
                },
                cancellationToken);

            previewStream.Position = 0;
        }

        return new ImageProcessingResult
        (
            originalStream,
            previewStream
        );
    }

    private static void Resize(Image image)
    {
        var ratioX = (double)ImageConstants.MaxPreviewWidth / image.Width;
        var ratioY = (double)ImageConstants.MaxPreviewHeight / image.Height;

        var ratio = Math.Min(ratioX, ratioY);

        image.Mutate(x => x.Resize(
            (int)(image.Width * ratio),
            (int)(image.Height * ratio)));
    }
}