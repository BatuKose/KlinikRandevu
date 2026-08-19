using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class EmailManager : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailManager> _logger;
        public EmailManager( IConfiguration configuration,ILogger<EmailManager> logger)
        {
            _configuration = configuration;
            _logger=logger;
        }

        public async Task MailGonderAsync(string aliciMail, string konu, string htmlIcerik)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = int.Parse(emailSettings["Port"] ?? throw new InvalidOperationException("SmtpServer ayarı bulunamadı"));
                var senderEmail = emailSettings["SenderEmail"];
                var senderName = emailSettings["SenderName"];
                var password = emailSettings["Password"];

                var mesaj = new MimeMessage();
                mesaj.From.Add(new MailboxAddress(senderName, senderEmail));
                mesaj.To.Add(MailboxAddress.Parse(aliciMail));
                mesaj.Subject = konu;
                mesaj.Body = new BodyBuilder { HtmlBody = htmlIcerik }.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, password);
                await smtp.SendAsync(mesaj);
                await smtp.DisconnectAsync(true);
            }
            catch(Exception ex)
            {
                _logger.LogWarning("**********Mail Gönderilemedi Hataları kontrol ediniz****************");
                _logger.LogWarning(ex.ToString());
            }

            
        }

        public async Task RandevuOnayMailiGonder(string aliciMail, string hastaAdi, string doktorAdi, DateTime randevuTarihi)
        {
            var konu = "Randevu Onayı - Klinik Randevu";
            var htmlIcerik = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;'>
                    <h2>Merhaba {hastaAdi},</h2>
                    <p>Randevunuz başarıyla oluşturulmuştur.</p>
                    <ul>
                        <li><strong>Doktor:</strong> {doktorAdi}</li>
                        <li><strong>Tarih:</strong> {randevuTarihi:dd.MM.yyyy HH:mm}</li>
                    </ul>
                    <p>Geçmiş olsun dileklerimizle.</p>
                </div>";

            await MailGonderAsync(aliciMail, konu, htmlIcerik);
        }
    }
}