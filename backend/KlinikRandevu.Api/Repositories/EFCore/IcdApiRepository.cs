using Entities.Models;
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

        public void TokenKayet(IcdApiEntegrasyon ıcdApiEntegrasyon)
        {
            _repositoryContext.IcdApiEntegrasyon.Add(ıcdApiEntegrasyon);
        }
    }
}
