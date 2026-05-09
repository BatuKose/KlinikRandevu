using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.PoliklinikEnum;

namespace Entities.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public int doktorNo { get; set; }
        public string DoktorAd { get; set; }
        public UzmanlikBransi doktorUzKod { get; set; }
        public long doktorTc { get; set; }
        public int ServisNo { get; set; }
        public int tescilNO { get; set; }
        public bool isActive { get; set; }=true;
    }
}
