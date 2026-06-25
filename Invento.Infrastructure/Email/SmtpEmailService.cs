using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Invento.Infrastructure.Email
{
    public class SmtpEmailService
        : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(
            IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string htmlBody)
        {
            using var client = new SmtpClient(
                _settings.Host,
                _settings.Port);

            client.EnableSsl = _settings.EnableSsl;

            client.Credentials =
                new NetworkCredential(
                    _settings.Username,
                    _settings.Password);

            using var message = new MailMessage
            {
                From = new MailAddress(
                    _settings.FromEmail,
                    _settings.FromName),

                Subject = subject,

                Body = htmlBody,

                IsBodyHtml = true
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
        }
    }
}