using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class UserLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }         
        public string AksiyonTipi { get; set; }   
        public string? EntityTipi { get; set; }   
        public int? EntityId { get; set; }      
        public string? Detay { get; set; }  
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public string? IpAdresi { get; set; }
    }
}
