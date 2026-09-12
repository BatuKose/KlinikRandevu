using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.PoliklinikEnum;

namespace Entities.Models
{
    public class PoliklinikYesilAlanAyarları
    {
        public int Id { get; set; }
        public int polNo { get; set; }
        public UzmanlikBransi PolUzKod { get; set; }
        public int gecerlilikSüresi { get; set; }
    }
}
