using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class DoktorRandevuHatirlatmaEmailDTO
    {
        public string doktorad { get; set; }
        public string polad { get; set; }
        public string hastaad { get; set; }
        public string hastsoyad { get; set; }
        public string doktormail { get; set; }
        public DateTime randevutarihi { get; set; }
    }
}
