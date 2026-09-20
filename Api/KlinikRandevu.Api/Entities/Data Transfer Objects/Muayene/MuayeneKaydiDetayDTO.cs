using System;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class MuayeneKaydiDetayDTO
    {
        public int Id { get; set; }
        public int ProtocolNo { get; set; }
        public int DoktorNo { get; set; }
        public string DoktorAd { get; set; }
        public int PolNo { get; set; }
        public string PolAdi { get; set; }
        public long HastaTc { get; set; }
        public DateTime MuayeneTarihi { get; set; }
        public TimeSpan BaslangicSaati { get; set; }
        public TimeSpan? BitisSaati { get; set; }
        public int? RandevuId { get; set; }
    }
}
