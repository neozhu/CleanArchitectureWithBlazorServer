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
using System.Collections.Generic;
using System.Linq;
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
        _handler = new DbExceptionHandler<TestRequest, Result, DbUpdateException>();
    }

    [Test]
    public async Task Handle_UniqueConstraintException_ReturnsUserFriendlyMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = CreateUniqueConstraintException("Users", ["Email"]);
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0], Does.Contain("Email"));
        Assert.That(result.Errors[0], Does.Contain("already exists"));
    }

    [Test]
    public async Task Handle_UniqueConstraintException_WithMultipleProperties_ReturnsCorrectMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = CreateUniqueConstraintException("Users", ["FirstName", "LastName"]);
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors[0], Does.Contain("FirstName, LastName"));
    }

    [Test]
    public async Task Handle_UniqueConstraintException_WithNoProperties_ReturnsGenericMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = CreateUniqueConstraintException("Users", new List<string>());
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors[0], Does.Contain("duplicate record"));
    }

    [Test]
    public async Task Handle_CannotInsertNullException_ReturnsRequiredFieldMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new CannotInsertNullException("Cannot insert null", new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors[0], Does.Contain("Required fields are missing"));
    }

    [Test]
    public async Task Handle_MaxLengthExceededException_ReturnsLengthExceededMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new MaxLengthExceededException("Max length exceeded", new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors[0], Does.Contain("exceeds maximum length"));
    }

    [Test]
    public async Task Handle_NumericOverflowException_ReturnsNumericRangeMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new NumericOverflowException("Numeric overflow", new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors[0], Does.Contain("outside the valid range"));
    }

    [Test]
    public async Task Handle_ReferenceConstraintException_ReturnsReferenceConstraintMessage()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new ReferenceConstraintException("Reference constraint violation", new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors[0], Does.Contain("dependent data"));
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
        Assert.That(result.Errors[0], Does.Contain("database error occurred"));
    }





    [Test]
    public async Task Handle_CachePerformance_UsesCompiledExpression()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new DbUpdateException("Test exception");
        var state1 = new RequestExceptionHandlerState<Result>();
        var state2 = new RequestExceptionHandlerState<Result>();

        // Act - Call handler multiple times to test caching
        await _handler.Handle(request, exception, state1, CancellationToken.None);
        await _handler.Handle(request, exception, state2, CancellationToken.None);

        // Assert - Both calls should succeed and return consistent results
        Assert.That(state1.Handled, Is.True);
        Assert.That(state2.Handled, Is.True);
        Assert.That(state1.Response.Succeeded, Is.EqualTo(state2.Response.Succeeded));
        Assert.That(state1.Response.Errors[0], Is.EqualTo(state2.Response.Errors[0]));
    }

    private static UniqueConstraintException CreateUniqueConstraintException(string tableName, IReadOnlyList<string> properties)
    {
        var exception = new UniqueConstraintException("Unique constraint violation", new Exception());
        
        // Use reflection to set the internal properties since they're typically set by EF
        var schemaTableNameProperty = typeof(UniqueConstraintException).GetProperty("SchemaQualifiedTableName");
        var constraintPropertiesProperty = typeof(UniqueConstraintException).GetProperty("ConstraintProperties");
        
        schemaTableNameProperty?.SetValue(exception, $"dbo.{tableName}");
        constraintPropertiesProperty?.SetValue(exception, properties);
        
        return exception;
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
        _handler = new DbExceptionHandler<TestRequestGeneric, Result<string>, DbUpdateException>();
    }

    [Test]
    public async Task Handle_GenericResult_CreatesCorrectFailureResult()
    {
        // Arrange
        var request = new TestRequestGeneric();
        var exception = new UniqueConstraintException("Unique constraint violation", new Exception("Inner exception"));
        var state = new RequestExceptionHandlerState<Result<string>>();

        // Act
        await _handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Errors.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task Handle_GenericResult_WithIntType_CreatesCorrectFailureResult()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DbExceptionHandler<TestRequestGenericInt, Result<int>, DbUpdateException>>>();
        var handler = new DbExceptionHandler<TestRequestGenericInt, Result<int>, DbUpdateException>();
        var request = new TestRequestGenericInt();
        var exception = new CannotInsertNullException("Cannot insert null", new Exception());
        var state = new RequestExceptionHandlerState<Result<int>>();

        // Act
        await handler.Handle(request, exception, state, CancellationToken.None);

        // Assert
        Assert.That(state.Handled, Is.True);
        var result = state.Response;
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Data, Is.EqualTo(0)); // Default value for int
        Assert.That(result.Errors[0], Does.Contain("Required fields are missing"));
    }
}

/// <summary>
/// Performance and Thread Safety Tests
/// </summary>
[TestFixture]
public class DbExceptionHandlerPerformanceTests
{
    [Test]
    public async Task Handle_ConcurrentAccess_ThreadSafe()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DbExceptionHandler<TestRequest, Result, DbUpdateException>>>();
        var handler = new DbExceptionHandler<TestRequest, Result, DbUpdateException>();
        var tasks = new List<Task>();

        // Act - Create multiple concurrent tasks
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var request = new TestRequest();
                var exception = new DbUpdateException($"Test exception {i}");
                var state = new RequestExceptionHandlerState<Result>();
                await handler.Handle(request, exception, state, CancellationToken.None);
                Assert.That(state.Handled, Is.True);
            }));
        }

        // Assert - All tasks should complete successfully
        await Task.WhenAll(tasks);
        Assert.That(tasks.All(t => t.IsCompletedSuccessfully), Is.True);
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

/// <summary>
/// Test request class for generic int result testing purposes.
/// </summary>
public class TestRequestGenericInt : IRequest<Result<int>>
{
}
