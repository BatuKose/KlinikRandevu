import { bugun } from '../../utils/tarih';
import Bildirim from '../bildirim/Bildirim';
import Ikon from '../ui/Ikon';
import { BosDurum } from '../ui/Ortak';
import { HAZIR_ARALIKLAR } from './raporYardimci';

// useTarihliRapor ile birlikte kullanılan ortak filtre çubuğu.
// onCsv verilirse CSV butonu çıkar; children ile rapora özel alan eklenebilir.
export default function TarihAraligiFiltresi({ rapor, onCsv, csvAktif, children }) {
  const { baslangic, setBaslangic, bitis, setBitis, yukleniyor, hata, setHata, aralikHatasi, yukle } = rapor;
  const aktifHazir = HAZIR_ARALIKLAR.find((h) => h.baslangic() === baslangic && bitis === bugun())?.id;

  return (
    <>
      <Bildirim mesaj={hata} onKapat={() => setHata('')} />
      {aralikHatasi && <Bildirim mesaj={aralikHatasi} tip="uyari" />}

      <section className="ui-kart">
        <div className="ui-arac-cubugu rp-filtre">
          <label className="ui-alan">
            <span>Başlangıç Tarihi</span>
            <input type="date" className="ui-input" value={baslangic} max={bitis} onChange={(e) => setBaslangic(e.target.value)} />
          </label>
          <label className="ui-alan">
            <span>Bitiş Tarihi</span>
            <input type="date" className="ui-input" value={bitis} min={baslangic} onChange={(e) => setBitis(e.target.value)} />
          </label>
          <div className="ui-segment rp-hazir" role="group" aria-label="Hazır tarih aralıkları">
            {HAZIR_ARALIKLAR.map((h) => (
              <button
                type="button"
                key={h.id}
                className={aktifHazir === h.id ? 'ui-segment-aktif' : ''}
                onClick={() => {
                  setBaslangic(h.baslangic());
                  setBitis(bugun());
                }}
              >
                {h.etiket}
              </button>
            ))}
          </div>
          {children}
          <div className="ui-arac-bosluk" />
          {onCsv && (
            <button type="button" className="ui-btn" onClick={onCsv} disabled={!csvAktif}>
              <Ikon ad="dosya" /> CSV
            </button>
          )}
          <button type="button" className="ui-btn ui-btn-birincil" onClick={yukle} disabled={yukleniyor || !!aralikHatasi}>
            <Ikon ad="yenile" /> {yukleniyor ? 'Yükleniyor...' : 'Yenile'}
          </button>
        </div>
      </section>
    </>
  );
}

// Yükleniyor / boş durumları raporlar arasında aynı görünsün diye.
export function RaporDurumu({ rapor, veriVar, bosBaslik }) {
  const { yukleniyor, aralikHatasi, hata } = rapor;
  if (yukleniyor && !veriVar) {
    return (
      <div className="ui-kart rp-yukleniyor" aria-busy="true">
        <span className="ui-spinner" /> Rapor hazırlanıyor...
      </div>
    );
  }
  if (!yukleniyor && !aralikHatasi && !hata && !veriVar) {
    return (
      <div className="ui-kart">
        <BosDurum ikon="grafik" baslik={bosBaslik} aciklama="Farklı bir tarih aralığı seçerek tekrar deneyin." />
      </div>
    );
  }
  return null;
}
