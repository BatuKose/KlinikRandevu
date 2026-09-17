using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Yetki
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Kod { get; set; }
        public ICollection<UserYetki> UserYetkiler { get; set; }
    }
}
