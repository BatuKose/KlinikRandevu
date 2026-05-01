using Entities.Data_Transfer_Objects.Patient;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IPatientRepository
    {
        void CreatePatient(Patient patient);
        Task<int> GetMaxProtokol();
        Task<bool> PhoneExists(string number);
        Task<bool> TcExists(long number);
        Task<List<GetPatientDTO>>getPatientAsync(string aramaMetni);
        Task<Patient> GetPatientByProtokolASycn(int protokol);
    }
}
