using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class DoktorIzınOlusturDTO
    {
        public int DoktorNo { get; set; }
        public DateOnly IzinBaslangic { get; set; }
        public DateOnly IzinBitis { get; set; }
        public string? Aciklama { get; set; }
    }
}
