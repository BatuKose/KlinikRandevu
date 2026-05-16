using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class EmailManager : IEmailService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;

        public EmailManager(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
        }

        public async Task MailGonderAsync(string aliciMail, string konu, string htmlIcerik)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
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