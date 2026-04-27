using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int  Protocol { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public GenderEnum Gender { get; set; }
        public BloodTypeEnum BloodType { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public bool IsActive { get; set; }
        public long TcKimlik { get; set; }
    }
}
