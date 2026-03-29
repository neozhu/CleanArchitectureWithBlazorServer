namespace MediatR
{
    public interface IRequestExceptionHandler<in TRequest, TResponse, in TException>
        where TRequest : notnull
        where TException : Exception
    {
        Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken);
    }

    public class RequestExceptionHandlerState<TResponse>
    {
        public bool Handled { get; private set; }

        public TResponse? Response { get; private set; }

        public void SetHandled(TResponse response)
        {
            Handled = true;
            Response = response;
        }
    }
}
