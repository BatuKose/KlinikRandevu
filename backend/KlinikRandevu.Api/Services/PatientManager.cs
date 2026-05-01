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
    public class PatientManager : IPatientService
    {
        private readonly IRepositoryManager _repositoryManager;

        public PatientManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager=repositoryManager;
        }
        public async Task<CreatePatientDto> CreatePatientAsync(CreatePatientDto dto)
        {
            string phone = dto.Phone.TrimStart('0');
            if (dto is null) throw new NotFoundException("Hasta bilgilerin dolu olması gerekiyor");
            if (dto.BirthDate>DateTime.Now) throw new BadRequestException("Doğum tarihi güncel tarihten büyük olamaz");
            var age = DateTime.Now.Year-dto.BirthDate.Year;
            if (age>120) throw new BadRequestException("Geçerli doğum tarihi giriniz");
            if (dto.Name.Any(char.IsDigit)) throw new BadRequestException("İsim rakam içeremez");
            if (dto.Surname.Any(char.IsDigit)) throw new BadRequestException("Soyisim rakam içeremez");
            if (!phone.Any(char.IsDigit)) throw new BadRequestException("Telefon numarası karakter içeremez");
            string tcKontrol = Convert.ToString(dto.TcKimlik);
            if (!tcKontrol.Any(char.IsDigit)) throw new BadRequestException("Tc kimliklik numarası karakter içeremez");
            if (tcKontrol.Length<11) throw new BadRequestException("Tc kimlik numarası 11 haneden küçük olamaz");
            bool phoneExists = await _repositoryManager.Patient.PhoneExists(phone);
            if (phoneExists) throw new BadRequestException("Telefon numarası sistemde kayıtlıdır");
            bool tcExists = await _repositoryManager.Patient.TcExists(dto.TcKimlik);
            if (tcExists) throw new BadRequestException("TC numarası sistemde kayıtlıdır");
            var maxProtokol = await _repositoryManager.Patient.GetMaxProtokol();
            if (maxProtokol<=0) maxProtokol=20260;

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
                Protocol=yeniProtol,
                TcKimlik=dto.TcKimlik
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
                Phone=patientDto.Phone,
                TcKimlik=patientDto.TcKimlik
            };

        }

        public async Task<List<GetPatientDTO>> getPatientAsync(string aramaMetni)
        {
            if (aramaMetni is null) throw new BadRequestException("Arama kriterlerini giriniz");
            if (aramaMetni.Length<3) throw new BadRequestException("Arama metni 3 karakterden küçük olamaz");
            var result = await _repositoryManager.Patient.getPatientAsync(aramaMetni);
            if (!result.Any()) throw new NotFoundException("Hasta kaydı bulunamadı");
            return result;
        }

        public async Task<Patient> UpdatePatient(UpdatePatientDTO hasta, int protokol)
        {
            var güncellenecekHasta = await _repositoryManager.Patient.GetPatientByProtokolASycn(protokol);
            if (güncellenecekHasta is null)
                throw new NotFoundException("Güncellenecek hasta bulunamadı");

            if (hasta.BirthDate.HasValue)
            {
                if (hasta.BirthDate.Value > DateTime.Now)
                    throw new BadRequestException("Doğum tarihi güncel tarihten büyük olamaz");

                var age = DateTime.Now.Year - hasta.BirthDate.Value.Year;
                if (age > 120)
                    throw new BadRequestException("Geçerli doğum tarihi giriniz");
            }

            if (hasta.Name != null && hasta.Name.Any(char.IsDigit))
                throw new BadRequestException("İsim rakam içeremez");

            if (hasta.Surname != null && hasta.Surname.Any(char.IsDigit))
                throw new BadRequestException("Soyisim rakam içeremez");

            string? phone = null;
            if (hasta.Phone != null)
            {
                phone = hasta.Phone.TrimStart('0');
                if (!phone.Any(char.IsDigit))
                    throw new BadRequestException("Telefon numarası karakter içeremez");

                bool phoneExists = await _repositoryManager.Patient.PhoneExists(phone);
                if (phoneExists)
                    throw new BadRequestException("Telefon numarası sistemde kayıtlıdır");
            }

            if (hasta.TcKimlik.HasValue)
            {
                string tcKontrol = Convert.ToString(hasta.TcKimlik.Value);
                if (!tcKontrol.All(char.IsDigit))
                    throw new BadRequestException("TC kimlik numarası karakter içeremez");
                if (tcKontrol.Length < 11)
                    throw new BadRequestException("TC kimlik numarası 11 haneden küçük olamaz");

                bool tcExists = await _repositoryManager.Patient.TcExists(hasta.TcKimlik.Value);
                if (tcExists)
                    throw new BadRequestException("TC numarası sistemde kayıtlıdır");
            }

            if (hasta.Name != null) güncellenecekHasta.Name = hasta.Name;
            if (hasta.Surname != null) güncellenecekHasta.Surname = hasta.Surname;
            if (hasta.Address != null) güncellenecekHasta.Address = hasta.Address;
            if (phone != null) güncellenecekHasta.Phone = phone;
            if (hasta.BirthDate.HasValue) güncellenecekHasta.BirthDate = hasta.BirthDate.Value;
            if (hasta.gender.HasValue) güncellenecekHasta.Gender = hasta.gender.Value;
            if (hasta.BloodType.HasValue) güncellenecekHasta.BloodType = hasta.BloodType.Value;
            if (hasta.TcKimlik.HasValue) güncellenecekHasta.TcKimlik = hasta.TcKimlik.Value;

            await _repositoryManager.saveAsyc();
            return güncellenecekHasta;
        }
    }
}