using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Raporlama
{
    public class UzamanlikBazliRaporDto
    {
        public string uzmanlikAdi { get; set; }
        public DateTime tarih { get; set; }
        public int muayeneSayisi { get; set; }
    }
}
