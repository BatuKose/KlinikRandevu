import UzmanlikBazliHastaGelisleri from './raporlar/UzmanlikBazliHastaGelisleri';
import TeshisRaporu from './raporlar/TeshisRaporu';

// Raporlama ekranındaki tüm raporlar buradan beslenir. Yeni rapor eklemek için:
// 1) components/raporlama/raporlar altına bileşenini yaz (kendi filtreleri + grafiği),
// 2) bu listeye bir kayıt ekle. Sayfa, katalog ve URL (?rapor=id) otomatik çalışır.
export const RAPORLAR = [
  {
    id: 'uzmanlik-bazli-hasta-gelisleri',
    baslik: 'Uzmanlık Bazlı Hasta Gelişleri',
    aciklama: 'Branşlara göre aylık muayene sayıları, eğilim ve dağılım',
    kategori: 'Hasta & Muayene',
    ikon: 'stetoskop',
    bilesen: UzmanlikBazliHastaGelisleri,
  },
  {
    id: 'teshis-raporu',
    baslik: 'Teşhis Raporu',
    aciklama: 'Aylara göre girilen teşhis sayısı, en sık teşhisler ve branş dağılımı',
    kategori: 'Hasta & Muayene',
    ikon: 'dosya',
    bilesen: TeshisRaporu,
  },
];

export function raporBul(id) {
  return RAPORLAR.find((r) => r.id === id);
}
