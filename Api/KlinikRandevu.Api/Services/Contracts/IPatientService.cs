using Entities.Data_Transfer_Objects.Patient;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IPatientService
    {
        Task<CreatePatientDto> CreatePatientAsync(CreatePatientDto dto);
        Task<List<GetPatientDTO>> getPatientAsync(string aramaMetni);
        Task<Patient>UpdatePatient(UpdatePatientDTO hasta, int protokol);
        Task<Patient> DeletePatient(int protokol);
    }
}
