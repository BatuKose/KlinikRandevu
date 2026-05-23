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
        private readonly Lazy<IAuthService> _authenticationService;
        private readonly Lazy<IUserLogService> _userLogService;
        public ServiceManager(
            IRepositoryManager repositoryManager,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<MuayeneManager> muayeneLogger)
        {
            _mailService = new Lazy<IEmailService>(() =>
                new EmailManager(configuration));

            _patientService = new Lazy<IPatientService>(() =>
                new PatientManager(repositoryManager));
            _MuayeneService = new Lazy<IMuayeneService>(() =>
                new MuayeneManager(repositoryManager, muayeneLogger, _mailService.Value));

            _sistemParametreService = new Lazy<ISistemParametreService>(() =>
                new SistemParametreServiceManager(repositoryManager, cache));
            _authenticationService= new Lazy<IAuthService>(()=>
                new AuthenticationManager(repositoryManager, configuration));
            _userLogService= new Lazy<IUserLogService>(() => new UserLogManager(repositoryManager));
        }

        public IPatientService PatientService => _patientService.Value;
        public IMuayeneService MuayeneService => _MuayeneService.Value;
        public ISistemParametreService SistemParametreService => _sistemParametreService.Value;
        public IEmailService EmailService => _mailService.Value;
        public IAuthService AuthenticationService => _authenticationService.Value;    
        public IUserLogService UserLogService=> _userLogService.Value;
    }
}