using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace CleanArchitecture.Blazor.Server.UI.Services.AI;

public sealed class LocalStorageChatHistoryProvider : ChatHistoryProvider
{
    private readonly Func<CancellationToken, ValueTask<IReadOnlyList<ChatMessage>>> _loadHistoryAsync;
    private readonly Func<IReadOnlyList<ChatMessage>, CancellationToken, ValueTask> _saveHistoryAsync;

    public LocalStorageChatHistoryProvider(
        Func<CancellationToken, ValueTask<IReadOnlyList<ChatMessage>>> loadHistoryAsync,
        Func<IReadOnlyList<ChatMessage>, CancellationToken, ValueTask> saveHistoryAsync)
    {
        _loadHistoryAsync = loadHistoryAsync;
        _saveHistoryAsync = saveHistoryAsync;
    }

    protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
        InvokingContext context,
        CancellationToken cancellationToken)
    {
        var history = await _loadHistoryAsync(cancellationToken);
        return history;
    }

    protected override async ValueTask StoreChatHistoryAsync(
        InvokedContext context,
        CancellationToken cancellationToken)
    {
        var history = (await _loadHistoryAsync(cancellationToken)).ToList();
        history.AddRange(context.RequestMessages);

        if (context.ResponseMessages is not null)
        {
            history.AddRange(context.ResponseMessages);
        }

        await _saveHistoryAsync(history, cancellationToken);
    }
}
