// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Service for analyzing user login security risks
/// </summary>
public interface ISecurityAnalysisService
{
    /// <summary>
    /// Analyzes security risks for a user based on their recent login audit and creates/updates UserLoginRiskSummary
    /// </summary>
    /// <param name="loginAudit">The current login audit to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task AnalyzeUserSecurityAsync(LoginAudit loginAudit, CancellationToken cancellationToken = default);
} 
