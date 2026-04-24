// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

namespace CleanArchitecture.Blazor.Infrastructure.Services.OpenAI;

public class DocumentOcrJob : IDocumentOcrJob
{
    private const int MaxContentLength = 4000;
    private const string SystemPrompt = "You are an advanced visual analysis AI. Analyze and describe images based on visual content, providing structured output in Markdown format.";
    private const string UserPrompt = "Analyze the following image and provide a comprehensive, briefly description in Markdown format.";

    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IAISettings _aiSettings;
    private readonly IAppCache _appCache;
    private readonly ILogger<DocumentOcrJob> _logger;
    private readonly IApplicationHubWrapper _hubNotification;

    public DocumentOcrJob(
        IApplicationHubWrapper hubNotification,
        IApplicationDbContextFactory dbContextFactory,
        IAISettings aiSettings,
        IAppCache appCache,
        ILogger<DocumentOcrJob> logger)
    {
        _hubNotification = hubNotification;
        _dbContextFactory = dbContextFactory;
        _aiSettings = aiSettings;
        _appCache = appCache;
        _logger = logger;
    }



    public async Task Recognition(int id,string? userName, CancellationToken cancellationToken)
    {
        await ProcessDocumentAsync(id, userName,cancellationToken);
    }

    private async Task ProcessDocumentAsync(int id,string? userName, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await using var dbContext = await _dbContextFactory.CreateAsync(cancellationToken);

            var document = await dbContext.Documents.FindAsync(new object[] { id }, cancellationToken);
            if (document is null)
            {
                _logger.LogWarning("Document not found. DocumentId: {DocumentId}, User: {@UserName}", id, userName);
                return;
            }

            await _hubNotification.JobStarted(id, document.Title!);
            await InvalidateDocumentCacheAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(document.URL))
            {
                await UpdateDocumentWithError(dbContext, document, "Document URL is missing or invalid.", cancellationToken);
                _logger.LogWarning("Invalid document URL. DocumentId: {DocumentId}", id);
                return;
            }

            var analysisResult = await AnalyzeDocumentImageAsync(document.URL, cancellationToken);

            await UpdateDocumentWithResult(dbContext, document, analysisResult, cancellationToken);
            await _hubNotification.JobCompleted(id, document.Title!);
            await InvalidateDocumentCacheAsync(cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation(
                "Document visual analysis completed. DocumentId: {DocumentId}, Title: {Title}, Duration: {Duration}ms, User: {@UserName}",
                id, document.Title, stopwatch.ElapsedMilliseconds,userName);
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
            var client = new OpenAIClient(_aiSettings.OpenAIApiKey);
            var chatClient = client.GetChatClient(_aiSettings.OpenAIModel);
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

    private Task InvalidateDocumentCacheAsync(CancellationToken cancellationToken)
    {
        return _appCache.RemoveByTagsAsync(DocumentCacheKey.Tags, cancellationToken);
    }
}

