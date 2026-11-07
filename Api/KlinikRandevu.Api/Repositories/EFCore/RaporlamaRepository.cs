using Entities.Data_Transfer_Objects.Raporlama;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class RaporlamaRepository: IRaporlamaRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public RaporlamaRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }
        public async Task<List<UzamanlikBazliRaporDto?>> UzmanlikBazliHastaBasvuru(DateTime baslangic, DateTime bitis)
        {
            var sqlParams = new[]
            {
                  new SqlParameter("@baslangic", SqlDbType.Date) { Value = baslangic },
                  new SqlParameter ("@bitis",SqlDbType.Date) {Value= bitis },
            };
            try
            {
                return await _repositoryContext.Database
                    .SqlQueryRaw<UzamanlikBazliRaporDto?>(@"
                SELECT
                    u.Ad                                                            AS uzmanlikAdi,
                    DATEFROMPARTS(YEAR(m.MuayeneTarihi), MONTH(m.MuayeneTarihi), 1) AS tarih,
                    COUNT(*)                                                        AS muayeneSayisi
                FROM MuayeneKayitlari m
                JOIN Poliklinikler   p ON p.PolNo = m.PolNo
                JOIN UzmanlikDallari u ON u.Kod   = p.PolUzKod
                WHERE m.MuayeneTarihi >= @baslangic
                  AND m.MuayeneTarihi <  DATEADD(DAY, 1, @bitis)
                GROUP BY u.Ad, DATEFROMPARTS(YEAR(m.MuayeneTarihi), MONTH(m.MuayeneTarihi), 1)
                ORDER BY tarih, uzmanlikAdi
            ", sqlParams).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public async Task<List<TeshisRaporDto>> TeshisRaporu(DateTime baslangic, DateTime bitis)
        {
            var sqlParams = new[]
            {
                  new SqlParameter("@baslangic", SqlDbType.Date) { Value = baslangic },
                  new SqlParameter ("@bitis",SqlDbType.Date) {Value= bitis },
            };
            try
            {
                return await _repositoryContext.Database
                    .SqlQueryRaw<TeshisRaporDto>(@"
                SELECT
                    DATEFROMPARTS(YEAR(m.MuayeneTarihi), MONTH(m.MuayeneTarihi), 1) AS tarih,
                    t.teshisKod                                                     AS teshisKod,
                    MAX(t.teshisAd)                                                 AS teshisAd,
                    u.Ad                                                            AS uzmanlikAdi,
                    COUNT(*)                                                        AS teshisSayisi
                FROM Teshisler t
                JOIN MuayeneKayitlari m ON m.Id      = t.muayeneId
                JOIN Poliklinikler   p ON p.PolNo   = m.PolNo
                JOIN UzmanlikDallari u ON u.Kod     = p.PolUzKod
                WHERE m.MuayeneTarihi >= @baslangic
                  AND m.MuayeneTarihi <  DATEADD(DAY, 1, @bitis)
                GROUP BY DATEFROMPARTS(YEAR(m.MuayeneTarihi), MONTH(m.MuayeneTarihi), 1), t.teshisKod, u.Ad
                ORDER BY tarih, teshisKod
            ", sqlParams).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
