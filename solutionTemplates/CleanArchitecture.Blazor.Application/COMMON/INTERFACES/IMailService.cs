// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.$safeprojectname$.Settings;

namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Interfaces;

public interface IMailService
{
    Task SendAsync(MailRequest request);
}
