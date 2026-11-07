import { useLayoutEffect, useRef, useState } from 'react';

// Kategorik seri renkleri: sabit sırayla atanır, döngüye girmez. 8'den fazla seri
// "Diğer" altında toplanır (nötr gri). Renk varlığı takip eder, sırayı değil:
// filtreyle seri gizlense de kalanların rengi değişmez.
export const SERI_RENKLERI = ['#2a78d6', '#eb6834', '#1baf7a', '#eda100', '#e87ba4', '#008300', '#6250d6', '#e34948'];
export const DIGER_RENGI = '#94a3b8';
export const TEK_SERI_RENGI = SERI_RENKLERI[0];

const sayiBicim = new Intl.NumberFormat('tr-TR');
export const sayiFormat = (n) => sayiBicim.format(n ?? 0);

// 0'dan başlayan, "yuvarlak" adımlı eksen ölçeği (1-2-5 serisi).
export function eksenOlcegi(maks, hedefAdim = 5) {
  if (!maks || maks <= 0) return { ust: hedefAdim, adim: 1, tikler: [0, 1, 2, 3, 4, 5] };
  const kaba = maks / hedefAdim;
  const us = 10 ** Math.floor(Math.log10(kaba));
  const adim = [1, 2, 5, 10].map((k) => k * us).find((a) => maks / a <= hedefAdim) ?? 10 * us;
  const tamAdim = Math.max(1, adim);
  const ust = Math.ceil(maks / tamAdim) * tamAdim;
  const tikler = [];
  for (let v = 0; v <= ust; v += tamAdim) tikler.push(v);
  return { ust, adim: tamAdim, tikler };
}

// Kapsayıcının genişliğini izler; grafikler buna göre yeniden çizilir.
export function useGenislik() {
  const ref = useRef(null);
  const [genislik, setGenislik] = useState(0);
  useLayoutEffect(() => {
    const el = ref.current;
    if (!el) return undefined;
    setGenislik(el.getBoundingClientRect().width);
    const izleyici = new ResizeObserver(([kayit]) => setGenislik(kayit.contentRect.width));
    izleyici.observe(el);
    return () => izleyici.disconnect();
  }, []);
  return [ref, genislik];
}

// Sadece üst köşeleri yuvarlatılmış dikdörtgen (bar uçları tabana oturur).
export function ustYuvarlakYol(x, y, w, h, r = 4) {
  if (h <= 0 || w <= 0) return '';
  const rr = Math.min(r, w / 2, h);
  return `M${x},${y + h} L${x},${y + rr} Q${x},${y} ${x + rr},${y} L${x + w - rr},${y} Q${x + w},${y} ${x + w},${y + rr} L${x + w},${y + h} Z`;
}

// Etiket kalabalığını önlemek için x ekseninde kaç etikette bir yazılacağı.
export function etiketAtlama(adet, genislik, etiketGenisligi = 52) {
  if (!genislik || adet === 0) return 1;
  return Math.max(1, Math.ceil((adet * etiketGenisligi) / genislik));
}
