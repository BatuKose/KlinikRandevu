import { useCallback, useEffect, useState } from 'react';
import { getApiErrorMessage } from '../../utils/apiError';
import { yerelTarih, bugun } from '../../utils/tarih';
import { SERI_RENKLERI, DIGER_RENGI } from './grafik/grafikYardimci';

// Tarih aralığıyla çalışan tüm raporların ortak yardımcıları.

const AY_KISA = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'];
const AY_UZUN = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
const MAKS_GUN = 366; // backend sınırı ile aynı
export const DIGER = 'Diğer';

export function ayBasi(ayGeri) {
  const d = new Date();
  return yerelTarih(new Date(d.getFullYear(), d.getMonth() - ayGeri, 1));
}

export const HAZIR_ARALIKLAR = [
  { id: '3ay', etiket: 'Son 3 Ay', baslangic: () => ayBasi(2) },
  { id: '6ay', etiket: 'Son 6 Ay', baslangic: () => ayBasi(5) },
  { id: '12ay', etiket: 'Son 12 Ay', baslangic: () => ayBasi(11) },
  { id: 'buyil', etiket: 'Bu Yıl', baslangic: () => `${new Date().getFullYear()}-01-01` },
];

// 'YYYY-MM-DD' aralığındaki ayları 'YYYY-MM' anahtarlarıyla sırayla üretir;
// verisi olmayan aylar grafikte 0 olarak görünsün diye.
export function aylariUret(baslangic, bitis) {
  const [by, ba] = baslangic.split('-').map(Number);
  const [sy, sa] = bitis.split('-').map(Number);
  const aylar = [];
  for (let y = by, a = ba; y < sy || (y === sy && a <= sa); a === 12 ? (y++, (a = 1)) : a++) {
    aylar.push({
      anahtar: `${y}-${String(a).padStart(2, '0')}`,
      kisa: `${AY_KISA[a - 1]} ${String(y).slice(2)}`,
      uzun: `${AY_UZUN[a - 1]} ${y}`,
    });
  }
  return aylar;
}

export function ayAnahtari(tarih) {
  return String(tarih).slice(0, 7);
}

function tarihAraligiHatasi(baslangic, bitis) {
  if (!baslangic || !bitis) return 'Başlangıç ve bitiş tarihi seçilmelidir.';
  if (baslangic > bitis) return 'Başlangıç tarihi bitiş tarihinden büyük olamaz.';
  if ((new Date(bitis) - new Date(baslangic)) / 86400000 > MAKS_GUN) return 'En fazla 1 yıllık rapor alınabilir.';
  return '';
}

// Tarih aralığı state'i + veri çekme. getir(baslangic, bitis) axios promise'i döner.
export function useTarihliRapor(getir, hataMetni = 'Rapor verisi alınamadı') {
  const [baslangic, setBaslangic] = useState(() => ayBasi(5));
  const [bitis, setBitis] = useState(bugun);
  const [kayitlar, setKayitlar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');
  const aralikHatasi = tarihAraligiHatasi(baslangic, bitis);

  const yukle = useCallback(async () => {
    if (aralikHatasi) return;
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await getir(baslangic, bitis);
      setKayitlar((data?.data ?? data ?? []).filter(Boolean));
    } catch (err) {
      setKayitlar([]);
      setHata(getApiErrorMessage(err, hataMetni));
    } finally {
      setYukleniyor(false);
    }
  }, [getir, baslangic, bitis, aralikHatasi, hataMetni]);

  useEffect(() => {
    // eslint-disable-next-line react/set-state-in-effect
    yukle();
  }, [yukle]);

  return { baslangic, setBaslangic, bitis, setBitis, kayitlar, yukleniyor, hata, setHata, aralikHatasi, yukle };
}

// Toplama göre sıralı listeye sabit sırayla renk verir; 8'den sonrası "Diğer"e katlanır.
// liste: [{ ad, toplam, degerler? }] (büyükten küçüğe sıralı)
export function serileriKatla(liste, ayAdedi = 0) {
  const renkli = liste.slice(0, SERI_RENKLERI.length).map((b, i) => ({ ...b, renk: SERI_RENKLERI[i] }));
  const kalan = liste.slice(SERI_RENKLERI.length);
  const seriler = kalan.length
    ? [
        ...renkli,
        {
          ad: DIGER,
          renk: DIGER_RENGI,
          degerler: Array.from({ length: ayAdedi }, (_, i) => kalan.reduce((t, b) => t + (b.degerler?.[i] ?? 0), 0)),
          toplam: kalan.reduce((t, b) => t + b.toplam, 0),
          icerik: kalan.length,
        },
      ]
    : renkli;
  return { seriler, renkHaritasi: new Map(renkli.map((b) => [b.ad, b.renk])) };
}

// Excel'in Türkçe karakterleri doğru açması için BOM + ';' ayraçlı CSV.
export function csvIndir(dosyaAdi, satirlar) {
  const icerik = satirlar.map((s) => s.map((h) => `"${String(h ?? '').replaceAll('"', '""')}"`).join(';')).join('\r\n');
  const url = URL.createObjectURL(new Blob(['﻿' + icerik], { type: 'text/csv;charset=utf-8' }));
  const a = document.createElement('a');
  a.href = url;
  a.download = dosyaAdi;
  a.click();
  URL.revokeObjectURL(url);
}
