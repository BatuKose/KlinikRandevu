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
        private readonly Lazy<IPoliklinikRepository> _PoliklinikRepository;
        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _patientRepository = new Lazy<IPatientRepository>(() =>new PatientRepository(_repositoryContext));
            _PoliklinikRepository= new Lazy<IPoliklinikRepository>(() => new PoliklinikRepository(_repositoryContext));
        }

        public IPatientRepository Patient => _patientRepository.Value;

        public IPoliklinikRepository Poliklinik => _PoliklinikRepository.Value;

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
