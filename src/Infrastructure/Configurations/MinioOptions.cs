using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

public class MinioOptions
{
    public const string Key = "Minio";
    public string Endpoint { get; set; } = "https://minio.blazors.app:8843";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "files";
}
