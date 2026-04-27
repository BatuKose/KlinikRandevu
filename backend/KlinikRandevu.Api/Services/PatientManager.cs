using Entities.Data_Transfer_Objects.Patient;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Exeptions.CustomExceptions;
using Entities.Enums;
namespace Services
{
    public class PatientManager: IPatientService
    {
        private readonly IRepositoryManager _repositoryManager;

        public PatientManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }
        public async Task<CreatePatientDto>CreatePatientAsync(CreatePatientDto dto)
        {
            if (dto is null) throw new BadRequestException("Hasta bilgilerin dolu olması gerekiyor");
            if (dto.BirthDate>DateTime.Now) throw new BadRequestException("Doğum tarihi güncel tarihten büyük olamaz");

            var maxProtokol= await _repositoryManager.Patient.GetMaxProtokol();
            var yeniProtol = (maxProtokol.Protocol)+1;
            var patientDto = new Patient
            {
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                BloodType = dto.BloodType,
                IsActive=true,
                Gender=dto.gender,
                Surname = dto.Surname,
                Name = dto.Name,
                Phone=dto.Phone,
                Protocol=yeniProtol
            };
            _repositoryManager.Patient.CreatePatient(patientDto);
            await _repositoryManager.saveAsyc();
            var result = new CreatePatientDto
            {
                Address=patientDto.Address,
                BirthDate=patientDto.BirthDate,
                BloodType = patientDto.BloodType,
                gender = patientDto.Gender,
                Surname = patientDto.Surname,
                Name = patientDto.Name,
                Phone=patientDto.Phone
            };
            return result;
        }
    }
}
