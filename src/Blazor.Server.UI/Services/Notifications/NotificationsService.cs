using Blazored.LocalStorage;

namespace Blazor.Server.UI.Services.Notifications;

public class InMemoryNotificationService : INotificationService
{
    private readonly ILogger<InMemoryNotificationService> _logger;
    private readonly List<NotificationMessage> _messages;
    private DateTime ___notficationTimestamp = DateTime.MinValue;

    public InMemoryNotificationService(
        ILogger<InMemoryNotificationService> logger)
    {
        _logger = logger;
        _messages = new List<NotificationMessage>();
        Preload();
    }

    private async Task<DateTime> GetLastReadTimestamp()
    {
        if (_messages.Count==0)
        {
            return await Task.FromResult(DateTime.MinValue);
        }
        else
        {
            var timestamp = ___notficationTimestamp;
            return await Task.FromResult(timestamp);
        }
    }

    public async Task<bool> AreNewNotificationsAvailable()
    {
        var timestamp = await GetLastReadTimestamp();
        var entriesFound = _messages.Any(x => x.PublishDate > timestamp);

        return entriesFound;
    }

    public  Task MarkNotificationsAsRead()
    {
        ___notficationTimestamp = DateTime.Now;
        return Task.CompletedTask;
    }

    public async Task MarkNotificationsAsRead(string id)
    {
        var message = await GetMessageById(id);
        if (message == null) { return; }

        var timestamp = ___notficationTimestamp;
        if (message.PublishDate > timestamp)
        {
            ___notficationTimestamp = message.PublishDate;
        }

    }

    public Task<NotificationMessage> GetMessageById(string id) =>
        Task.FromResult(_messages.FirstOrDefault((x => x.Id == id)));

    public async Task<IDictionary<NotificationMessage, bool>> GetNotifications()
    {
        var lastReadTimestamp = await GetLastReadTimestamp();
        var items = _messages.ToDictionary(x => x, x => lastReadTimestamp > x.PublishDate);
        return items;
    }

    public Task AddNotification(NotificationMessage message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }


    public void Preload()
    {
        _messages.Add(new NotificationMessage(
            "mudblazor-here-to-stay",
            "MudBlazor is here to stay",
            "We are paving the way for the future of Blazor",
            "Announcement",
            new DateTime(2022, 01, 13),
            "_content/MudBlazor.Docs/images/announcements/mudblazor_heretostay.png",
            new[]
            {
                new NotificationAuthor("Jonny Larsson",
                    "https://avatars.githubusercontent.com/u/10367109?v=4")
            }, typeof(NotificationMessage)));
    }
}