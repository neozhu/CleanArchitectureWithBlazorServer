// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Json;
using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using CleanArchitecture.Blazor.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;
public class DocumentOcrJob : IDocumentOcrJob
{
    private readonly IHubContext<SignalRHub, ISignalRHub> _hubContext;
    private readonly IApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISerializer _serializer;
    private readonly ILogger<DocumentOcrJob> _logger;
    private readonly Stopwatch _timer;
    public DocumentOcrJob(
        IHubContext<SignalRHub, ISignalRHub> hubContext,
        IApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        ISerializer serializer,
        ILogger<DocumentOcrJob> logger
        )
    {
        _hubContext = hubContext;
        _context = context;
        _httpClientFactory = httpClientFactory;
        _serializer = serializer;
        _logger = logger;
        _timer=new Stopwatch();
    }
    private string readbase64string(string path)
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
                var doc =await _context.Documents.FindAsync(id);
                if (doc == null) return;
                await _hubContext.Clients.All.Start(doc.Title!);
                DocumentCacheKey.SharedExpiryTokenSource().Cancel();
                if (string.IsNullOrEmpty(doc.URL)) return;
                var imgfile = Path.Combine(Directory.GetCurrentDirectory(), doc.URL);
                if (!File.Exists(imgfile)) return;
                string base64string = readbase64string(imgfile);
               
                var response = client.PostAsJsonAsync<dynamic>("", new { images = new string[] { base64string } }).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var ocr_result = JsonSerializer.Deserialize<ocr_result>(result);
                    var ocr_status = ocr_result!.status;
                    doc.Status = JobStatus.Done;
                    doc.Description = "recognize the result: " + ocr_status;
                    if (ocr_result.status == "000")
                    {
                        var content = _serializer.Serialize(ocr_result.results);
                        doc!.Content = content;

                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    await _hubContext.Clients.All.Completed(doc.Title!);
                    DocumentCacheKey.SharedExpiryTokenSource().Cancel();
                    _timer.Stop();
                    var elapsedMilliseconds = _timer.ElapsedMilliseconds;
                    _logger.LogInformation("Id: {id}, elapsed: {elapsedMilliseconds}, recognize the result: {@result},{@content}", id, elapsedMilliseconds, ocr_result, doc.Content);

                }

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{id}: recognize error {ex.Message}");
        }

    }

}

class result
{
    public decimal confidence { get; set; }
    public string text { get; set; } = String.Empty;
    public List<int[]> text_region { get; set; } = new();
}
class ocr_result
{
    public string msg { get; set; } = String.Empty;
    public List<result[]> results { get; set; } = new();
    public string status { get; set; }=String.Empty;
}


