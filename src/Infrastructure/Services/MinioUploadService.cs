using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
public class MinioUploadService : IUploadService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly string _endpoint;
    public MinioUploadService(MinioOptions options)
    {
        var opt = options;
        _endpoint = opt.Endpoint;
        _minioClient = new MinioClient()
                            .WithEndpoint(_endpoint)
                            .WithCredentials(opt.AccessKey, opt.SecretKey)
                            .WithSSL()
                            .Build();
        _bucketName = opt.BucketName;
    }

    public async Task<string> UploadAsync(UploadRequest request)
    {
        // Use FileExtensionContentTypeProvider to determine the MIME type.
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(request.FileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        // Define common bitmap image extensions (not including vector formats like SVG).
        var bitmapImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        var ext = Path.GetExtension(request.FileName).ToLowerInvariant();

        // If ResizeOptions is provided and the file is a bitmap image, process the image.
        if (request.ResizeOptions != null && Array.Exists(bitmapImageExtensions, e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)))
        {
            using var inputStream = new MemoryStream(request.Data);
            using var outputStream = new MemoryStream();
            using var image = Image.Load(inputStream);
            image.Mutate(x => x.Resize(request.ResizeOptions));
            // Convert the image to PNG format.
            image.Save(outputStream, new PngEncoder());
            request.Data = outputStream.ToArray();
            contentType = "image/png";
        }

        // Determine if the file should be previewed inline.
        // Define a set of extensions that can be previewed in the browser.
        var previewExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg" };
        // If the file extension is in the preview list, use "inline"; otherwise, "attachment".
        string disposition = Array.Exists(previewExtensions, e => e.Equals(ext, StringComparison.OrdinalIgnoreCase))
                             ? "inline"
                             : "attachment";

        // Set the Content-Disposition header, including the filename.
        var headers = new Dictionary<string, string>
        {
            { "Content-Disposition", $"{disposition}; filename=\"{request.FileName}\"" }
        };

        // Ensure the bucket exists.
        bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }

        // Build folder path based on UploadType and optional Folder property.
        string folderPath = $"{request.UploadType.GetDescription()}";
        if (!string.IsNullOrWhiteSpace(request.Folder))
        {
            folderPath = $"{folderPath}/{request.Folder.Trim('/')}";
        }
                                         
        // Construct the object name including the folder path.
        string objectName = $"{folderPath}/{request.FileName}";

        using (var stream = new MemoryStream(request.Data))
        {
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType)
            );
        }

        // Return the URL constructed using the configured Endpoint.
        return $"https://{_endpoint}/{_bucketName}/{objectName}";
    }
    public void Remove(string filename)
    {
        // Expected filename format: "bucketName/objectName"
        var parts = filename.Split('/', 2);
        if (parts.Length < 2)
            throw new ArgumentException("Filename must be in the format 'bucketName/objectName'.");

        string bucket = parts[0];
        string objectName = parts[1];

        try
        {
            _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)).Wait();
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting object", ex);
        }
    }

    
}
