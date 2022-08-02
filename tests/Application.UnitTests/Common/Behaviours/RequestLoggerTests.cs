using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IIdentityService> _identityService;


    public RequestLoggerTests()
    {
        _currentUserService = new Mock<ICurrentUserService>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public  Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _currentUserService.Setup(x => x.UserId()).Returns(Task.FromResult("Administrator"));
        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
       return Task.CompletedTask;
    }

    [Test]
    public  Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        _identityService.Verify(i => i.GetUserNameAsync(null), Times.Never);
        return Task.CompletedTask;
    }
}
