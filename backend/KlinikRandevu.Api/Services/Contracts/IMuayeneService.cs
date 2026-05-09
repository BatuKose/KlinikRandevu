using Entities.Data_Transfer_Objects.Muayene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IMuayeneService
    {
        Task<CalismaPlaniOlusturDTO> CalismaPlaniOlusturAsync(CalismaPlaniOlusturDTO plan);
        Task<RandevuOlusturDTO> RandevuOlusturAsync(RandevuOlusturDTO plan);
        Task<MuayeneKayitiOlusturDTO> MuayeneKayitiOlustur(MuayeneKayitiOlusturDTO muayene);
        Task<List<HastaRandevulariniGetirDTO>> HastaRandevulariniGetir(DateTime baslangic, DateTime bitis);
        Task<List<HastaRandevulariniGetirDTO>> HastanınRandevulariniGetir(int protokol);
    }
}
