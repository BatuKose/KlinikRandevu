using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class RandevuOlusturDTO
    {
        public int DoktorNo { get; set; }
        public int PolNo { get; set; }
        public long HastaTc { get; set; }
        public int ProtocolNo { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public int SureDakika { get; set; }
        public string? Notlar { get; set; }
    }
}
