using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using Blazor.Server.UI.Models.Notification;

namespace Blazor.Server.UI.Services;

public class NotificationsService : INotificationsService
{
    private const string UriRequest = "sample-data/notifications.json";
    private readonly IWebHostEnvironment _environment;

    public NotificationsService( IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<IEnumerable<NotificationModel>> GetNotifications()
    {
        var jsonstring = File.ReadAllText(Path.Combine(_environment.WebRootPath, UriRequest));
        var notifications = JsonSerializer.Deserialize<IEnumerable<NotificationModel>>(jsonstring, new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        });
        return await Task.FromResult(notifications) ?? throw new InvalidOperationException();
    }

    public async Task<IEnumerable<NotificationModel>> GetActiveNotifications()
    {
        var jsonstring = File.ReadAllText(Path.Combine(_environment.WebRootPath, UriRequest));
        var notifications = JsonSerializer.Deserialize<IEnumerable<NotificationModel>>(jsonstring, new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        });
        var activeNotifications = (notifications ?? throw new InvalidOperationException()).Where(n => n.IsActive);
        return await Task.FromResult(activeNotifications) ?? throw new InvalidOperationException();
    }
}