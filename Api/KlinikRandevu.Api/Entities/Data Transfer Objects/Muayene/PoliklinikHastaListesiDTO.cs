using System;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class PoliklinikHastaListesiDTO
    {
        public int? MuayeneId { get; set; }
        public int? DosyaId { get; set; }
        public int Protokol { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public long Tc { get; set; }
        public string Doktor { get; set; }
        public DateTime Tarih { get; set; }
        public bool MuayeneVarMi { get; set; }
    }
}
