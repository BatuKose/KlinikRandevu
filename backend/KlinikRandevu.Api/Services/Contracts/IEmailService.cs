using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IEmailService
    {
        Task RandevuOnayMailiGonder(string aliniciMail,string hastaAdi, string doktorAdi,DateTime randevuTarihi);
    }
}
