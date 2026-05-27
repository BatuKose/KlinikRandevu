using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class UserYetkiRepository: IUserYetkiRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UserYetkiRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }
    }
}
