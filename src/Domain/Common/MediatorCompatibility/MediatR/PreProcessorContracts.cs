namespace MediatR.Pipeline
{
    public interface IRequestPreProcessor<in TRequest>
        where TRequest : notnull
    {
        Task Process(TRequest request, CancellationToken cancellationToken);
    }
}
