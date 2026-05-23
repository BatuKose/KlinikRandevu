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
    }
}
