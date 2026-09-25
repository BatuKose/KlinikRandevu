namespace Entities.Data_Transfer_Objects.Muayene
{
    public class HastaTaahütnameListeDTO
    {
        public int Id { get; set; }
        public int MuayeneId { get; set; }
        public DateTime MuayeneTarihi { get; set; }
        public string PolAdi { get; set; }
        public double ToplamBorc { get; set; }
        public DateTime TahütTarihi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public bool BilgilendirmeSms { get; set; }
        public bool BilgilendirmeMail { get; set; }
        public bool iptal { get; set; }
        public bool odendi { get; set; }
    }
}
