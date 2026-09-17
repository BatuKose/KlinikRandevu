using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public  class IcdApiEntegrasyon
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime GecerlilikSüresi { get; set; }
        public string TokenType { get; set; }
    }
}
