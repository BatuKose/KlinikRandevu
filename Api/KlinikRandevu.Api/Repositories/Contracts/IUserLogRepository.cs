using Entities.Data_Transfer_Objects.Parametre;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IUserLogRepository
    {
        void LoginLogYaz(UserLog log);
        Task<List<LogListeDTO>> LoglariGetirAsync(DateTime baslangic, DateTime bitis, string? aksiyonTipi, int limit);
        Task<List<string>> AksiyonTipleriniGetirAsync();
    }
}
