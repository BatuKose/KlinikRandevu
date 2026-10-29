using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.Sms
{
    public class UpdateSmsConfigDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string? Token { get; set; } 
        public string FromNumber { get; set; }
    }
}
