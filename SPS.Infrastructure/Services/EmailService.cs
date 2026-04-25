using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPS.Application.Common;
using SPS.Infrastructure.Services.SPS.Infrastructure.Options;
using System.Net;
using System.Net.Mail;

namespace SPS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpOptions> options, ILogger<EmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            using var smtp = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };
            using var mail = new MailMessage(_options.FromAddress, message.To, message.Subject, message.Body)
            {
                IsBodyHtml = message.IsHtml
            };
            await smtp.SendMailAsync(mail, cancellationToken);
        }
    }

    // Options
    namespace SPS.Infrastructure.Options
    {
        public class SmtpOptions
        {
            public const string SectionName = "SmtpSettings";
            public string Host { get; set; } = string.Empty;
            public int Port { get; set; } = 587;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FromAddress { get; set; } = string.Empty;
            public string FromName { get; set; } = string.Empty;
            public bool EnableSsl { get; set; } = true;
        }
    }
}
