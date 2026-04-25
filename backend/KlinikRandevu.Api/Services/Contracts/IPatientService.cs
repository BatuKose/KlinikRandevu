using Entities.Data_Transfer_Objects.Patient;
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
    }
}
