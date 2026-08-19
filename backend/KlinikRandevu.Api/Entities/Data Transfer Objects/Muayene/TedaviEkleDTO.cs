using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class TedaviEkleDTO
    {
        public int MuyaneId { get; set; }
        public string? tedaviKodu { get; set; }
        public string? tedaviAdi { get; set; }
    }
}
