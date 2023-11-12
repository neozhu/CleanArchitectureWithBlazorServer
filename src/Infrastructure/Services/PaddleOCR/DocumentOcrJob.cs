// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Domain.Common.Enums;

namespace CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;
public class DocumentOcrJob : IDocumentOcrJob
{
    private readonly IApplicationHubWrapper _notificationService;
    private readonly IApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISerializer _serializer;
    private readonly ILogger<DocumentOcrJob> _logger;
    private readonly Stopwatch _timer;

    public DocumentOcrJob(
        IApplicationHubWrapper appNotificationService,
        IApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        ISerializer serializer,
        ILogger<DocumentOcrJob> logger)
    {
        _notificationService = appNotificationService;
        _context = context;
        _httpClientFactory = httpClientFactory;
        _serializer = serializer;
        _logger = logger;
        _timer = new Stopwatch();
    }
    private string ReadBase64String(string path)
    {
        using (Image image = Image.FromFile(path))
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

    }
    public void Do(int id)
    {
        Recognition(id, CancellationToken.None).Wait();
    }
    public async Task Recognition(int id, CancellationToken cancellationToken)
    {
        try
        {
            using (var client = _httpClientFactory.CreateClient("ocr"))
            {
                _timer.Start();
                var doc = await _context.Documents.FindAsync(id);
                if (doc == null) return;
                await _notificationService.JobStarted(doc.Title!);
                DocumentCacheKey.SharedExpiryTokenSource().Cancel();
                if (string.IsNullOrEmpty(doc.URL)) return;
                var imgFile = Path.Combine(Directory.GetCurrentDirectory(), doc.URL);
                if (!File.Exists(imgFile)) return;
                // Create multipart/form-data content
                using var form = new MultipartFormDataContent();
                using var fileStream = new FileStream(imgFile, FileMode.Open);
                using var fileContent = new StreamContent(fileStream);

                form.Add(fileContent, "file", Path.GetFileName(imgFile));  // "image" is the form parameter name for the file

                var response = await client.PostAsync("", form);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var ocrResult = JsonSerializer.Deserialize<OcrResult>(result);
                    doc.Status = JobStatus.Done;

                    if (ocrResult is not null)
                    {
                        var content = string.Join(',', ocrResult.data);
                        doc.Description = $"recognize the result: success";
                        doc.Content = content;
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    await _notificationService.JobCompleted(doc.Title!);
                    DocumentCacheKey.SharedExpiryTokenSource().Cancel();
                    _timer.Stop();
                    var elapsedMilliseconds = _timer.ElapsedMilliseconds;
                    _logger.LogInformation("Id: {Id}, elapsed: {ElapsedMilliseconds}, recognize the result: {@Result},{@Content}", id, elapsedMilliseconds, ocrResult, doc.Content);

                }

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Id}: recognize error {ExMessage}", id, ex.Message);
        }

    }

}
#pragma warning disable CS8981
internal class OcrResult
{
    public string[] data { get; set; } = Array.Empty<string>();
}
#pragma warning restore CS8981

