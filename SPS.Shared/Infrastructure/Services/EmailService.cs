using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPS.Shared.Abstractions;
using SPS.Shared.Options;
using System.Net;
using System.Net.Mail;

namespace SPS.Shared.Infrastructure.Services;


public class EmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (string.IsNullOrWhiteSpace(message.To))
            throw new ArgumentException("Recipient address is required", nameof(message.To));

        using var smtpClient = CreateSmtpClient();
        using var mailMessage = CreateMailMessage(message);

        try
        {
            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent to {Recipient} with subject '{Subject}'", message.To, message.Subject);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Email sending to {Recipient} was cancelled", message.To);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", message.To);
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Timeout = _options.TimeoutSeconds * 1000,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_options.Username, _options.Password)
        };

        return client;
    }

    private MailMessage CreateMailMessage(EmailMessage message)
    {
        var mail = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        mail.To.Add(message.To);

 
        if (message.Attachments?.Count > 0)
        {
            foreach (var attachment in message.Attachments)
            {
                var mailAttachment = new Attachment(
                    new MemoryStream(attachment.Data),
                    attachment.FileName,
                    attachment.ContentType);
                mail.Attachments.Add(mailAttachment);
            }
        }

        return mail;
    }
}