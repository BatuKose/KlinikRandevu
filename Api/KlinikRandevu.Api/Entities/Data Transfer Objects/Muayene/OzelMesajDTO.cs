using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class OzelMesajDTO
    {
        public string hasta { get; set; }
        public string pol { get; set; }
        public DateTime randevu  { get; set; }
        public int protokol { get; set; }
        public int randevuid { get; set; }
    }
}
