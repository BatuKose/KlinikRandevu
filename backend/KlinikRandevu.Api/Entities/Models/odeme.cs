using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class odeme
    {
        public int Id { get; set; }
        public int muayeneId { get; set; }
        public double odemeToplam { get; set; }
        public DateTime odemeTarihi { get; set; }
        public int tedaviId { get; set; }

    }
}
