// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using DocumentFormat.OpenXml.Math;

namespace CleanArchitecture.Blazor.Application.Constants.User;

public abstract class UserName
{
    public const string Administrator = nameof(Administrator);
    public const string Demo = nameof(Demo);
    public const string Users = nameof(Users);
    public const string DefaultPassword = "Password123!";

    public const string DefaultTenant = "Tenant1";
}
