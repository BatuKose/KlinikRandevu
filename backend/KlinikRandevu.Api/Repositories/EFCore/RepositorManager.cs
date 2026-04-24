using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class RepositorManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly IPatientRepository _patientRepository;
        public RepositorManager(RepositoryContext repositoryContext,IPatientRepository patient)
        {
            _repositoryContext=repositoryContext;
            _patientRepository = new Lazy<IPatientRepository>(() => new PatientRepository(_repositoryContext));
        }
        public IPatientRepository Patient => _patientRepository;

    }
}
