using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Sms
{
    public class InsertSmsConfigDto
    {
        public string AccountId { get; set; }
        public string Token { get; set; }
        public string FromNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
