using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IPatientRepository> _patientRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _patientRepository = new Lazy<IPatientRepository>(() =>new PatientRepository(_repositoryContext));
        }

        public IPatientRepository Patient => _patientRepository.Value;

        public async Task saveAsyc()
        {
           await _repositoryContext.SaveChangesAsync();
        }
        public void Save()
        {
            _repositoryContext.SaveChanges();
        }
    }
}
