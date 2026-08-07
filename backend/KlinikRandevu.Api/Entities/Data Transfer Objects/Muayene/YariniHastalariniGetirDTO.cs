using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class YariniHastalariniGetirDTO
    {
        public int doktor { get; set; }
        public int polno { get; set; }
        public int protokol { get; set; }
        public DateTime tarih { get; set; }
        public long tc { get; set; }
        public TimeSpan muayenesaati { get; set; }
        public int randevuid { get; set; }

    }
}
