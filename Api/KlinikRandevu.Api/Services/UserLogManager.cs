using Entities.Data_Transfer_Objects.Parametre;
using Entities.Exeptions.CustomExceptions;
using Repositories.Contracts;
using Repositories.EFCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserLogManager:IUserLogService
    {
        private const int MaksimumKayit = 1000;
        private readonly IRepositoryManager _repositoryManager;

        public UserLogManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }

        public async Task<List<LogListeDTO>> LoglariGetirAsync(DateTime baslangic, DateTime bitis, string? aksiyonTipi)
        {
            if (baslangic > bitis)
                throw new BadRequestException("Başlangıç tarihi bitişten büyük olamaz.");

            // Bitiş günü dahil olsun diye bir sonraki günün başlangıcına kadar alıyoruz.
            return await _repositoryManager.UserLogRepository.LoglariGetirAsync(
                baslangic.Date, bitis.Date.AddDays(1), aksiyonTipi, MaksimumKayit);
        }

        public async Task<List<string>> AksiyonTipleriniGetirAsync()
        {
            return await _repositoryManager.UserLogRepository.AksiyonTipleriniGetirAsync();
        }
    }
}
