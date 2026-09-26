// Sayfalar arasında tekrar eden tarih yardımcıları. API'ye giden tarihler
// yerel saatle (YYYY-MM-DD) üretilir; toISOString UTC'ye kaydırdığı için kullanılmıyor.

export function yerelTarih(d) {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

export function bugun() {
  return yerelTarih(new Date());
}

export function haftaninPazartesisi(d) {
  const kopya = new Date(d);
  const gun = kopya.getDay(); // 0 = Pazar, 1 = Pazartesi, ...
  const fark = gun === 0 ? -6 : 1 - gun;
  kopya.setDate(kopya.getDate() + fark);
  kopya.setHours(0, 0, 0, 0);
  return kopya;
}

export function gunEkle(d, gun) {
  const kopya = new Date(d);
  kopya.setDate(kopya.getDate() + gun);
  return kopya;
}

export function tarihFormat(deger) {
  if (!deger) return '-';
  return new Date(deger).toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

export function uzunTarihFormat(deger) {
  if (!deger) return '-';
  return new Date(deger).toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric', weekday: 'long' });
}

export function saatFormat(deger) {
  if (!deger) return '-';
  return new Date(deger).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
}

export function tarihSaatFormat(deger) {
  if (!deger) return '-';
  return `${tarihFormat(deger)} ${saatFormat(deger)}`;
}

export function yasHesapla(dogumTarihi) {
  if (!dogumTarihi) return null;
  const d = new Date(dogumTarihi);
  if (Number.isNaN(d.getTime())) return null;
  const simdi = new Date();
  let yas = simdi.getFullYear() - d.getFullYear();
  const ayFark = simdi.getMonth() - d.getMonth();
  if (ayFark < 0 || (ayFark === 0 && simdi.getDate() < d.getDate())) yas -= 1;
  return yas;
}

export function ayniGunMu(a, b) {
  return yerelTarih(new Date(a)) === yerelTarih(new Date(b));
}
