using System;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class CalismaPlaniListeDTO
    {
        public int Id { get; set; }
        public int DoktorNo { get; set; }
        public string? DoktorAd { get; set; }
        public int PolNo { get; set; }
        public string? PolAdi { get; set; }
        public DayOfWeek GunAdi { get; set; }
        public TimeSpan BaslangicSaati { get; set; }
        public TimeSpan BitisSaati { get; set; }
        public int RandevuSuresiDk { get; set; }
        public int SlotSayisi { get; set; }
        public bool IsActive { get; set; }
        public bool YesilAlanZorunlu { get; set; }
    }
}
