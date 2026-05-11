using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class SistemParametresi
    {
        public int Id { get; set; }
        public string ParametreAdi { get; set; }    
        public string? Deger1 { get; set; }
        public string? Deger2 { get; set; }
        public string? Deger3 { get; set; }
        public string? Deger4 { get; set; }
        public string? Deger5 { get; set; }
        public string? Aciklama { get; set; }
        public bool Aktif { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
    }
}
