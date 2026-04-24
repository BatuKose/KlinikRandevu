using Entities.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class PatientRepository : IPatientRepository
    {
            private readonly RepositoryContext _repositoryContext;

        public PatientRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext=repositoryContext;
        }

        public void CreatePatient(Patient patient)
        {
        }
    }
}
