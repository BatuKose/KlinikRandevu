using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Tetkikler
    {
        public int Id { get; set; }
        public string TetikAdi { get; set; }
        public double Fiyat { get; set; }
        public string Kodu { get; set; }
        public bool aktifMi { get; set; }

    }
}
