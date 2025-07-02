using Microsoft.Extensions.Options;
using Survey.DTOs;
using SurveyProject.Business.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace SurveyProject.Business.Services.Managers
{
    public class EmailManager : IEmailService
    {
        private readonly MailSettingsDto _mailSettings;

        // Ayarları constructor üzerinden DI ile alıyoruz (IOptions<T> kullanarak)
        public EmailManager(IOptions<MailSettingsDto> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // E-posta mesajını oluştur
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_mailSettings.FromMail, "Anket Sistemi"), // Gönderici adı belirtebilirsiniz
                Subject = subject,
                Body = message,
                IsBodyHtml = true, // E-postanın HTML olarak yorumlanmasını sağlar (linkler için önemli)
            };
            mailMessage.To.Add(new MailAddress(toEmail));

            // SMTP istemcisini yapılandır
            using var smtpClient = new SmtpClient
            {
                Host = _mailSettings.SmtpHost,
                Port = _mailSettings.SmtpPort,
                EnableSsl = true, // Güvenli bağlantı (TLS/SSL) için gereklidir
                Credentials = new NetworkCredential(_mailSettings.FromMail, _mailSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            // E-postayı asenkron olarak gönder
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}