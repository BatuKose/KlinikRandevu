using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Patient
{
    public class UpdatePatientDTO
    {
        public int Protocol { get;}
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public GenderEnum? gender { get; set; }
        public BloodTypeEnum? BloodType { get; set; }
        public long? TcKimlik { get; set; }
    }
}
