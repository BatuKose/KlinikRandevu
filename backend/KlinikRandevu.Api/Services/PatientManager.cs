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
            string phone = dto.Phone.TrimStart('0');
            if (dto is null) throw new NotFoundException("Hasta bilgilerin dolu olması gerekiyor");
            if (dto.BirthDate>DateTime.Now) throw new BadRequestException("Doğum tarihi güncel tarihten büyük olamaz");
            var age = DateTime.Now.Year-dto.BirthDate.Year;
            if (age>120) throw new BadRequestException("Geçerli doğum tarihi giriniz");
            if (dto.Name.Any(char.IsDigit)) throw new BadRequestException("İsim rakam içeremez");
            if(dto.Surname.Any(char.IsDigit)) throw new BadRequestException("Soyisim rakam içeremez");
            if(!dto.Phone.Any(char.IsDigit)) throw new BadRequestException("Telefon numarası karakter içeremez");
            bool phoneExists = await _repositoryManager.Patient.PhoneExists(dto.Phone);
            if (phoneExists) throw new BadRequestException("Telefon numarası sistemde kayıtlıdır");
            var maxProtokol= await _repositoryManager.Patient.GetMaxProtokol();
            if(maxProtokol<=0) maxProtokol=20260;
           
            var yeniProtol = (maxProtokol)+1;
            var patientDto = new Patient
            {
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                BloodType = dto.BloodType,
                IsActive=true,
                Gender=dto.gender,
                Surname = dto.Surname,
                Name = dto.Name,
                Phone=phone,
                Protocol=yeniProtol
            };
            _repositoryManager.Patient.CreatePatient(patientDto);
            await _repositoryManager.saveAsyc();
            return new CreatePatientDto
            {
                Address=patientDto.Address,
                BirthDate=patientDto.BirthDate,
                BloodType = patientDto.BloodType,
                gender = patientDto.Gender,
                Surname = patientDto.Surname,
                Name = patientDto.Name,
                Phone=patientDto.Phone
            };
            
        }
    }
}
