using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class JobHatirlatmaSorguDTO
    {
        public DateTime randevutarihi { get; set; }
        public string poliklinik { get; set; }
        public string email { get; set; }
        public string numara { get; set; }
        public string doktorad { get; set; }
        public bool randevuMailGonderildimi { get; set; }
        public int randevuId { get; set; }
    }
}
