using Entities.Data_Transfer_Objects.Sms;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface ITwilioSmsManager
    {
        Task<bool> SmsGonderAsync(string telefonNumarası, string gonderilcekMesaj);
        Task<int> SmsConfigEkleAsync(InsertSmsConfigDto model);
        Task SmsConfigGuncelleAsync(UpdateSmsConfigDto model);
        Task<bool> SmsConfigAktifPasifAsync(int id);
    }
}
