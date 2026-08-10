using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class TaahütBilgilendirme
    {
        public string mail { get; set; }
        public string tel { get; set; }
        public double borc { get; private set; }
        public DateTime SonOdemeTarihi { get; set; }
        public DateTime TaTarih { get; set; }
        public string polAd { get; set; }
        public DateTime muaTarih { get; set; }

    }
}
