using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
public class MinioFileUploadService : IFileUploadService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly string _endpoint;
    public MinioFileUploadService(MinioOptions options)
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

    public async Task<Result<UploadedFileInfo>> UploadAsync(UploadRequest request)
    {
        try
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

            // Ensure the bucket exists.
            bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
            }

            // Build folder path based on UploadType and optional Folder property.
            string folderPath = $"{request.UploadType.GetDisplayName()}";
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
            string fileUrl = $"https://{_endpoint}/{_bucketName}/{objectName}";
            return Result<UploadedFileInfo>.Success(new UploadedFileInfo
            {
                Name = request.FileName,
                Url = fileUrl,
                Size = request.Data.Length
            });
        }
        catch (Exception ex)
        {
            return Result<UploadedFileInfo>.Failure($"Failed to upload file to Minio: {ex.Message}");
        }
    }
    public async Task RemoveAsync(string uri)
    {
        // Remove the "https://" or "http://" prefix from the URL and extract the bucket and object name.
        Uri fileUri = new Uri(uri);

        // Ensure the URL is well-formed and can be parsed
        if (!fileUri.IsAbsoluteUri)
            throw new ArgumentException("Invalid URL format.");

        // Extract the bucket from the path portion of the URL
        string[] pathParts = fileUri.AbsolutePath.TrimStart('/').Split('/', 2);
        if (pathParts.Length < 2)
            throw new ArgumentException("URL format must be 'https://<endpoint>/<bucket>/<object>'.");

        string bucket = pathParts[0];  
        string objectName = pathParts[1];  

        try
        {
            // Proceed to remove the object from the correct bucket
           await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName));
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting object", ex);
        }
    }


}
