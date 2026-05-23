using Repositories.Contracts;
using Repositories.EFCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserLogManager:IUserLogService
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserLogManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }
    }
}
