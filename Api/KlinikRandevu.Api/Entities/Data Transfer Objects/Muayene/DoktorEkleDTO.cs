using System.ComponentModel.DataAnnotations;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class DoktorEkleDTO
    {
        [Required]
        public string DoktorAd { get; set; }
        public long DoktorTc { get; set; }
        public int TescilNo { get; set; }
        public int UzmanlikKodu { get; set; }
        public int? ServisNo { get; set; }
        public string? Email { get; set; }
    }
}
