namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

// TODO: can be improved or removed as the mediator pipeline evolves?
public interface IApplicationHubWrapper
{
    Task JobStarted(int id,string message);
    Task JobCompleted(int id,string message);
}
