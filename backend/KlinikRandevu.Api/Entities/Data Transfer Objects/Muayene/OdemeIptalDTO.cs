using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.odemeTipiEnum;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class OdemeIptalDTO
    {
        public int? tedaviId { get; set; }
        public int muyaneId { get; set; }
        public OdemeEnum odemeIptalTipi { get; set; }
    }
}
