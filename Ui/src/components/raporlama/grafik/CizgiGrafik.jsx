import { useState } from 'react';
import { eksenOlcegi, etiketAtlama, useGenislik, sayiFormat } from './grafikYardimci';
import GrafikIpucu from './GrafikIpucu';

const YUKSEKLIK = 320;
const KENAR = { ust: 16, sag: 16, alt: 30, sol: 48 };
const ETIKET_ALANI = 120; // ≤4 seride çizgi sonuna doğrudan etiket yazılır

// Zaman içindeki eğilim: her seri bir çizgi. x ekseni bant merkezleri kullanır
// ki aynı veriyle çizilen sütun grafikle ay konumları birebir örtüşsün.
// seriler: [{ ad, renk, degerler: number[] }]
export default function CizgiGrafik({ etiketler, seriler, uzunEtiketler }) {
  const [ref, genislik] = useGenislik();
  const [hover, setHover] = useState(null);

  const dogrudanEtiket = seriler.length > 0 && seriler.length <= 4 && genislik > 520;
  const sag = KENAR.sag + (dogrudanEtiket ? ETIKET_ALANI : 0);
  const cizimG = Math.max(0, genislik - KENAR.sol - sag);
  const cizimY = YUKSEKLIK - KENAR.ust - KENAR.alt;
  const adet = etiketler.length;
  const bant = adet ? cizimG / adet : 0;

  const maks = Math.max(0, ...seriler.flatMap((s) => s.degerler));
  const { ust, tikler } = eksenOlcegi(maks);
  const x = (i) => KENAR.sol + bant * (i + 0.5);
  const y = (v) => KENAR.ust + cizimY - (v / ust) * cizimY;
  const atla = etiketAtlama(adet, cizimG);

  // Çizgi sonu etiketleri: üst üste binmesinler diye dikeyde en az 14px aralık.
  const sonEtiketler = [];
  if (dogrudanEtiket && adet) {
    seriler
      .map((s) => ({ ad: s.ad, renk: s.renk, y: y(s.degerler[adet - 1] ?? 0) }))
      .sort((a, b) => a.y - b.y)
      .forEach((e, i, liste) => {
        if (i > 0 && e.y - liste[i - 1].y < 14) e.y = liste[i - 1].y + 14;
        sonEtiketler.push(e);
      });
  }

  const fareHareket = (e) => {
    const kutu = e.currentTarget.getBoundingClientRect();
    const mx = e.clientX - kutu.left;
    const i = Math.min(adet - 1, Math.max(0, Math.floor((mx - KENAR.sol) / bant)));
    setHover(Number.isFinite(i) ? i : null);
  };

  return (
    <div className="grf-kapsayici" ref={ref}>
      {genislik > 0 && (
        <svg
          width={genislik}
          height={YUKSEKLIK}
          role="img"
          aria-label="Aylara göre branş eğilim grafiği"
          onMouseMove={fareHareket}
          onMouseLeave={() => setHover(null)}
        >
          {tikler.map((t) => (
            <g key={t}>
              <line className="grf-izgara" x1={KENAR.sol} x2={KENAR.sol + cizimG} y1={y(t)} y2={y(t)} />
              <text className="grf-eksen-yazi" x={KENAR.sol - 8} y={y(t)} dy="0.32em" textAnchor="end">
                {sayiFormat(t)}
              </text>
            </g>
          ))}
          {etiketler.map((et, i) =>
            i % atla === 0 ? (
              <text key={et} className="grf-eksen-yazi" x={x(i)} y={YUKSEKLIK - 8} textAnchor="middle">
                {et}
              </text>
            ) : null
          )}

          {hover !== null && (
            <line className="grf-imlec" x1={x(hover)} x2={x(hover)} y1={KENAR.ust} y2={KENAR.ust + cizimY} />
          )}

          {seriler.map((s) => (
            <g key={s.ad}>
              <polyline
                fill="none"
                stroke={s.renk}
                strokeWidth="2"
                strokeLinejoin="round"
                strokeLinecap="round"
                points={s.degerler.map((v, i) => `${x(i)},${y(v)}`).join(' ')}
              />
              {/* Tek noktalı (veya az noktalı) aralıkta çizgi görünmesin diye işaretçi */}
              {adet <= 2 &&
                s.degerler.map((v, i) => (
                  <circle key={i} cx={x(i)} cy={y(v)} r="4" fill={s.renk} stroke="var(--renk-kart)" strokeWidth="2" />
                ))}
            </g>
          ))}

          {hover !== null &&
            seriler.map((s) => (
              <circle
                key={s.ad}
                cx={x(hover)}
                cy={y(s.degerler[hover])}
                r="4.5"
                fill={s.renk}
                stroke="var(--renk-kart)"
                strokeWidth="2"
              />
            ))}

          {sonEtiketler.map((e) => (
            <g key={e.ad}>
              <circle cx={KENAR.sol + cizimG + 10} cy={e.y} r="3.5" fill={e.renk} />
              <text className="grf-dogrudan-etiket" x={KENAR.sol + cizimG + 18} y={e.y} dy="0.32em">
                {e.ad.length > 15 ? `${e.ad.slice(0, 14)}…` : e.ad}
              </text>
            </g>
          ))}
        </svg>
      )}
      {hover !== null && (
        <GrafikIpucu
          x={x(hover)}
          y={KENAR.ust}
          kapsayiciGenislik={genislik}
          baslik={uzunEtiketler?.[hover] ?? etiketler[hover]}
          satirlar={seriler
            .map((s) => ({ ad: s.ad, renk: s.renk, deger: s.degerler[hover] }))
            .sort((a, b) => b.deger - a.deger)}
          alt={seriler.length > 1 ? `Toplam: ${sayiFormat(seriler.reduce((t, s) => t + s.degerler[hover], 0))}` : null}
        />
      )}
    </div>
  );
}
