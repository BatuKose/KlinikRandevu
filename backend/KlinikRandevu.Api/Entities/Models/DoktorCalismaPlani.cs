using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class DoktorCalismaPlani
    {
        public int Id { get; set; }
        public int DoktorNo { get; set; }
        public int PolNo { get; set; }
        public DayOfWeek GunAdi { get; set; }        
        public TimeSpan BaslangicSaati { get; set; } 
        public TimeSpan BitisSaati { get; set; }     
        public int RandevuSuresiDk { get; set; }  
        public bool IsActive { get; set; } = true;
    }
}
