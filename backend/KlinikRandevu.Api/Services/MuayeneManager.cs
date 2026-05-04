using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MuayeneManager:IMuayeneService
    {
        private readonly IRepositoryManager _repositoryManager;

        public MuayeneManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }
    }
}
