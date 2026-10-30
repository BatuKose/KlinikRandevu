using Entities.Data_Transfer_Objects.Email;
using Entities.Exeptions.CustomExceptions;
using Entities.Models;
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
        private readonly IRepositoryManager _repositoryManager;
        public EmailManager( IConfiguration configuration,ILogger<EmailManager> logger, IRepositoryManager repositoryManager)
        {
            _configuration = configuration;
            _logger=logger;
            _repositoryManager=repositoryManager;
        }

        public async Task MailGonderAsync(string aliciMail, string konu, string htmlIcerik)
        {
            try
            {

                var emailConfig = await _repositoryManager.SistemParametresi.GetEmail();
                if (emailConfig != null) 
                {
                    var smtpServer = emailConfig.SmtpServer;
                    var port = emailConfig.Port;
                    var senderEmail = emailConfig.SenderEmail;
                    var senderName = emailConfig.SenderName;
                    var password = emailConfig.Password;

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

        public async Task<List<EmailConfigListeDto>> EmailConfigListeleAsync()
        {
            var liste = await _repositoryManager.SistemParametresi.EmailConfigListele();
            return liste.Select(e => new EmailConfigListeDto
            {
                Id = e.Id,
                SmtpServer = e.SmtpServer,
                Port = e.Port,
                SenderName = e.SenderName,
                SenderEmail = e.SenderEmail,
                Active = e.Active,
                PasswordMaskeli = string.IsNullOrEmpty(e.Password) || e.Password.Length <= 4
                    ? "****"
                    : new string('*', 8) + e.Password[^4..]
            }).ToList();
        }

        public async Task<int> EmailConfigEkleAsync(InsertEmailConfigDto model)
        {
            if (model == null)
                throw new BadRequestException("Email bilgilerini kontrol ediniz");

           
            var yeniConfig = new EmailConfig
            {
                SmtpServer = model.SmtpServer,
                Port = model.Port,
                SenderName = model.SenderName,
                SenderEmail = model.SenderEmail,
                Password = model.Password,
                Active = false
            };

            _repositoryManager.SistemParametresi.EmailConfigEkle(yeniConfig);
            await _repositoryManager.saveAsyc();

            return yeniConfig.Id;
        }

        public async Task EmailConfigGuncelleAsync(UpdateEmailConfigDto model)
        {
            if (model == null)
                throw new BadRequestException("Email bilgilerini kontrol ediniz");

            var config = await _repositoryManager.SistemParametresi.EmailConfigGetirById(model.Id);
            if (config == null)
                throw new NotFoundException("Güncellenecek email ayarı bulunamadı");

            config.SmtpServer = model.SmtpServer;
            config.Port = model.Port;
            config.SenderName = model.SenderName;
            config.SenderEmail = model.SenderEmail;

            if (!string.IsNullOrWhiteSpace(model.Password))
                config.Password = model.Password;

            await _repositoryManager.saveAsyc();
        }

        public async Task<bool> EmailConfigAktifPasifAsync(int id)
        {
            var config = await _repositoryManager.SistemParametresi.EmailConfigGetirById(id);
            if (config == null)
                throw new NotFoundException("Email ayarı bulunamadı");

            if (!config.Active && await _repositoryManager.SistemParametresi.BaskaAktifEmailConfigVarMi(id))
                throw new BadRequestException("Aktif bir email ayarı zaten mevcut. Önce aktif olan ayarı pasife alınız");

            config.Active = !config.Active;
            await _repositoryManager.saveAsyc();

            return config.Active;
        }
    }
}