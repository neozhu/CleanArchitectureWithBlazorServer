// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Domain.Common.Enums;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Gemini;

public class DocumentOcrJob : IDocumentOcrJob
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DocumentOcrJob> _logger;
    private readonly IApplicationHubWrapper _notificationService;
    private readonly Stopwatch _timer;

    public DocumentOcrJob(
        IApplicationHubWrapper appNotificationService,
        IApplicationDbContextFactory dbContextFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<DocumentOcrJob> logger)
    {
        _notificationService = appNotificationService;
        _dbContextFactory = dbContextFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _timer = new Stopwatch();
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
                await using var db = await _dbContextFactory.CreateAsync(cancellationToken);

                var doc = await db.Documents.FindAsync(id);
                if (doc == null)
                {
                    _logger.LogWarning("Document with Id {Id} not found.", id);
                    return;
                }

                await _notificationService.JobStarted(id, doc.Title!);
                CancelCacheToken();

                if (string.IsNullOrEmpty(doc.URL))
                {
                    _logger.LogWarning("Document URL is null or empty for Id {Id}.", id);
                    return;
                }

                byte[] imageBytes;
                string? mimeType;
                using (var imgClient = new HttpClient())
                {
                    var imgResponse = await imgClient.GetAsync(doc.URL, cancellationToken);
                    imgResponse.EnsureSuccessStatusCode();

                    imageBytes = await imgResponse.Content.ReadAsByteArrayAsync();
                    mimeType = imgResponse.Content.Headers.ContentType?.MediaType ?? "image/png";

                }
                string base64Image = Convert.ToBase64String(imageBytes);
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = "Perform OCR, only return the recognized text, output in markdown, keep original structure and layout, no extra explanation, use English." },
                                new
                                {
                                    inline_data = new
                                    {
                                        mime_type = mimeType, 
                                        data = base64Image
                                    }
                                }
                            }
                        }
                    }
                };
                var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("", content);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();

                    using var docJson = System.Text.Json.JsonDocument.Parse(resultJson);
                    // candidates[0].content.parts[0].text
                    var root = docJson.RootElement;
                    string markdownText = "";

                    try
                    {
                        var candidates = root.GetProperty("candidates");
                        if (candidates.GetArrayLength() > 0)
                        {
                            var firstPart = candidates[0].GetProperty("content").GetProperty("parts");
                            if (firstPart.GetArrayLength() > 0)
                            {
                                markdownText = firstPart[0].GetProperty("text").GetString() ?? "";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 可加日志
                        markdownText = $"[Error parsing OCR result] {ex.Message}";
                    }

                    if (markdownText.Length > 4000)
                    {
                        markdownText = markdownText.Substring(0, 4000);
                    }
                    doc.Status = JobStatus.Done;
                    doc.Description = "Recognition result: success";
                    doc.Content = markdownText;

                    await db.SaveChangesAsync(cancellationToken);
                    await _notificationService.JobCompleted(id, doc.Title!);
                    CancelCacheToken();

                    _timer.Stop();
                    _logger.LogInformation(
                        "Image recognition completed successfully {@Document}. Id: {Id}, Elapsed Time: {ElapsedMilliseconds}ms", doc,
                        id, _timer.ElapsedMilliseconds);
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    doc.Status = JobStatus.Pending;
                    doc.Content = result;

                    await db.SaveChangesAsync(cancellationToken);
                    await _notificationService.JobCompleted(id, $"Error: {result}");
                    CancelCacheToken();

                    _logger.LogError("Image recognition failed for Id: {Id}, Status Code: {StatusCode}, Message: {Message}",
                        id, response.StatusCode, result);
                }
            }
        }
        catch (Exception ex)
        {
            await _notificationService.JobCompleted(id, $"Error: {ex.Message}");
            _logger.LogError(ex, "Image recognition error for Id: {Id}, Message: {Message}", id, ex.Message);
        }
        finally
        {
            if (_timer.IsRunning)
            {
                _timer.Stop();
            }
        }
    }

    private void CancelCacheToken()
    {
        DocumentCacheKey.Refresh();
    }
}
 