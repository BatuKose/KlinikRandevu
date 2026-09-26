import { useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from './useDoktorVeServisListesi';
import './HastaKayitModal.css';
import Bildirim from '../bildirim/Bildirim';

export default function RandevuVerModal({ hasta, onKapat, onBasarili }) {
  const { doktorlar, servisler, yukleniyor: listeYukleniyor, hata: listeHata } = useDoktorVeServisListesi();

  const [doktorNo, setDoktorNo] = useState('');
  const [polNo, setPolNo] = useState('');
  const [randevuTarihi, setRandevuTarihi] = useState('');
  const [sureDakika, setSureDakika] = useState('15');
  const [notlar, setNotlar] = useState('');
  const [bekleme, setBekleme] = useState(false);

  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    setHata('');
    setKaydediliyor(true);
    try {
      await muayeneService.randevuOlustur({
        doktorNo: Number(doktorNo),
        polNo: Number(polNo),
        hastaTc: hasta.tcKimlik,
        protocolNo: hasta.protocol,
        randevuTarihi,
        sureDakika: Number(sureDakika),
        notlar: notlar.trim() || null,
        randevuBekleme: bekleme,
      });
      onBasarili();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Randevu oluşturulamadı'));
    } finally {
      setKaydediliyor(false);
    }
  };

  return (
    <div className="hkm-overlay" onClick={onKapat}>
      <div className="hkm-kart" onClick={(e) => e.stopPropagation()}>
        <div className="hkm-baslik">
          <div>
            <h2>Randevu Ver</h2>
            <p>{hasta.name} {hasta.surname} · Protokol {hasta.protocol}</p>
          </div>
          <button className="hkm-kapat-btn" onClick={onKapat} type="button">✕</button>
        </div>

        <form onSubmit={submit}>
          <Bildirim mesaj={listeHata} />

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

          <label className="hkm-alan">
            <span>Randevu Tarihi ve Saati</span>
            <input
              type="datetime-local"
              value={randevuTarihi}
              onChange={(e) => setRandevuTarihi(e.target.value)}
              required
            />
          </label>

          <label className="hkm-alan">
            <span>Süre (Dakika)</span>
            <input
              type="number"
              min="1"
              value={sureDakika}
              onChange={(e) => setSureDakika(e.target.value)}
              required
            />
          </label>

          <label className="hkm-alan">
            <span>Notlar</span>
            <textarea rows={2} value={notlar} onChange={(e) => setNotlar(e.target.value)} />
          </label>

          <label className="hkm-alan hkm-checkbox">
            <input type="checkbox" checked={bekleme} onChange={(e) => setBekleme(e.target.checked)} />
            <span>Bekleme listesine ekle</span>
          </label>

          <Bildirim mesaj={hata} onKapat={() => setHata('')} />

          <div className="hkm-footer">
            <button type="button" className="hkm-btn" onClick={onKapat} disabled={kaydediliyor}>
              Vazgeç
            </button>
            <button type="submit" className="hkm-btn hkm-btn-birincil" disabled={kaydediliyor}>
              {kaydediliyor ? 'Kaydediliyor...' : 'Randevu Oluştur'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
