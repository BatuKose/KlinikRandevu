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
    public class IcdApiRepository: IIcdApiRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public IcdApiRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public Task<IcdApiEntegrasyon> DbApiTokenGetir()
        {
           
            var result = _repositoryContext.IcdApiEntegrasyon.SingleOrDefaultAsync(t=>t.Id!=null);
            return result;
        }

        public void TokenKayet(IcdApiEntegrasyon ıcdApiEntegrasyon)
        {
            _repositoryContext.IcdApiEntegrasyon.Add(ıcdApiEntegrasyon);
        }
    }
}
