using Entities.Data_Transfer_Objects.Parametre;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class UserLogRepository: IUserLogRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UserLogRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public void LoginLogYaz(UserLog log)
        {
            _repositoryContext.userLogs.Add(log);
        }

        public async Task<List<LogListeDTO>> LoglariGetirAsync(DateTime baslangic, DateTime bitis, string? aksiyonTipi, int limit)
        {
            var sorgu = _repositoryContext.userLogs
                .AsNoTracking()
                .Where(l => l.OlusturmaTarihi >= baslangic && l.OlusturmaTarihi < bitis);

            if (!string.IsNullOrWhiteSpace(aksiyonTipi))
                sorgu = sorgu.Where(l => l.AksiyonTipi == aksiyonTipi);

            return await sorgu
                .OrderByDescending(l => l.OlusturmaTarihi)
                .Take(limit)
                .Select(l => new LogListeDTO
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    KullaniciAdi = _repositoryContext.Users
                        .Where(u => u.UserID == l.UserId)
                        .Select(u => u.UserName)
                        .FirstOrDefault(),
                    AksiyonTipi = l.AksiyonTipi,
                    EntityTipi = l.EntityTipi,
                    EntityId = l.EntityId,
                    Detay = l.Detay,
                    OlusturmaTarihi = l.OlusturmaTarihi,
                    IpAdresi = l.IpAdresi
                })
                .ToListAsync();
        }

        public async Task<List<string>> AksiyonTipleriniGetirAsync()
        {
            return await _repositoryContext.userLogs
                .AsNoTracking()
                .Select(l => l.AksiyonTipi)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }
    }
}
