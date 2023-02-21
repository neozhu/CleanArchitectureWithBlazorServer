// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class MailService : IMailService
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<MailService> _logger;
    private const string TemplatePath = "Blazor.Server.UI.Resources.EmailTemplates.{0}.cshtml";
    public MailService(IFluentEmail fluentEmail, ILogger<MailService> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public Task<SendResponse> SendAsync(string to, string subject, string body)
    {
        try
        {
            return _fluentEmail
                .To(to)
                .Subject(subject)
                .Body(body, true)
                .SendAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error sending {to} with subject {subject}", to, subject);
            throw e;
        }
    }
    public Task<SendResponse> SendAsync(string to, string subject, string template, object model)
    {
        try
        {
      
            return _fluentEmail
                .To(to)
                .Subject(subject)
                .UsingTemplateFromEmbedded(string.Format(TemplatePath, template), model, Assembly.GetEntryAssembly())
                .SendAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error sending {to} with subject {subject}", to, subject);
            throw e;
        }
    }
}
