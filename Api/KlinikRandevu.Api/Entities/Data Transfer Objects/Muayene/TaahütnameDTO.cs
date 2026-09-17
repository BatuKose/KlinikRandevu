using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class TaahütnameDTO
    {
        public int MuayeneId { get; set; }
        public double ToplamBorc { get; set; }
        public DateTime TahütTarihi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public bool BilgilendirmeSms { get; set; }
        public bool BilgilendirmeMail { get; set; }
        public bool iptal { get; set; }
        public bool odendi { get; set; }

    }
}
