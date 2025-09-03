using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CleanArchitecture.Blazor.Infrastructure.Services;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace Infrastructure.UnitTests;

public class MailServiceTests
{
    private readonly Mock<ILogger<MailService>> _loggerMock;
    private readonly SmtpClientOptions _smtpOptions;

    public MailServiceTests()
    {
        _loggerMock = new Mock<ILogger<MailService>>();
        _smtpOptions = new SmtpClientOptions
        {
            Host = "smtp.test.com",
            Port = 587,
            UseSsl = true,
            UserName = "test@test.com",
            Password = "password"
        };
    }

    [Fact]
    public void SmtpClientOptions_Should_Have_Correct_Default_Values()
    {
        // Arrange
        var options = new SmtpClientOptions();

        // Assert
        Assert.Equal(string.Empty, options.Host);
        Assert.Equal(587, options.Port);
        Assert.True(options.UseSsl);
        Assert.Equal(string.Empty, options.UserName);
        Assert.Equal(string.Empty, options.Password);
    }

    [Fact]
    public void MailService_Constructor_Should_Not_Throw_With_Valid_Options()
    {
        // Arrange & Act & Assert
        var exception = Record.Exception(() => new MailService(_smtpOptions, _loggerMock.Object));
        Assert.Null(exception);
    }

    [Fact]
    public void MailService_Constructor_Should_Not_Throw_With_Null_Parameters()
    {
        // Arrange & Act & Assert - Constructor does not validate parameters
        var exception1 = Record.Exception(() => new MailService(null!, _loggerMock.Object));
        var exception2 = Record.Exception(() => new MailService(_smtpOptions, null!));
        
        // The current implementation doesn't validate constructor parameters
        Assert.Null(exception1);
        Assert.Null(exception2);
    }

    [Fact]
    public async Task SendAsync_Should_Throw_ArgumentNullException_When_ToEmail_Is_Null()
    {
        // Arrange
        var mailService = new MailService(_smtpOptions, _loggerMock.Object);

        // Act & Assert - The actual implementation throws ArgumentNullException from MimeKit
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            mailService.SendAsync(null!, "Test Subject", "Test Body"));
    }
}
