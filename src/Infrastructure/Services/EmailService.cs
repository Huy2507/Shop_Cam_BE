using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Shop_Cam_BE.Application.Common.Interfaces;

namespace Shop_Cam_BE.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _config["Smtp:Host"];
        var portStr = _config["Smtp:Port"];
        var username = _config["Smtp:Username"];
        var password = _config["Smtp:Password"];
        var from = _config["Smtp:From"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portStr) || string.IsNullOrEmpty(from))
        {
            // Log and no-op if SMTP not configured (e.g. dev)
            return;
        }

        using var smtp = new SmtpClient
        {
            Host = host,
            Port = int.Parse(portStr),
            Credentials = !string.IsNullOrEmpty(username) ? new NetworkCredential(username, password) : null,
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(from),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        mailMessage.To.Add(to);

        await smtp.SendMailAsync(mailMessage);
    }
}
