// Tıklanabilir lejant: seriyi gizler/gösterir. En az bir seri açık kalır.
// Tek seride lejant gösterilmez (başlık seriyi zaten adlandırır).
export default function SeriLejanti({ seriler, gizliSeriler, onDegis }) {
  if (seriler.length < 2) return null;

  const degistir = (ad) => {
    onDegis((onceki) => {
      const yeni = new Set(onceki);
      if (yeni.has(ad)) yeni.delete(ad);
      else if (seriler.length - yeni.size > 1) yeni.add(ad);
      return yeni;
    });
  };

  return (
    <div className="grf-lejant" aria-label="Seriler (göstermek / gizlemek için tıklayın)">
      {seriler.map((s) => {
        const gizli = gizliSeriler.has(s.ad);
        return (
          <button
            type="button"
            key={s.ad}
            className={`grf-lejant-oge${gizli ? ' grf-lejant-gizli' : ''}`}
            aria-pressed={!gizli}
            onClick={() => degistir(s.ad)}
            title={s.icerik ? `${s.icerik} branş daha` : undefined}
          >
            <span className="grf-lejant-isaret" style={{ background: s.renk }} />
            {s.ad}
            {s.icerik ? ` (${s.icerik})` : ''}
          </button>
        );
      })}
    </div>
  );
}
