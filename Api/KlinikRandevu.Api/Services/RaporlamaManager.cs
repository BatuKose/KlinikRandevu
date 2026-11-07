using Entities.Data_Transfer_Objects.Muayene;
using Entities.Data_Transfer_Objects.Raporlama;
using Entities.Exeptions.CustomExceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class RaporlamaManager : IRaporlamaService
    {
        private static readonly DateTime MinTarih = new DateTime(1970, 1, 1);
        private static readonly DateTime MaxTarih = new DateTime(2099, 12, 31);

        private readonly IRepositoryManager _repositoryManager;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RaporlamaManager> _logger;

        public RaporlamaManager(IRepositoryManager repositoryManager, IMemoryCache cache, ILogger<RaporlamaManager> logger)
        {
            _repositoryManager = repositoryManager;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<UzamanlikBazliRaporDto?>> UzmanlikBazliHastaBasvuru(DateTime baslangic, DateTime bitis)
        {
            TarihAraligiKontrol(baslangic, bitis);

            var data= await _repositoryManager.RaporlamaRepository.UzmanlikBazliHastaBasvuru(baslangic, bitis);
            
            return data;
        }

        public async Task<List<TeshisRaporDto>> TeshisRaporu(DateTime baslangic, DateTime bitis)
        {
            TarihAraligiKontrol(baslangic, bitis);

            return await _repositoryManager.RaporlamaRepository.TeshisRaporu(baslangic, bitis);
        }

       
        private static void TarihAraligiKontrol(DateTime baslangic, DateTime bitis)
        {
            if (baslangic < MinTarih || bitis > MaxTarih)
                throw new BadRequestException("Hatalı tarih seçimi");

            if (baslangic > bitis)
                throw new BadRequestException("Başlangıç tarihi bitiş tarihinden büyük olamaz");

            if ((bitis - baslangic).TotalDays > 366)
                throw new BadRequestException("En fazla 1 yıllık rapor alınabilir");
        }
    }
}