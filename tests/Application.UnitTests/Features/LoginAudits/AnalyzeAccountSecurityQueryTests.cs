// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.AnalyzeAccountSecurity;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.LoginAudits;

public class AnalyzeAccountSecurityQueryTests
{
    private Mock<IApplicationDbContext> _context;

    [SetUp]
    public void SetUp()
    {
        _context = new Mock<IApplicationDbContext>();
    }

    [Test]
    public async Task Handle_ValidUser_ReturnsSecurityAnalysis()
    {
        // Arrange
        var userId = "test-user-id";
        var userName = "testuser";
        
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = userName
        };

        var loginAudits = new List<LoginAudit>
        {
            new() { Id = 1, UserId = userId, LoginTimeUtc = DateTime.UtcNow.AddDays(-1), 
                   IpAddress = "192.168.1.1", Region = "US", BrowserInfo = "Chrome", Success = true },
            new() { Id = 2, UserId = userId, LoginTimeUtc = DateTime.UtcNow.AddDays(-2), 
                   IpAddress = "192.168.1.2", Region = "CA", BrowserInfo = "Firefox", Success = true },
            new() { Id = 3, UserId = userId, LoginTimeUtc = DateTime.UtcNow.AddDays(-3), 
                   IpAddress = "192.168.1.1", Region = "US", BrowserInfo = "Chrome", Success = false }
        };

        var mockDbSet = MockDbSet(loginAudits);

        _context.Setup(x => x.LoginAudits).Returns(mockDbSet.Object);

        var mockIdentityService = new Mock<IIdentityService>();
        mockIdentityService.Setup(x => x.GetUserNameAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(userName);

        var logger = new Mock<ILogger<AnalyzeAccountSecurityQueryHandler>>();
        var handler = new AnalyzeAccountSecurityQueryHandler(_context.Object, mockIdentityService.Object, logger.Object);

        var query = new AnalyzeAccountSecurityQuery
        {
            UserId = userId,
            AnalysisPeriodDays = 30,
            IncludeFailedLogins = true
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.UserId, Is.EqualTo(userId));
        Assert.That(result.Data.UserName, Is.EqualTo(userName));
        Assert.That(result.Data.AnalysisPeriodDays, Is.EqualTo(30));
    }

    [Test]
    public async Task Handle_NonExistentUser_ReturnsFailure()
    {
        // Arrange
        var userId = "non-existent-user";
        
        var mockIdentityService = new Mock<IIdentityService>();
        mockIdentityService.Setup(x => x.GetUserNameAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(string.Empty);

        var logger = new Mock<ILogger<AnalyzeAccountSecurityQueryHandler>>();
        var handler = new AnalyzeAccountSecurityQueryHandler(_context.Object, mockIdentityService.Object, logger.Object);

        var query = new AnalyzeAccountSecurityQuery
        {
            UserId = userId,
            AnalysisPeriodDays = 30
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task Handle_NoRecentLogins_ReturnsLowRisk()
    {
        // Arrange
        var userId = "test-user-id";
        var userName = "testuser";
        
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = userName
        };

        var mockDbSet = MockDbSet(new List<LoginAudit>());

        _context.Setup(x => x.LoginAudits).Returns(mockDbSet.Object);

        var mockIdentityService = new Mock<IIdentityService>();
        mockIdentityService.Setup(x => x.GetUserNameAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(userName);

        var logger = new Mock<ILogger<AnalyzeAccountSecurityQueryHandler>>();
        var handler = new AnalyzeAccountSecurityQueryHandler(_context.Object, mockIdentityService.Object, logger.Object);

        var query = new AnalyzeAccountSecurityQuery
        {
            UserId = userId,
            AnalysisPeriodDays = 30
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.OverallRiskLevel, Is.EqualTo(SecurityRiskLevel.Low));
        Assert.That(result.Data.RiskScore, Is.EqualTo(0));
        Assert.That(result.Data.ShouldChangePassword, Is.False);
    }

    [Test]
    public void Validator_ValidQuery_PassesValidation()
    {
        // Arrange
        var validator = new AnalyzeAccountSecurityQueryValidator();
        var query = new AnalyzeAccountSecurityQuery
        {
            UserId = "test-user-id",
            AnalysisPeriodDays = 30
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validator_EmptyUserId_FailsValidation()
    {
        // Arrange
        var validator = new AnalyzeAccountSecurityQueryValidator();
        var query = new AnalyzeAccountSecurityQuery
        {
            UserId = string.Empty,
            AnalysisPeriodDays = 30
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(query.UserId)), Is.True);
    }

    [Test]
    public void Validator_InvalidAnalysisPeriod_FailsValidation()
    {
        // Arrange
        var validator = new AnalyzeAccountSecurityQueryValidator();
        var query = new AnalyzeAccountSecurityQuery
        {
            UserId = "test-user-id",
            AnalysisPeriodDays = 0
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(query.AnalysisPeriodDays)), Is.True);
    }

    private static Mock<DbSet<T>> MockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();
        
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        
        return mockDbSet;
    }
} 