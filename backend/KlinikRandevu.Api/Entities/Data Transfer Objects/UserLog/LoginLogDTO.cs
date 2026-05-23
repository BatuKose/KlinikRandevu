using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data_Transfer_Objects.UserLog
{
    public class LoginLogDTO
    {
        public int userID { get; set; }
        public string IpAdress { get; set; }
        public string AksiyonTipi { get; set; }
        public string Tablo { get; set; }
    }
}
