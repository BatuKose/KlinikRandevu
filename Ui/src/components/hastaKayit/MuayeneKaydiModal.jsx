import { useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from './useDoktorVeServisListesi';
import './HastaKayitModal.css';

export default function MuayeneKaydiModal({ hasta, onKapat, onBasarili }) {
  const { doktorlar, servisler, yukleniyor: listeYukleniyor, hata: listeHata } = useDoktorVeServisListesi();

  const [doktorNo, setDoktorNo] = useState('');
  const [polNo, setPolNo] = useState('');
  const [muayeneTarihi, setMuayeneTarihi] = useState('');
  const [baslangicSaati, setBaslangicSaati] = useState('');

  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    setHata('');
    setKaydediliyor(true);
    try {
      await muayeneService.muayeneOlustur({
        protocolNo: hasta.protocol,
        doktorNo: Number(doktorNo),
        polNo: Number(polNo),
        hastaTc: hasta.tcKimlik,
        muayeneTarihi,
        baslangicSaati: `${baslangicSaati}:00`,
      });
      onBasarili();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Muayene kaydı oluşturulamadı'));
    } finally {
      setKaydediliyor(false);
    }
  };

  return (
    <div className="hkm-overlay" onClick={onKapat}>
      <div className="hkm-kart" onClick={(e) => e.stopPropagation()}>
        <div className="hkm-baslik">
          <div>
            <h2>Muayene Kaydı Aç</h2>
            <p>{hasta.name} {hasta.surname} · Protokol {hasta.protocol}</p>
          </div>
          <button className="hkm-kapat-btn" onClick={onKapat} type="button">✕</button>
        </div>

        <form onSubmit={submit}>
          {listeHata && <div className="hkm-hata">{listeHata}</div>}

          <div className="hkm-satir-2">
            <label className="hkm-alan">
              <span>Doktor</span>
              <select
                value={doktorNo}
                onChange={(e) => setDoktorNo(e.target.value)}
                required
                disabled={listeYukleniyor}
              >
                <option value="">{listeYukleniyor ? 'Yükleniyor...' : 'Seçiniz'}</option>
                {doktorlar.map((d) => (
                  <option key={d.doktorNo} value={d.doktorNo}>{d.doktorAd}</option>
                ))}
              </select>
            </label>
            <label className="hkm-alan">
              <span>Poliklinik</span>
              <select
                value={polNo}
                onChange={(e) => setPolNo(e.target.value)}
                required
                disabled={listeYukleniyor}
              >
                <option value="">{listeYukleniyor ? 'Yükleniyor...' : 'Seçiniz'}</option>
                {servisler.map((s) => (
                  <option key={s.servisNo} value={s.servisNo}>{s.servisAdi}</option>
                ))}
              </select>
            </label>
          </div>

          <div className="hkm-satir-2">
            <label className="hkm-alan">
              <span>Muayene Tarihi</span>
              <input
                type="date"
                value={muayeneTarihi}
                onChange={(e) => setMuayeneTarihi(e.target.value)}
                required
              />
            </label>
            <label className="hkm-alan">
              <span>Başlangıç Saati</span>
              <input
                type="time"
                value={baslangicSaati}
                onChange={(e) => setBaslangicSaati(e.target.value)}
                required
              />
            </label>
          </div>

          {hata && <div className="hkm-hata">{hata}</div>}

          <div className="hkm-footer">
            <button type="button" className="hkm-btn" onClick={onKapat} disabled={kaydediliyor}>
              Vazgeç
            </button>
            <button type="submit" className="hkm-btn hkm-btn-birincil" disabled={kaydediliyor}>
              {kaydediliyor ? 'Kaydediliyor...' : 'Muayene Kaydı Aç'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
