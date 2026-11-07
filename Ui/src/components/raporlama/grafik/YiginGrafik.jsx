import { useState } from 'react';
import { eksenOlcegi, etiketAtlama, useGenislik, ustYuvarlakYol, sayiFormat } from './grafikYardimci';
import GrafikIpucu from './GrafikIpucu';

const KENAR = { ust: 16, sag: 16, alt: 30, sol: 48 };
const ARALIK = 2; // segmentler arası yüzey boşluğu

// Yığılmış sütun: her ay bir sütun, segmentler seriler. Toplam hacmi ve
// branşların o aydaki payını birlikte gösterir.
export default function YiginGrafik({ etiketler, seriler, uzunEtiketler, yukseklik = 320, ariaEtiket = 'Aylara göre yığılmış sütun grafiği' }) {
  const [ref, genislik] = useGenislik();
  const [hover, setHover] = useState(null);

  const cizimG = Math.max(0, genislik - KENAR.sol - KENAR.sag);
  const cizimY = yukseklik - KENAR.ust - KENAR.alt;
  const adet = etiketler.length;
  const bant = adet ? cizimG / adet : 0;
  const barG = Math.min(56, Math.max(6, bant * 0.62));

  const toplamlar = etiketler.map((_, i) => seriler.reduce((t, s) => t + s.degerler[i], 0));
  const { ust, tikler } = eksenOlcegi(Math.max(0, ...toplamlar));
  const y = (v) => KENAR.ust + cizimY - (v / ust) * cizimY;
  const h = (v) => (v / ust) * cizimY;
  const xBant = (i) => KENAR.sol + bant * i;
  const atla = etiketAtlama(adet, cizimG);

  return (
    <div className="grf-kapsayici" ref={ref} style={{ minHeight: yukseklik }}>
      {genislik > 0 && (
        <svg width={genislik} height={yukseklik} role="img" aria-label={ariaEtiket}>
          {tikler.map((t) => (
            <g key={t}>
              <line className="grf-izgara" x1={KENAR.sol} x2={KENAR.sol + cizimG} y1={y(t)} y2={y(t)} />
              <text className="grf-eksen-yazi" x={KENAR.sol - 8} y={y(t)} dy="0.32em" textAnchor="end">
                {sayiFormat(t)}
              </text>
            </g>
          ))}

          {etiketler.map((et, i) => {
            const bx = xBant(i) + (bant - barG) / 2;
            let taban = 0;
            const gorunur = seriler.filter((s) => s.degerler[i] > 0);
            return (
              <g key={et} opacity={hover === null || hover === i ? 1 : 0.45}>
                {gorunur.map((s, k) => {
                  const v = s.degerler[i];
                  const segH = h(v);
                  const segY = y(taban + v);
                  taban += v;
                  const enUst = k === gorunur.length - 1;
                  // Üstteki segment hariç her segmentin üstünden yüzey boşluğu kesilir.
                  const kesik = enUst ? 0 : Math.min(ARALIK, segH / 2);
                  return enUst ? (
                    <path key={s.ad} d={ustYuvarlakYol(bx, segY, barG, segH, 4)} fill={s.renk} />
                  ) : (
                    <rect key={s.ad} x={bx} y={segY + kesik} width={barG} height={Math.max(0, segH - kesik)} fill={s.renk} />
                  );
                })}
                {i % atla === 0 && (
                  <text className="grf-eksen-yazi" x={xBant(i) + bant / 2} y={yukseklik - 8} textAnchor="middle">
                    {et}
                  </text>
                )}
                {/* Hover hedefi: sütundan geniş, tüm bant yüksekliği */}
                <rect
                  x={xBant(i)}
                  y={KENAR.ust}
                  width={bant}
                  height={cizimY}
                  fill="transparent"
                  onMouseEnter={() => setHover(i)}
                  onMouseLeave={() => setHover(null)}
                />
              </g>
            );
          })}
        </svg>
      )}
      {hover !== null && (
        <GrafikIpucu
          x={xBant(hover) + bant / 2}
          y={KENAR.ust}
          kapsayiciGenislik={genislik}
          baslik={uzunEtiketler?.[hover] ?? etiketler[hover]}
          satirlar={seriler
            .map((s) => ({ ad: s.ad, renk: s.renk, deger: s.degerler[hover] }))
            .filter((s) => s.deger > 0)
            .sort((a, b) => b.deger - a.deger)}
          alt={`Toplam: ${sayiFormat(toplamlar[hover])}`}
        />
      )}
    </div>
  );
}
