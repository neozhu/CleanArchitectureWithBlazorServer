namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public sealed class RequestExceptionHandlerState<TResponse>
{
    public bool Handled { get; private set; }
    public TResponse Response { get; private set; } = default!;

    public void SetHandled(TResponse response)
    {
        Handled = true;
        Response = response;
    }
}
