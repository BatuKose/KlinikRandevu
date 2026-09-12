using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.PoliklinikEnum;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class YesilListeyeHastaEkleDTO
    {
        public long hastaTc { get; set; }
        public int muayeneId { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public long EkleyenDoktorTC { get; set; }
        public bool aktifMi { get; set; }
        public UzmanlikBransi PolUzKod { get; set; }
    }
}
