using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class MuayeneKayitiOlusturDTO
    {
        public int ProtocolNo { get; set; }
        public int DoktorNo { get; set; }
        public int PolNo { get; set; }
        public long HastaTc { get; set; }
        public DateTime MuayeneTarihi { get; set; }
        public TimeSpan BaslangicSaati { get; set; }
    }
}
