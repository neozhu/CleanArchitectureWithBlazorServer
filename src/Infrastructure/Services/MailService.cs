// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using MailKit.Net.Smtp;
using MimeKit;
using Scriban;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class MailService : IMailService
{
    private const string TemplatePath = "Resources/EmailTemplates/{0}.cshtml";
    private readonly SmtpClientOptions _smtpOptions;
    private readonly ILogger<MailService> _logger;

    public MailService(
        SmtpClientOptions smtpOptions,
        ILogger<MailService> logger)
    {
        _smtpOptions = smtpOptions;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpOptions.DefaultFromEmail, _smtpOptions.DefaultFromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, _smtpOptions.UseSsl);

            if (_smtpOptions.RequireCredentials)
            {
                await client.AuthenticateAsync(_smtpOptions.UserName, _smtpOptions.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send email. An exception occurred.");
            throw;
        }
    }

    public async Task SendAsync(string to, string subject, string template, object model)
    {
        try
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), string.Format(TemplatePath, template));
            
            if (!File.Exists(templatePath))
            {
                var errorMessage = $"Template file not found: {templatePath}";
                _logger.LogError(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }

            var templateContent = await File.ReadAllTextAsync(templatePath);
            var scribanTemplate = Template.Parse(templateContent);
            
            if (scribanTemplate.HasErrors)
            {
                var errors = string.Join(", ", scribanTemplate.Messages.Select(m => m.Message));
                _logger.LogError("Template parsing errors: {Errors}", errors);
                throw new InvalidOperationException($"Template parsing errors: {errors}");
            }

            // Convert model properties to lowercase with underscores for Scriban
            var templateModel = ConvertToScribanModel(model);
            var htmlBody = await scribanTemplate.RenderAsync(templateModel);

            await SendAsync(to, subject, htmlBody);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Failed to send templated email, Template: {EmailTemplate}. An exception occurred.",
                template);
            throw;
        }
    }

    private static object ConvertToScribanModel(object model)
    {
        if (model == null)
            return new { };

        var modelType = model.GetType();
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var result = new Dictionary<string, object?>();

        foreach (var property in properties)
        {
            var key = ConvertPropertyName(property.Name);
            var value = property.GetValue(model);
            result[key] = value;
        }

        return result;
    }

    private static string ConvertPropertyName(string propertyName)
    {
        // Convert PascalCase to snake_case
        return propertyName
            .Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLower(c) : char.ToLower(c).ToString())
            .Aggregate(string.Concat);
    }
}
