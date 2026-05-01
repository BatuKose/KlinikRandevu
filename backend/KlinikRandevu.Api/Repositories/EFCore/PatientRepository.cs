using Entities.Data_Transfer_Objects.Patient;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<List<GetPatientDTO>> getPatientAsync(string aramaMetni)
        {
            var sqlParams = new[]
            {
                   new SqlParameter("@arama",SqlDbType.NVarChar) {Value=aramaMetni},
                  
            };

            return await _repositoryContext.Patients.FromSqlRaw(@"
                    SELECT TOP 1
                    Name, Surname, Protocol, Address, Phone, BirthDate, Gender, BloodType, TcKimlik
                FROM Patients
                WHERE IsActive = 1
                  AND (
                       CAST(TcKimlik AS NVARCHAR)   LIKE '%' +@arama + '%'
                       OR CAST(Protocol AS NVARCHAR) LIKE '%' +@arama + '%'
                       OR Name     LIKE '%' + @arama + '%'
                       OR Surname  LIKE '%' + @arama + '%'
                       )
                ", sqlParams).Select(p=>new GetPatientDTO
            {
                Name = p.Name,
                Surname = p.Surname,
                Protocol = p.Protocol,
                Address = p.Address,
                Phone = p.Phone,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                BloodType = p.BloodType,
                TcKimlik = p.TcKimlik
            }).ToListAsync();
        }

        public Task<Patient> GetPatientByProtokolASycn(int protokol)
        {
            var result = _repositoryContext.Patients.SingleOrDefaultAsync(p=>p.Protocol==protokol);
            return result;
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
