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
    public class SistemParametresiRepository : ISistemParametresiRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public SistemParametresiRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }
        public async Task<SistemParametresi?> GetirAsync(string parametreAdi)
        {
            return await _repositoryContext.parametreler
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ParametreAdi == parametreAdi && p.Aktif);
        }

        public async Task<List<SistemParametresi>> HepsiniGetirAsync()
        {
            return await _repositoryContext.parametreler
                .AsNoTracking()
                .ToListAsync();
        }

        public void Ekle(SistemParametresi entity)
        {
            _repositoryContext.parametreler.Add(entity);
        }

        public void Guncelle(SistemParametresi entity)
        {
            _repositoryContext.parametreler.Update(entity);
        }
    }
}
