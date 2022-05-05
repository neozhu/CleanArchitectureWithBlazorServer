// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MailKit.Net.Smtp;
using MimeKit;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class SMTPMailService : IMailService
{
    public MailSettings _mailSettings { get; }
    public ILogger<SMTPMailService> _logger { get; }

    public SMTPMailService(MailSettings mailSettings, ILogger<SMTPMailService> logger)
    {
        _mailSettings = mailSettings;
        _logger = logger;
    }

    public async Task SendAsync(MailRequest request)
    {
        try
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(request.From ?? _mailSettings.From);
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = request.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, true);
            smtp.Authenticate(_mailSettings.UserName, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }
}
