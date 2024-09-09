using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Server.UI.Services.Notifications;

public class InMemoryNotificationService : INotificationService
{
    private const string LocalStorageKey = "__notficationTimestamp";
    private readonly ProtectedLocalStorage _localStorageService;
    private readonly ILogger<InMemoryNotificationService> _logger;

    private readonly List<NotificationMessage> _messages;

    public InMemoryNotificationService(ProtectedLocalStorage localStorageService,
        ILogger<InMemoryNotificationService> logger)
    {
        _localStorageService = localStorageService;
        _logger = logger;
        _messages = new List<NotificationMessage>();
    }

    public async Task<bool> AreNewNotificationsAvailable()
    {
        var timestamp = await GetLastReadTimestamp().ConfigureAwait(false);
        var entriesFound = _messages.Any(x => x.PublishDate > timestamp);

        return entriesFound;
    }

    public async Task MarkNotificationsAsRead()
    {
        await _localStorageService.SetAsync(LocalStorageKey, DateTime.UtcNow.Date).ConfigureAwait(false);
    }

    public async Task MarkNotificationsAsRead(string id)
    {
        var message = await GetMessageById(id);
        if (message == null) return;

        var timestamp = await _localStorageService.GetAsync<DateTime>(LocalStorageKey).ConfigureAwait(false);
        if (timestamp.Success) await _localStorageService.SetAsync(LocalStorageKey, message.PublishDate).ConfigureAwait(false);
    }

    public Task<NotificationMessage> GetMessageById(string id)
    {
        return Task.FromResult(_messages.First(x => x.Id == id));
    }

    public async Task<IDictionary<NotificationMessage, bool>> GetNotifications()
    {
        var lastReadTimestamp = await GetLastReadTimestamp().ConfigureAwait(false);
        var items = _messages.ToDictionary(x => x, x => lastReadTimestamp > x.PublishDate);
        return items;
    }

    public Task AddNotification(NotificationMessage message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    private async Task<DateTime> GetLastReadTimestamp()
    {
        try
        {
            if ((await _localStorageService.GetAsync<DateTime>(LocalStorageKey)).Success == false)
                return DateTime.MinValue;

            var timestamp = await _localStorageService.GetAsync<DateTime>(LocalStorageKey).ConfigureAwait(false);
            return timestamp.Value;
        }
        catch (CryptographicException)
        {
            await _localStorageService.DeleteAsync(LocalStorageKey).ConfigureAwait(false);
            return DateTime.MinValue;
        }
    }


    public void Preload()
    {
        _messages.Add(new NotificationMessage(
            "readme",
            "Blazor Application is ready",
            "We are paving the way for the future of Blazor",
            "Announcement",
            new DateTime(2022, 01, 13),
            "https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/dotnet.yml/badge.svg",
            new[]
            {
                new NotificationAuthor("Hualin",
                    "https://avatars.githubusercontent.com/u/1549611?s=48&v=4")
            }, typeof(NotificationMessage)));
    }
}