using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Models;
using EntityFramework.Exceptions.Common;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.ExceptionHandlers;

/// <summary>
/// Unit tests for the DbExceptionHandler class.
/// </summary>
[TestFixture]
public class DbExceptionHandlerTests
{
    private Mock<ILogger<DbExceptionHandler<TestRequest, Result, DbUpdateException>>> _mockLogger = null!;
    private DbExceptionHandler<TestRequest, Result, DbUpdateException> _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<DbExceptionHandler<TestRequest, Result, DbUpdateException>>>();
        _handler = new DbExceptionHandler<TestRequest, Result, DbUpdateException>(_mockLogger.Object);
    }

    [Test]
    public async Task Handle_UniqueConstraintException_ReturnsUserFriendlyMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new UniqueConstraintException("Unique constraint violation", 
            new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("unique constraint"));
    }

    [Test]
    public async Task Handle_CannotInsertNullException_ReturnsRequiredFieldMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new CannotInsertNullException("Cannot insert null", 
            new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("required"));
    }

    [Test]
    public async Task Handle_MaxLengthExceededException_ReturnsLengthExceededMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new MaxLengthExceededException("Max length exceeded", 
            new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("too long"));
    }

    [Test]
    public async Task Handle_NumericOverflowException_ReturnsNumericRangeMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new NumericOverflowException("Numeric overflow", 
            new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("outside the allowed range"));
    }

    [Test]
    public async Task Handle_ReferenceConstraintException_ReturnsReferenceConstraintMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new ReferenceConstraintException("Reference constraint violation", 
            new Exception("Inner exception"), 
            Array.Empty<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry>());
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("linked to other data"));
    }

    [Test]
    public async Task Handle_GenericDbUpdateException_ReturnsGenericMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new DbUpdateException("Generic database error");
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("database error occurred"));
    }

    [Test]
    public async Task Handle_LogsErrorWithCorrectInformation()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new DbUpdateException("Test exception");
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Database update exception occurred")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new DbExceptionHandler<TestRequest, Result, DbUpdateException>(null!));
    }
}

/// <summary>
/// Generic Result Handler Tests for Result&lt;T&gt; scenarios
/// </summary>
[TestFixture]
public class DbExceptionHandlerGenericTests
{
    private Mock<ILogger<DbExceptionHandler<TestRequestGeneric, Result<string>, DbUpdateException>>> _mockLogger = null!;
    private DbExceptionHandler<TestRequestGeneric, Result<string>, DbUpdateException> _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<DbExceptionHandler<TestRequestGeneric, Result<string>, DbUpdateException>>>();
        _handler = new DbExceptionHandler<TestRequestGeneric, Result<string>, DbUpdateException>(_mockLogger.Object);
    }

    [Test]
    public async Task Handle_GenericResult_CreatesCorrectFailureResult()
    {
        // Arrange
        var request = new TestRequestGeneric();
        var exception = new UniqueConstraintException("Unique constraint violation", 
            new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result<string>>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.ErrorMessage, Does.Contain("unique constraint"));
    }
}

/// <summary>
/// Test request class for testing purposes.
/// </summary>
public class TestRequest : IRequest<Result>
{
}

/// <summary>
/// Test request class for generic result testing purposes.
/// </summary>
public class TestRequestGeneric : IRequest<Result<string>>
{
}
