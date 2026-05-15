using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IPatientService> _patientService;
        private readonly Lazy<IMuayeneService> _MuayeneService;
        private readonly Lazy<ISistemParametreService> _sistemParametreService;
        private readonly Lazy<IEmailService> _mailService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<MuayeneManager> muayeneLogger)
        {
            _mailService = new Lazy<IEmailService>(() =>
                new EmailManager(repositoryManager, configuration));

            _patientService = new Lazy<IPatientService>(() =>
                new PatientManager(repositoryManager));
            _MuayeneService = new Lazy<IMuayeneService>(() =>
                new MuayeneManager(repositoryManager, muayeneLogger, _mailService.Value));

            _sistemParametreService = new Lazy<ISistemParametreService>(() =>
                new SistemParametreServiceManager(repositoryManager, cache));
        }

        public IPatientService PatientService => _patientService.Value;
        public IMuayeneService MuayeneService => _MuayeneService.Value;
        public ISistemParametreService SistemParametreService => _sistemParametreService.Value;
        public IEmailService EmailService => _mailService.Value;
    }
}