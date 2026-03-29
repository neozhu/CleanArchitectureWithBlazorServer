namespace MediatR.Pipeline
{
    public interface IRequestPreProcessor<in TRequest>
        where TRequest : notnull
    {
        Task Process(TRequest request, CancellationToken cancellationToken);
    }
}

namespace Mediator.Pipeline
{
    public interface IRequestPreProcessor<in TRequest> : global::MediatR.Pipeline.IRequestPreProcessor<TRequest>
        where TRequest : notnull
    {
    }
}
