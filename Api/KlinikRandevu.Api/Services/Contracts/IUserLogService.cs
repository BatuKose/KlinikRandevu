using Entities.Data_Transfer_Objects.Parametre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUserLogService
    {
        Task<List<LogListeDTO>> LoglariGetirAsync(DateTime baslangic, DateTime bitis, string? aksiyonTipi);
        Task<List<string>> AksiyonTipleriniGetirAsync();
    }
}
