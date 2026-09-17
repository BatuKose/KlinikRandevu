using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class TedaviKaydi
    {
        public int Id { get; set; }
        public int MuyaneId { get; set; }
        public int doktorId { get; set; }
        public double fiyat { get; set; }
        public string tedaviKodu { get; set; }
        public string tedaviAdi { get; set; }
        public int prtokol { get; set; }
        public bool Odendi { get; set; } = false;

    }
}
