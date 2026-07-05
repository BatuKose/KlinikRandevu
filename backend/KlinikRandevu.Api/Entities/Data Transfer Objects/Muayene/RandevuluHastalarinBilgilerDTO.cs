using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class RandevuluHastalarinBilgilerDTO
    {
        public DateTime RandevuTarihi { get; set; }
        public string DoktorAd { get; set; }
        public string poladi { get; set; }
        public string uzmanlik { get; set; }
        public string hasta { get; set; }      
        public string cinsiyet { get; set; }
        public string? adres { get; set; }     
    }
}
