// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace CleanArchitecture.Blazor.Infrastructure.Services.OpenAI;

public class DocumentOcrJob : IDocumentOcrJob
{
    private const int MaxContentLength = 4000;
    private const string SystemPrompt = "You are an advanced visual analysis AI. Analyze and describe images based on visual content, providing structured output in Markdown format.";
    private const string UserPrompt = "Analyze the following image and provide a comprehensive, briefly description in Markdown format.";

    private readonly IApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<DocumentOcrJob> _logger;
    private readonly IApplicationHubWrapper _hubNotification;

    public DocumentOcrJob(
        IApplicationHubWrapper hubNotification,
        IApplicationDbContext db,
        IConfiguration config,
        ILogger<DocumentOcrJob> logger)
    {
        _hubNotification = hubNotification;
        _db = db;
        _config = config;
        _logger = logger;
    }

    public void Do(int id)
    {
        ProcessDocumentAsync(id, CancellationToken.None).Wait();
    }

    public async Task Recognition(int id, CancellationToken cancellationToken)
    {
        await ProcessDocumentAsync(id, cancellationToken);
    }

    private async Task ProcessDocumentAsync(int id, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
             

            var document = await _db.Documents.FindAsync(new object[] { id }, cancellationToken);
            if (document is null)
            {
                _logger.LogWarning("Document not found. DocumentId: {DocumentId}", id);
                return;
            }

            await _hubNotification.JobStarted(id, document.Title!);
            InvalidateDocumentCache();

            if (string.IsNullOrWhiteSpace(document.URL))
            {
                await UpdateDocumentWithError(_db, document, "Document URL is missing or invalid.", cancellationToken);
                _logger.LogWarning("Invalid document URL. DocumentId: {DocumentId}", id);
                return;
            }

            var analysisResult = await AnalyzeDocumentImageAsync(document.URL, cancellationToken);

            await UpdateDocumentWithResult(_db, document, analysisResult, cancellationToken);
            await _hubNotification.JobCompleted(id, document.Title!);
            InvalidateDocumentCache();

            stopwatch.Stop();
            _logger.LogInformation(
                "Document visual analysis completed. DocumentId: {DocumentId}, Title: {Title}, Duration: {Duration}ms",
                id, document.Title, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await _hubNotification.JobCompleted(id, $"Analysis failed: {ex.Message}");
            _logger.LogError(ex, 
                "Document visual analysis failed. DocumentId: {DocumentId}, Duration: {Duration}ms", 
                id, stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task<string> AnalyzeDocumentImageAsync(string imageUrl, CancellationToken cancellationToken)
    {
        try
        {
            var apiKey = _config["AISettings:OpenAIApiKey"];
            var model = _config["AISettings:OpenAIModel"];
            var client = new OpenAIClient(apiKey);
            var chatClient = client.GetChatClient(model);
            var agent = chatClient.AsAIAgent(instructions: SystemPrompt);

            var message = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, [
                new TextContent(UserPrompt),
                new UriContent(imageUrl, "image/png")
            ]);

            var response = await agent.RunAsync(message, cancellationToken: cancellationToken);
            var result = response.Text ?? string.Empty;

            return result.Length > MaxContentLength 
                ? result[..MaxContentLength] 
                : result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI vision analysis failed. ImageUrl: {ImageUrl}", imageUrl);
            return $"[Analysis Error] {ex.Message}";
        }
    }

    private async Task UpdateDocumentWithResult(
        IApplicationDbContext dbContext,
        Document document,
        string content,
        CancellationToken cancellationToken)
    {
        document.Status = JobStatus.Done;
        document.Description = "Visual analysis completed successfully.";
        document.Content = content;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateDocumentWithError(
        IApplicationDbContext dbContext,
        Document document,
        string errorMessage,
        CancellationToken cancellationToken)
    {
        document.Status = JobStatus.Pending;
        document.Description = $"Analysis failed: {errorMessage}";
        document.Content = string.Empty;

        await dbContext.SaveChangesAsync(cancellationToken);
        await _hubNotification.JobCompleted(document.Id, errorMessage);
    }

    private static void InvalidateDocumentCache()
    {
        DocumentCacheKey.Refresh();
    }
}

