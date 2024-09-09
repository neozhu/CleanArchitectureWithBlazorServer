namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

// TODO: can be improved or removed using MediatR?
public interface IApplicationHubWrapper
{
    Task JobStarted(int id,string message);
    Task JobCompleted(int id,string message);
}