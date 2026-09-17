using Microsoft.AspNetCore.Components.Web;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Taahütname
    {
        public int Id { get; set; }
        public int MuayeneId{ get; set; }
        public double ToplamBorc { get; set; }
        public DateTime TahütTarihi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public bool  BilgilendirmeSms{ get; set; }
        public bool  BilgilendirmeMail{ get; set; }
        public bool iptal {  get; set; }
        public bool odendi { get; set; }

    }
}
