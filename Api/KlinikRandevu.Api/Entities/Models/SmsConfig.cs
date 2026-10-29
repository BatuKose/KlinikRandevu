using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class SmsConfig
    {
        public  int Id { get; set; }
        public string AccountId { get; set; }
        public string Token { get; set; }
        public string FromNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
