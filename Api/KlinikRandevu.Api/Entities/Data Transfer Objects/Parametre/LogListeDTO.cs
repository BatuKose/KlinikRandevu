namespace Entities.Data_Transfer_Objects.Parametre
{
    public class LogListeDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? KullaniciAdi { get; set; }
        public string AksiyonTipi { get; set; }
        public string? EntityTipi { get; set; }
        public int? EntityId { get; set; }
        public string? Detay { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public string? IpAdresi { get; set; }
    }
}
