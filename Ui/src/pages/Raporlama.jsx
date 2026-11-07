import { useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { RAPORLAR, raporBul } from '../components/raporlama/raporKatalogu';
import Ikon from '../components/ui/Ikon';
import { SayfaBaslik, BosDurum } from '../components/ui/Ortak';
import './Raporlama.css';

export default function Raporlama() {
  const [params, setParams] = useSearchParams();
  const [arama, setArama] = useState('');

  // Seçili rapor URL'de tutulur; sayfa yenilense veya link paylaşılsa da aynı rapor açılır.
  const seciliRapor = raporBul(params.get('rapor')) ?? RAPORLAR[0];
  const raporSec = (id) => setParams({ rapor: id }, { replace: true });

  const gruplar = useMemo(() => {
    const metin = arama.trim().toLocaleLowerCase('tr-TR');
    const harita = new Map();
    for (const r of RAPORLAR) {
      const eslesir = !metin || `${r.baslik} ${r.aciklama} ${r.kategori}`.toLocaleLowerCase('tr-TR').includes(metin);
      if (!eslesir) continue;
      if (!harita.has(r.kategori)) harita.set(r.kategori, []);
      harita.get(r.kategori).push(r);
    }
    return [...harita.entries()];
  }, [arama]);

  const RaporBileseni = seciliRapor?.bilesen;

  return (
    <div className="sayfa">
      <SayfaBaslik ikon="grafik" baslik="Raporlama" aciklama="Görmek istediğin raporu soldan seç, tarih aralığını ayarla." />

      <div className="rp-duzen">
        <aside className="ui-kart rp-katalog" aria-label="Rapor listesi">
          <div className="rp-katalog-ust">
            <div className="rp-katalog-baslik">
              <span>Raporlar</span>
              <span className="ui-sayac">{RAPORLAR.length}</span>
            </div>
            <label className="rp-arama">
              <Ikon ad="ara" />
              <input
                type="search"
                className="ui-input"
                placeholder="Rapor ara"
                value={arama}
                onChange={(e) => setArama(e.target.value)}
              />
            </label>
          </div>

          <nav className="rp-katalog-liste">
            {gruplar.length === 0 && <p className="rp-katalog-bos">Eşleşen rapor yok.</p>}
            {gruplar.map(([kategori, raporlar]) => (
              <div className="rp-katalog-grup" key={kategori}>
                <span className="rp-katalog-kategori">{kategori}</span>
                {raporlar.map((r) => (
                  <button
                    type="button"
                    key={r.id}
                    className={`rp-katalog-oge${r.id === seciliRapor?.id ? ' rp-katalog-oge-aktif' : ''}`}
                    aria-current={r.id === seciliRapor?.id ? 'page' : undefined}
                    onClick={() => raporSec(r.id)}
                  >
                    <span className="rp-katalog-ikon">
                      <Ikon ad={r.ikon} />
                    </span>
                    <span className="rp-katalog-metin">
                      <strong>{r.baslik}</strong>
                      <small>{r.aciklama}</small>
                    </span>
                  </button>
                ))}
              </div>
            ))}
          </nav>
        </aside>

        <section className="rp-icerik">
          {seciliRapor ? (
            <>
              <div className="rp-rapor-baslik">
                <div>
                  <span className="ui-rozet ui-rozet-bilgi">{seciliRapor.kategori}</span>
                  <h2>{seciliRapor.baslik}</h2>
                  <p>{seciliRapor.aciklama}</p>
                </div>
              </div>
              {/* key: rapor değişince bileşen state'i (filtreler vb.) sıfırlansın */}
              <RaporBileseni key={seciliRapor.id} />
            </>
          ) : (
            <div className="ui-kart">
              <BosDurum ikon="grafik" baslik="Rapor seçin" aciklama="Soldaki listeden görüntülemek istediğiniz raporu seçin." />
            </div>
          )}
        </section>
      </div>
    </div>
  );
}
