using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Size = SixLabors.ImageSharp.Size;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;
namespace CleanArchitecture.Blazor.Server.UI.Services;
public static class ImageProcessor
{
    /// <summary>
    /// Resizes and compresses an image from a stream to JPEG format
    /// </summary>
    /// <param name="sourceStream">The source stream containing the image</param>
    /// <param name="maxWidth">Maximum width of the resized image</param>
    /// <param name="maxHeight">Maximum height of the resized image</param>
    /// <param name="quality">JPEG compression quality (0-100)</param>
    /// <returns>A memory stream containing the resized and compressed JPEG image</returns>
    public static async Task<MemoryStream> ResizeAndCompressToJpegAsync(
        Stream sourceStream,
        int maxWidth = 1200,
        int maxHeight = 1200,
        int quality = 80)
    {
        // Ensure the stream is at the beginning
        sourceStream.Position = 0;

        // Load image from stream
        using var image = await Image.LoadAsync(sourceStream);

        // Resize with center-crop to exact target size (no distortion)
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(maxWidth, maxHeight),
            Mode = ResizeMode.Crop,
            Position = AnchorPositionMode.Center
        }));

        // Create new stream for the resized image
        var resizedStream = new MemoryStream();

        // Save as JPEG with compression
        await image.SaveAsJpegAsync(resizedStream, new JpegEncoder
        {
            Quality = quality // Compression level (0-100)
        });

        // Reset position to beginning of stream
        resizedStream.Position = 0;

        return resizedStream;
    }

    /// <summary>
    /// Checks if a file has an image extension
    /// </summary>
    /// <param name="fileName">The file name to check</param>
    /// <returns>True if the file has an image extension</returns>
    public static bool IsImageFile(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext == ".jpg" || ext == ".jpeg" || ext == ".png" ||
               ext == ".gif" || ext == ".bmp" || ext == ".webp";
    }
}