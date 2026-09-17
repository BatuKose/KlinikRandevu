using Entities.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.odemeTipiEnum;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class OdemeYapDTO
    {
        public int muayeneId { get; set; }
        public double odemeToplam { get; set; }
        public int TedaviId { get; set; }
        public OdemeEnum odeme { get; set; }
    }
}   