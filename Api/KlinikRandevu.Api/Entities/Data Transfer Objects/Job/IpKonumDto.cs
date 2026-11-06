using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Job
{
    public class IpKonumDto
    {
        public string Status { get; set; }
        public string Country { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
    }
}
