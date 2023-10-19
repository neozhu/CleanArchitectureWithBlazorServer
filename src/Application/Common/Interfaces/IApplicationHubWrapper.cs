namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

// TODO: can be improved or removed using MediatR?
public interface IApplicationHubWrapper
{
    Task JobStarted(string message);
    Task JobCompleted(string message);
}
