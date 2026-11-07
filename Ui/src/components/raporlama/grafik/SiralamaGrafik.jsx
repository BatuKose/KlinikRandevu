import { TEK_SERI_RENGI, sayiFormat } from './grafikYardimci';

// Yatay sıralama çubukları: tek seri (tek renk), büyükten küçüğe. Uzun branş
// adları dikey eksende okunaklı kalsın diye HTML grid ile çizilir.
// satirlar: [{ ad, deger }]
export default function SiralamaGrafik({ satirlar }) {
  const maks = Math.max(1, ...satirlar.map((s) => s.deger));
  const toplam = satirlar.reduce((t, s) => t + s.deger, 0);

  return (
    <ol className="grf-siralama" aria-label="Branşlara göre toplam muayene sıralaması">
      {satirlar.map((s, i) => {
        const pay = toplam ? (s.deger / toplam) * 100 : 0;
        return (
          <li
            key={s.ad}
            className="grf-siralama-satir"
            title={`${s.ad}: ${sayiFormat(s.deger)} muayene (%${pay.toFixed(1)})`}
          >
            <span className="grf-siralama-sira">{i + 1}</span>
            <span className="grf-siralama-ad">{s.ad}</span>
            <span className="grf-siralama-iz">
              <span
                className="grf-siralama-bar"
                style={{ width: `${(s.deger / maks) * 100}%`, background: TEK_SERI_RENGI }}
              />
            </span>
            <span className="grf-siralama-deger">
              {sayiFormat(s.deger)}
              <small>%{pay.toFixed(1)}</small>
            </span>
          </li>
        );
      })}
    </ol>
  );
}
