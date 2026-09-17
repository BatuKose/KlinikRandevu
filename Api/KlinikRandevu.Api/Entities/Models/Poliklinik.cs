using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Enums.PoliklinikEnum;

namespace Entities.Models
{
    public class Poliklinik
    {
        public int Id { get; set; }
        public int PolNo { get; set; }
        public string Name { get; set; }
        public string? Aciklama { get; set; }         
        public UzmanlikBransi PolUzKod { get; set; }
        public int DoktorNo { get; set; }
        public int? KatNo { get; set; }                 
        public string? OdaNo { get; set; }            
        public int? MaxRandevuSuresi { get; set; }     
        public int? GunlukMaksRandevuSayisi { get; set; }
        public string? Telefon { get; set; }
        public bool OnlineRandevuAktif { get; set; }    
        public bool isActive { get; set; }

    }
}
