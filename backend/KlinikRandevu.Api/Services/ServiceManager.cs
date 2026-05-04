using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManager: IServiceManager
    {
        private readonly Lazy<IPatientService> _patientService;
        private readonly Lazy<IMuayeneService> _MuayeneService; 
        public ServiceManager(IRepositoryManager repositoryManager)
        {
            _patientService= new Lazy<IPatientService>(()=> new PatientManager(repositoryManager));
            _MuayeneService= new Lazy<IMuayeneService>(()=> new MuayeneManager(repositoryManager));
        }
        public IPatientService PatientService => _patientService.Value;

        public IMuayeneService MuayeneService => _MuayeneService.Value;
    }
}
