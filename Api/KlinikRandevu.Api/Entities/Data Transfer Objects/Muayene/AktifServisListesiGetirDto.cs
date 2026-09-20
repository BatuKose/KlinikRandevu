using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Entities.Enums.PoliklinikEnum;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class AktifServisListesiGetirDto
    {
        public int ServisNo { get; set; }
        public string ServisAdi { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UzmanlikBransi PolUzKod { get; set; }
    }
}
