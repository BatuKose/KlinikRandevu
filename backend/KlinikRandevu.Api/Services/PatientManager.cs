using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PatientManager: IPatientService
    {
        private readonly IRepositoryManager _repositoryManager;

        public PatientManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }
    }
}
