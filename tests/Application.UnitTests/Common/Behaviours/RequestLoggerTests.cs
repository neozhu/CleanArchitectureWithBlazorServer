using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private readonly Mock<IUserContextAccessor> _userContextAccessor;
    private readonly Mock<IIdentityService> _identityService;
    private readonly Mock<ILogger<AddEditProductCommand>> _logger;

    public RequestLoggerTests()
    {
        _userContextAccessor = new Mock<IUserContextAccessor>();
        _identityService = new Mock<IIdentityService>();
        _logger = new Mock<ILogger<AddEditProductCommand>>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        var userContext = new UserContext("Administrator", "Administrator");
        _userContextAccessor.Setup(x => x.Current).Returns(userContext);
        var requestLogger = new LoggingPreProcessor<AddEditProductCommand>(_logger.Object, _userContextAccessor.Object);
        await requestLogger.Process(
            new AddEditProductCommand { Brand = "Brand", Name = "Brand", Price = 1.0m, Unit = "EA" },
            new CancellationToken());
        _userContextAccessor.Verify(i => i.Current, Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingPreProcessor<AddEditProductCommand>(_logger.Object, _userContextAccessor.Object);
        await requestLogger.Process(
            new AddEditProductCommand { Brand = "Brand", Name = "Brand", Price = 1.0m, Unit = "EA" },
            new CancellationToken());
        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
    }
}
