// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class MailRequest
{
    public string? To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? From { get; set; }
}