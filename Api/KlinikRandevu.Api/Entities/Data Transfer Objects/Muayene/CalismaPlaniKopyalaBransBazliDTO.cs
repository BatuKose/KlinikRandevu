using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class CalismaPlaniKopyalaBransBazliDTO
    {
        public DayOfWeek YeniGün { get; set; }
        public int CalismaPlaniId { get; set; }
        public int DoktorNumara { get; set; }
    }
}
