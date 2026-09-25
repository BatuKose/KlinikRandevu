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
        Task<List<HastaTaahütnameListeDTO>> HastaninTaahütnameleriniGetirAsync(int protokol);
        Task<TaahütnameGuncelleDTO> TaahütnameGuncelleAsync(TaahütnameGuncelleDTO model);
        Task TaahütnameIptalAsync(int id);
        Task<OdemeYapDTO> OdemeYap(OdemeYapDTO odeme);
        Task<TedaviEkleDTO> MuayeneyeTedaviEKle(TedaviEkleDTO giris);
        Task<OdemeIptalDTO> OdemeIade(OdemeIptalDTO odemeIptal);
        Task<DoktorIzınOlusturDTO> DoktorIzınOlusturAsync(DoktorIzınOlusturDTO model);
        Task<CalismaPlaniKopyala> CalismaPlaniKopyala(CalismaPlaniKopyala model);
        Task<CalismaPlaniKopyalaBransBazliDTO> BransBazliCalismaPlaniKopyalaAsync(CalismaPlaniKopyalaBransBazliDTO model);
        Task<PoliklinikYesilAlanAyarlarıEkleDto> PoliklinikYesilAlanAyariEkle(PoliklinikYesilAlanAyarlarıEkleDto model);
        Task<YesilListeyeHastaEkleDTO> YesilListeyeHastaEkleAsync(YesilListeyeHastaEkleDTO model);
        List<AktifDoktorlariGetirDTO> AktifDoktorlariGetir();
        List<AktifServisListesiGetirDto> AktifServisleriGetir();
        Task<MuayeneKaydiDetayDTO> MuayeneDetayGetir(int muayeneId);
        Task<MuayeneKaydiDetayDTO> MuayeneDetayRandevuIleGetir(int randevuId);
        Task<List<teshisler>> TeshisleriGetir(int muayeneId);
        Task<List<TedaviKaydi>> TedavileriGetir(int muayeneId);
        Task<List<odeme>> OdemeleriGetir(int muayeneId);
        Task<double> MuayeneBorcGetir(int muayeneId);
        Task<double> MuayeneOdemeToplamGetir(int muayeneId);
        Task<List<PoliklinikHastaListesiDTO>> PoliklinikHastaListesiGetir(int polNo, DateTime baslangic, DateTime bitis);
    }
}
