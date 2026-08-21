using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class RandevuBekleyenHastalar
    {
        public int Id { get; set; }
        public long tcKimlik { get; set; }
        public int protokol { get; set; }
        public int doktorNo { get; set; }
        public int polNo { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public bool Bilgilendirme { get; set; }
        public bool RandevuVerildi { get; set; }
        public string? randevuNotu { get; set; }

    }
}
