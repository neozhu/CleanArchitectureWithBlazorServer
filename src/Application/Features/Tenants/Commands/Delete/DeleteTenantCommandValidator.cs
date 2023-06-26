// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator()
    {
        RuleFor(v => v.Id).NotNull().ForEach(v => v.NotEmpty());
    }
}