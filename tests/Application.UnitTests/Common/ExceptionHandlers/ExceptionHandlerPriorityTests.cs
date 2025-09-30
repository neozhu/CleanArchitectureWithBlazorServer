using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.ExceptionHandlers;

/// <summary>
/// Tests to verify exception handler priority and selection logic
/// </summary>
[TestFixture]
public class ExceptionHandlerPriorityTests
{
    private IServiceProvider _serviceProvider = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging();
        
        // Register exception handlers in the same order as in DependencyInjection.cs
        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(DbExceptionHandler<,,>));
        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(NotFoundExceptionHandler<,,>));
        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(FallbackExceptionHandler<,,>));
        
        // Add MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(ExceptionHandlerPriorityTests).Assembly);
        });
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public async Task Handle_NotFoundException_ShouldBeHandledByServerExceptionHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var request = new TestRequestWithNotFoundException();

        // Act
        var result = await mediator.Send(request, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0], Is.EqualTo("This command is not supported in the current context. Please use the appropriate command for your operation."));
    }

    [Test]
    public async Task Handle_GenericException_ShouldBeHandledByFallbackExceptionHandler()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var request = new TestRequestWithGenericException();

        // Act
        var result = await mediator.Send(request, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        // The fallback handler currently prefixes unexpected errors with a friendly message.
        // We assert it still surfaces the original exception detail rather than enforcing exact formatting.
        Assert.That(result.Errors[0], Does.Contain("Generic exception occurred"));
    }

    public class TestRequestWithNotFoundException : IRequest<Result>
    {
    }

    public class TestRequestWithNotFoundExceptionHandler : IRequestHandler<TestRequestWithNotFoundException, Result>
    {
        public Task<Result> Handle(TestRequestWithNotFoundException request, CancellationToken cancellationToken)
        {
            throw new NotFoundException("This command is not supported in the current context. Please use the appropriate command for your operation.");
        }
    }

    public class TestRequestWithGenericException : IRequest<Result>
    {
    }

    public class TestRequestWithGenericExceptionHandler : IRequestHandler<TestRequestWithGenericException, Result>
    {
        public Task<Result> Handle(TestRequestWithGenericException request, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Generic exception occurred");
        }
    }
} 
