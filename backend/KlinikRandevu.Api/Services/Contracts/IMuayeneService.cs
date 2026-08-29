using Entities.Data_Transfer_Objects.Muayene;
using Entities.Models;
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
        Task<Doctor> DoktoruPasifeAl(int doktor);
        Task<Poliklinik> PoluPasifeAl(int polno);
        Task DoktorGunlukProgramMailiGonderAsync(int doktorNo);
        Task<Randevu> RandevuIptalAsync(int randevuId);
        Task<List<RandevuluHastalarinBilgilerDTO>> RandevuluHastaBilgileriniGetir(
            DateTime basla, DateTime bitis, bool muayeneOldumu);
        Task<teshisler> TeshisEkle(int muayeneId, string teshis);
        Task<int> MuayeneKapat(int id);
        Task<TaahütnameDTO> TaahütnameEkleAsync(TaahütnameEkleDTO taahütname);
        Task<OdemeYapDTO> OdemeYap(OdemeYapDTO odeme);
        Task<TedaviEkleDTO> MuayeneyeTedaviEKle(TedaviEkleDTO giris);
        Task<OdemeIptalDTO> OdemeIade(OdemeIptalDTO odemeIptal);
    }
}
