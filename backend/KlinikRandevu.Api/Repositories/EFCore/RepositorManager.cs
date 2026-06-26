using Microsoft.AspNetCore.Components.Forms;
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
        private readonly Lazy<IMuayeneRepository> _MuayeneRepository;
        private readonly Lazy<ISistemParametresiRepository> _sistemParametresiRepository;
        private readonly Lazy<IAuthenticationRepository> _authenticationRepository;
        private readonly Lazy<IUserLogRepository> _userLogRepository;
        private readonly Lazy<IUserYetkiRepository> _userYetkiRepository;
        private readonly Lazy<ITatilRepository> _tatilRepository;
        private readonly Lazy<IIcdApiRepository> _IcdApiRepository;
        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;

            _patientRepository = new Lazy<IPatientRepository>(() => new PatientRepository(_repositoryContext));

            _MuayeneRepository = new Lazy<IMuayeneRepository>(() => new MuayeneRepository(_repositoryContext));

            _sistemParametresiRepository = new Lazy<ISistemParametresiRepository>(() => new SistemParametresiRepository(_repositoryContext));

            _authenticationRepository = new Lazy<IAuthenticationRepository>(() => new AuthenticationRepository(_repositoryContext));

            _userLogRepository = new Lazy<IUserLogRepository>(() => new UserLogRepository(_repositoryContext));

            _userYetkiRepository = new Lazy<IUserYetkiRepository>(() => new UserYetkiRepository(_repositoryContext));
            _tatilRepository = new Lazy<ITatilRepository>(() => new TatilRepository(_repositoryContext));
            _IcdApiRepository= new Lazy<IIcdApiRepository>(() => new IcdApiRepository(_repositoryContext));
        }

        public IPatientRepository Patient => _patientRepository.Value;

        public IMuayeneRepository Muayene => _MuayeneRepository.Value;

        public async Task saveAsyc()
        {
           await _repositoryContext.SaveChangesAsync();
        }
        public void Save()
        {
            _repositoryContext.SaveChanges();
        }
        public ISistemParametresiRepository SistemParametresi => _sistemParametresiRepository.Value;
        public IAuthenticationRepository Authentication => _authenticationRepository.Value;

        public IUserLogRepository UserLogRepository=> _userLogRepository.Value;
        public IUserYetkiRepository UserYetkiRepository=> _userYetkiRepository.Value;
        public ITatilRepository TatilRepository => _tatilRepository.Value;
        public IIcdApiRepository IcdApiRepository=>_IcdApiRepository.Value;
    }
}
