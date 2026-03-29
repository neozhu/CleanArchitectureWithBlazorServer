using MediatR;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.MediatorCompatibility;

public class RequestExceptionHandlerStateTests
{
    [Test]
    public void RequestExceptionHandlerState_ShouldStoreHandledResponse()
    {
        _ = new SmokeNotification();
        _ = new SmokeRequest();

        var state = new RequestExceptionHandlerState<string>();

        state.SetHandled("ok");

        Assert.That(state.Handled, Is.True);
        Assert.That(state.Response, Is.EqualTo("ok"));
    }

    private sealed record SmokeNotification : INotification;

    private sealed record SmokeRequest : IRequest<string>;
}
