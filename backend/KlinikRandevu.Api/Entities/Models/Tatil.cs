using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Tatil
    {
        public int Id { get; set; }
        public string TatilAdi { get; set; }
        public DateTime Tarih { get; set; }
        public bool ApiMi { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
