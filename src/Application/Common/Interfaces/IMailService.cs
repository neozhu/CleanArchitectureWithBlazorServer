// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentEmail.Core.Models;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IMailService
{
    Task<SendResponse> SendAsync(string to, string subject, string body);
    Task<SendResponse> SendAsync(string to, string subject, string template, object model);
}