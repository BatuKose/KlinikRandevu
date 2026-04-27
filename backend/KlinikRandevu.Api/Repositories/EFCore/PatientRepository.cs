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

        public async Task<int> GetMaxProtokol()
        {
            var maxProtokol = await _repositoryContext.Patients.OrderByDescending(p => p.Protocol).Select(p=>p.Protocol).FirstOrDefaultAsync();
            return maxProtokol;
        }

        public async Task<bool> PhoneExists(string number)
        {
            var phone = await _repositoryContext.Patients.AnyAsync(p => p.Phone==number);
            return phone;
        }

        public async Task<bool> TcExists(long number)
        {
            var tc = await _repositoryContext.Patients.AnyAsync(t => t.TcKimlik==number);
            return tc;
           
        }
    }
}
