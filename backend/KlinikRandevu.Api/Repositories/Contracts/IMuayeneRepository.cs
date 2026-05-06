using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IMuayeneRepository
    {
        public void CalismaPlaniOlustur(DoktorCalismaPlani plan);
        public Task<bool> doktorVarMI(int number);
        public Task<bool> polVarMI(int number);
        public Task<bool> hastaVarmi(long number);
        public Task<bool> PolRandevuMüsaitMi(int number);
        Task<int?> PolMaxSüre(int number);
        Task<int?> PolMaxRanevu(int number);
        public void RandevuOlustur(Randevu randevu);
        public Task<bool> randevuVarmı(int dnumber ,int pnumber, DayOfWeek day);
        Task<bool> randevuSaatiVarmı(int dnumber, int pnumber, TimeSpan baslangic, TimeSpan randevuBitis);
        Task<bool> CakisanRandevuVarMi(int doktorNo, int polNo, DateTime yeniBaslangic, DateTime yeniBitis);
        Task<bool> HastaAyniGunRandevusuVarMi(long hastaTc, int doktorNo, DateTime tarih);
        Task<DoktorCalismaPlani?> CalismaPlaniGetirAsync(int doktorNo, int polNo, DayOfWeek gun,
        TimeSpan baslangic, TimeSpan bitis);
    }
}
