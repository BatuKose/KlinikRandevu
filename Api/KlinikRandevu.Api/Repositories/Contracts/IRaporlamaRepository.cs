using Entities.Data_Transfer_Objects.Muayene;
using Entities.Data_Transfer_Objects.Raporlama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRaporlamaRepository
    {
        Task<List<UzamanlikBazliRaporDto?>> UzmanlikBazliHastaBasvuru(DateTime baslangic, DateTime bitis);
        Task<List<TeshisRaporDto>> TeshisRaporu(DateTime baslangic, DateTime bitis);
    }
}
