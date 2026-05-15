using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmailManager : IEmailService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        public EmailManager(IRepositoryManager repositoryManager,IConfiguration configuration)
        {
            _repositoryManager=repositoryManager;
            _configuration = configuration;
        }
        public async Task RandevuOnayMailiGonder(string aliniciMail, string hastaAdi, string doktorAdi, DateTime randevuTarihi)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
            var senderEmail = emailSettings["SenderEmail"];
            var senderName = emailSettings["SenderName"];
            var password = emailSettings["Password"];

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject="Randevu onayı- Klinik randevu",
                Body=$@"
                    <h2>Merhaba {hastaAdi},</h2>
                    <p>Randevunuz başarıyla oluşturulmuştur.</p>
                    <ul>
                        <li><strong>Doktor:</strong> {doktorAdi}</li>
                        <li><strong>Tarih:</strong> {randevuTarihi:dd.MM.yyyy HH:mm}</li>
                    </ul>
                    <p>Geçmiş olsun dileklerimizle.</p>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(aliniciMail);

            using var smtpClient = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };
            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
