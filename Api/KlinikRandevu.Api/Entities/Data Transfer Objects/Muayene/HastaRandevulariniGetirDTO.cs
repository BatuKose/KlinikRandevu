using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class HastaRandevulariniGetirDTO
    {
        public int dosyaId { get; set; }
        public int protokol { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }  
        public long tc { get; set; }
        public string poliklinik { get; set; }
        public string Doktor { get; set; }
        public string UzmanlikDali { get; set; }
        public DateTime RandevuTarihi { get; set; }
    }
}
