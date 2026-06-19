using Entities.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class TatilRepository : ITatilRepository
    {
        private readonly RepositoryContext _repositoryContext;
        public TatilRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public Task<bool> ApiVeriVarmı()
        {
            var result = _repositoryContext.Tatil.AnyAsync(x => x.ApiMi==true);
            return result;
        }

        public void TatilGünleriEkle(Tatil entity)
        {
            _repositoryContext.Add(entity);
        }
        public async Task ApiVerileriniSil()
        {
            await _repositoryContext.Tatil
                .Where(x => x.ApiMi)
                .ExecuteDeleteAsync();  
        }
    }
}
