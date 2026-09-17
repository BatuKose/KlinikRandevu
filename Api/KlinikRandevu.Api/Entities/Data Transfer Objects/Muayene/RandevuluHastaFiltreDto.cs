using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class RandevuluHastaFiltreDto
    {
        public DateTime Basla { get; set; }
        public DateTime Bitis { get; set; }
        public bool MuayeneOldumu { get; set; }
    }
}
