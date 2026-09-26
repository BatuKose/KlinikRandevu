using System;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class RandevuSlotDTO
    {
        public DateTime Baslangic { get; set; }
        public int SureDk { get; set; }
        public bool Dolu { get; set; }
        public bool Gecmis { get; set; }
    }
}
