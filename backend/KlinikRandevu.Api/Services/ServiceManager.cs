using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IPatientService> _patientService;
        private readonly Lazy<IMuayeneService> _MuayeneService;
        private readonly Lazy<ISistemParametreService> _sistemParametreService;

        public ServiceManager(IRepositoryManager repositoryManager, IMemoryCache cache)
        {
            _patientService = new Lazy<IPatientService>(() => new PatientManager(repositoryManager));
            _MuayeneService = new Lazy<IMuayeneService>(() => new MuayeneManager(repositoryManager));
            _sistemParametreService = new Lazy<ISistemParametreService>(() => new SistemParametreServiceManager(repositoryManager, cache));
        }

        public IPatientService PatientService => _patientService.Value;
        public IMuayeneService MuayeneService => _MuayeneService.Value;
        public ISistemParametreService SistemParametreService => _sistemParametreService.Value;
    }
}