using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArchitecture.Blazor.Infrastructure.Services.OpenAI;

public sealed class DocumentOcrBackgroundService : BackgroundService
{
    private readonly IDocumentOcrQueue _documentOcrQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly ILogger<DocumentOcrBackgroundService> _logger;

    public DocumentOcrBackgroundService(
        IDocumentOcrQueue documentOcrQueue,
        IServiceScopeFactory scopeFactory,
        IUserContextAccessor userContextAccessor,
        ILogger<DocumentOcrBackgroundService> logger)
    {
        _documentOcrQueue = documentOcrQueue;
        _scopeFactory = scopeFactory;
        _userContextAccessor = userContextAccessor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DocumentOcrRequest request;

            try
            {
                request = await _documentOcrQueue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                using var userContextScope = PushUserContext(request.UserId,request.UserName,request.TenantId);
                await using var scope = _scopeFactory.CreateAsyncScope();
                var ocrJob = scope.ServiceProvider.GetRequiredService<IDocumentOcrJob>();

                await ocrJob.Recognition(request.DocumentId,request.UserName, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Document OCR background processing failed. DocumentId: {DocumentId}",
                    request.DocumentId);
            }
        }
    }

    private IDisposable PushUserContext(string? userId,string? userName,string? tenantId)
    {
        return string.IsNullOrWhiteSpace(userId)
            ? EmptyScope.Instance
            : _userContextAccessor.Push(new UserContext(userId, userName??string.Empty, TenantId:tenantId??string.Empty));
    }

    private sealed class EmptyScope : IDisposable
    {
        public static readonly EmptyScope Instance = new();

        public void Dispose()
        {
        }
    }
}
