// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserName { get; }
    string Email { get; }
    string TenantId { get; }
    string TenantName { get; }
    string SuperiorId { get; }
    string SuperiorName { get; }
    string ProfilePictureDataUrl { get; }
    string[] AssignRoles { get; }
}
