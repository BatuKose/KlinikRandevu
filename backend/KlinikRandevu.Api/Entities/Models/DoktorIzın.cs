using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class DoktorIzın
    {
        public int Id { get; set; }
        public int DoktorNo { get; set; }
        public bool Iptal { get; set; }=false;
        public DateOnly IzinBaslangic { get; set; }
        public DateOnly IzinBitis { get; set; }
        public string? Aciklama { get; set; }

    }
}
