using System;

namespace Entities.Data_Transfer_Objects.Raporlama
{
    // Ay + teşhis + branş kırılımında teşhis sayısı. UI aylık toplamı,
    // en sık teşhisleri ve teşhisin branş dağılımını bu satırlardan üretir.
    public class TeshisRaporDto
    {
        // Ayın ilk günü (sorgu aylık gruplar)
        public DateTime tarih { get; set; }
        public string teshisKod { get; set; }
        public string teshisAd { get; set; }
        public string uzmanlikAdi { get; set; }
        public int teshisSayisi { get; set; }
    }
}
