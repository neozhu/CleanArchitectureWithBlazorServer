// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Polly;
using Polly.Retry;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class MailService : IMailService
{
    private const string TemplatePath = "CleanArchitecture.Blazor.Server.UI.Resources.EmailTemplates.{0}.cshtml";
    private readonly IApplicationSettings _appConfig;
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<MailService> _logger;
    private readonly AsyncRetryPolicy _policy;

    public MailService(
        IApplicationSettings appConfig,
        IFluentEmail fluentEmail,
        ILogger<MailService> logger)
    {
        _appConfig = appConfig;
        _fluentEmail = fluentEmail;
        _logger = logger;
        _policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2));
    }

    public Task<SendResponse> SendAsync(string to, string subject, string body)
    {
        try
        {
            if (_appConfig.Resilience)
                return _policy.ExecuteAsync(() => _fluentEmail
                    .To(to)
                    .Subject(subject)
                    .Body(body, true)
                    .SendAsync());
            return _fluentEmail
                .To(to)
                .Subject(subject)
                .Body(body, true)
                .SendAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending an email to {Unknown} with subject {Subject}", to, subject);
            throw;
        }
    }

    public Task<SendResponse> SendAsync(string to, string subject, string template, object model)
    {
        try
        {
            if (_appConfig.Resilience)
                return _policy.ExecuteAsync(() => _fluentEmail
                    .To(to)
                    .Subject(subject)
                    .UsingTemplateFromEmbedded(string.Format(TemplatePath, template), model,
                        Assembly.GetEntryAssembly())
                    .SendAsync());
            return _fluentEmail
                .To(to)
                .Subject(subject)
                .UsingTemplateFromEmbedded(string.Format(TemplatePath, template), model, Assembly.GetEntryAssembly())
                .SendAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending an email to {Unknown} with subject {Subject} and template {Template}",
                to, subject, template);
            throw;
        }
    }
}