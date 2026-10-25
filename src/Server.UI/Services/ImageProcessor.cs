using SkiaSharp;
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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxHeight);
        if (quality is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(quality));

        sourceStream.Position = 0;
        using var input = new MemoryStream();
        await sourceStream.CopyToAsync(input);
        using var image = SKBitmap.Decode(input.ToArray())
            ?? throw new InvalidDataException("Unable to decode the image.");

        // Resize with center-crop to exact target size (no distortion).
        var scale = Math.Max((float)maxWidth / image.Width, (float)maxHeight / image.Height);
        var width = maxWidth / scale;
        var height = maxHeight / scale;
        var left = (image.Width - width) / 2;
        var top = (image.Height - height) / 2;
        using var resized = new SKBitmap(maxWidth, maxHeight);
        using (var canvas = new SKCanvas(resized))
        using (var source = SKImage.FromBitmap(image))
        {
            canvas.Clear(SKColors.White);
            canvas.DrawImage(source, new SKRect(left, top, left + width, top + height),
                new SKRect(0, 0, maxWidth, maxHeight), new SKSamplingOptions(SKFilterMode.Linear));
        }

        return Encode(resized, SKEncodedImageFormat.Jpeg, quality);
    }

    public static async Task<MemoryStream> ResizeAndCompressToPngAsync(Stream sourceStream)
    {
        sourceStream.Position = 0;
        using var input = new MemoryStream();
        await sourceStream.CopyToAsync(input);
        using var data = SKData.CreateCopy(input.ToArray());
        using var codec = SKCodec.Create(data)
            ?? throw new InvalidDataException("Unable to decode the image.");
        using var image = SKBitmap.Decode(codec)
            ?? throw new InvalidDataException("Unable to decode the image.");

        var scale = image.Width > 1600 ? 0.3m : image.Width > 800 ? 0.5m : 0.9m;
        var swapsDimensions = codec.EncodedOrigin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
            or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var width = swapsDimensions ? image.Height : image.Width;
        var height = swapsDimensions ? image.Width : image.Height;
        var targetWidth = Math.Max(1, Convert.ToInt32(image.Width * scale));
        var targetHeight = Math.Max(1, (int)Math.Round((double)height * targetWidth / width));
        using var resized = new SKBitmap(targetWidth, targetHeight);
        using (var canvas = new SKCanvas(resized))
        using (var source = SKImage.FromBitmap(image))
        {
            canvas.Clear(SKColors.Transparent);
            canvas.Scale((float)targetWidth / width, (float)targetHeight / height);
            // Apply EXIF orientation before resizing, including mirrored orientations.
            var (sx, kx, tx, ky, sy, ty) = codec.EncodedOrigin switch
            {
                SKEncodedOrigin.TopRight => (-1f, 0f, (float)width, 0f, 1f, 0f),
                SKEncodedOrigin.BottomRight => (-1f, 0f, (float)width, 0f, -1f, (float)height),
                SKEncodedOrigin.BottomLeft => (1f, 0f, 0f, 0f, -1f, (float)height),
                SKEncodedOrigin.LeftTop => (0f, 1f, 0f, 1f, 0f, 0f),
                SKEncodedOrigin.RightTop => (0f, -1f, (float)width, 1f, 0f, 0f),
                SKEncodedOrigin.RightBottom => (0f, -1f, (float)width, -1f, 0f, (float)height),
                SKEncodedOrigin.LeftBottom => (0f, 1f, 0f, -1f, 0f, (float)height),
                _ => (1f, 0f, 0f, 0f, 1f, 0f)
            };
            var orientation = new SKMatrix
            {
                ScaleX = sx, SkewX = kx, TransX = tx,
                SkewY = ky, ScaleY = sy, TransY = ty, Persp2 = 1
            };
            canvas.Concat(in orientation);
            canvas.DrawImage(source, new SKRect(0, 0, image.Width, image.Height),
                new SKSamplingOptions(SKFilterMode.Linear));
        }

        return Encode(resized, SKEncodedImageFormat.Png, 100);
    }

    private static MemoryStream Encode(SKBitmap bitmap, SKEncodedImageFormat format, int quality)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(format, quality)
            ?? throw new InvalidDataException("Unable to encode the image.");
        return new MemoryStream(data.ToArray());
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
