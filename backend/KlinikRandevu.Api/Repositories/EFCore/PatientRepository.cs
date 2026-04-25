using Entities.Models;
using Microsoft.EntityFrameworkCore;
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
            _repositoryContext.Patients.Add(patient);          
        }

        public async Task<Patient?> GetMaxProtokol()
        {
            var maxProtokol = await _repositoryContext.Patients.OrderByDescending(p => p.Protocol).FirstOrDefaultAsync();
            return maxProtokol;
        }

    }
}
