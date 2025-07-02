// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.AnalyzeAccountSecurity;

public class AnalyzeAccountSecurityQueryValidator : AbstractValidator<AnalyzeAccountSecurityQuery>
{
    public AnalyzeAccountSecurityQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required")
            .NotNull().WithMessage("User ID cannot be null");

        RuleFor(x => x.AnalysisPeriodDays)
            .GreaterThan(0).WithMessage("Analysis period must be greater than 0 days")
            .LessThanOrEqualTo(365).WithMessage("Analysis period cannot exceed 365 days");
    }
} 