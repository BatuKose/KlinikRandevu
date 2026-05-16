using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IEmailService
    {
        Task MailGonderAsync(string aliciMail, string konu, string htmlIcerik);
        Task RandevuOnayMailiGonder(string aliciMail, string hastaAdi, string doktorAdi, DateTime randevuTarihi);
    }
}
