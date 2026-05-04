using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{

        public class Randevu
        {
            public int Id { get; set; }
            public int DoktorNo { get; set; }
            public int PolNo { get; set; }
            public long HastaTc { get; set; }
            public int ProtocolNo { get; set; }
            public DateTime RandevuTarihi { get; set; }  
            public int SureDakika { get; set; }
            public string? Notlar { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }
}

