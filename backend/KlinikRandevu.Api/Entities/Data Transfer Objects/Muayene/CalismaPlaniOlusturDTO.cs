using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class CalismaPlaniOlusturDTO
    {
        public int DoktorNo { get; set; }
        public int PolNo { get; set; }
        public DayOfWeek GunAdi { get; set; } 
        public TimeSpan BaslangicSaati { get; set; }  
        public TimeSpan BitisSaati { get; set; }       
        public int RandevuSuresiDk { get; set; }
    }
}
