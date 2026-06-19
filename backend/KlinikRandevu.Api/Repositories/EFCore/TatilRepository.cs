using Entities.Models;
using Microsoft.AspNetCore.Http.Features;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class TatilRepository: ITatilRepository
    {
        private readonly RepositoryContext _repositoryContext;
        public TatilRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public void TatilGünleriEkle(Tatil entity)
        {
            _repositoryContext.Add(entity);
        }
    }
}
