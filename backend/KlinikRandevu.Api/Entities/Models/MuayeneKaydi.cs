using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class MuayeneKaydi
    {
        public int Id { get; set; }
        public int ProtocolNo { get; set; }
        public int DoktorNo { get; set; }
        public int PolNo { get; set; }
        public long HastaTc { get; set; }
        public DateTime MuayeneTarihi { get; set; }
        public DateTime BaslangicSaati { get; set; }
        public DateTime BitisSaati { get; set; }
        public int RandevuId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
