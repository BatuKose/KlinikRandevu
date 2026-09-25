using System.ComponentModel.DataAnnotations;

namespace Entities.Data_Transfer_Objects.Muayene
{
    public class ServisEkleDTO
    {
        [Required]
        public string Name { get; set; }
        public string? Aciklama { get; set; }
        public int UzmanlikKodu { get; set; }
        public int? DoktorNo { get; set; }
        public int? KatNo { get; set; }
        public string? OdaNo { get; set; }
        public int? MaxRandevuSuresi { get; set; }
        public int? GunlukMaksRandevuSayisi { get; set; }
        public string? Telefon { get; set; }
        public bool OnlineRandevuAktif { get; set; }
    }
}
