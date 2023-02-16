// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentEmail.Core;
using MailKit.Net.Smtp;
using MimeKit;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class SMTPMailService : IMailService
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<SMTPMailService> _logger;

    public SMTPMailService(IFluentEmail fluentEmail,ILogger<SMTPMailService> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public  Task SendAsync(string to, string subject, string body)
    {
        return _fluentEmail
            .To(to)
            .Subject(subject)
            .Body(body)
            .SendAsync();
    }
}
