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
    }
}
