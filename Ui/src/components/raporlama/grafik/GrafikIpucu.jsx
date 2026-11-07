import { sayiFormat } from './grafikYardimci';

// Grafik üzerinde fareyle gezinirken gösterilen kutu. x, kapsayıcıya göre
// piksel; sağ kenara yaklaşınca kutu imlecin soluna geçer.
export default function GrafikIpucu({ x, y, kapsayiciGenislik, baslik, satirlar, alt }) {
  const solda = x > kapsayiciGenislik * 0.6;
  return (
    <div
      className="grf-ipucu"
      style={{
        left: solda ? undefined : x + 14,
        right: solda ? kapsayiciGenislik - x + 14 : undefined,
        top: y,
      }}
      role="status"
    >
      {baslik && <div className="grf-ipucu-baslik">{baslik}</div>}
      {satirlar.map((s) => (
        <div className="grf-ipucu-satir" key={s.ad}>
          {s.renk && <span className="grf-ipucu-isaret" style={{ background: s.renk }} />}
          <span className="grf-ipucu-ad">{s.ad}</span>
          <span className="grf-ipucu-deger">{sayiFormat(s.deger)}</span>
        </div>
      ))}
      {alt && <div className="grf-ipucu-alt">{alt}</div>}
    </div>
  );
}
