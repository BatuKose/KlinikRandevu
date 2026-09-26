import { useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from './useDoktorVeServisListesi';
import './HastaKayitModal.css';
import SlotSecici from '../randevu/SlotSecici';
import Bildirim from '../bildirim/Bildirim';

export default function RandevuVerModal({ hasta, onKapat, onBasarili }) {
  const { doktorlar, servisler, yukleniyor: listeYukleniyor, hata: listeHata } = useDoktorVeServisListesi();

  const [doktorNo, setDoktorNo] = useState('');
  const [polNo, setPolNo] = useState('');
  const [slot, setSlot] = useState(null);
  const [notlar, setNotlar] = useState('');
  const [bekleme, setBekleme] = useState(false);

  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    if (!slot) {
      setHata('Lütfen bir randevu saati seçiniz');
      return;
    }
    setHata('');
    setKaydediliyor(true);
    try {
      await muayeneService.randevuOlustur({
        doktorNo: Number(doktorNo),
        polNo: Number(polNo),
        hastaTc: hasta.tcKimlik,
        protocolNo: hasta.protocol,
        randevuTarihi: slot.baslangic,
        sureDakika: slot.sureDk,
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

          <SlotSecici
            doktorNo={doktorNo}
            polNo={polNo}
            secili={slot}
            onSec={setSlot}
            beklemeIzinli={bekleme}
          />

          <label className="hkm-alan">
            <span>Notlar</span>
            <textarea rows={2} value={notlar} onChange={(e) => setNotlar(e.target.value)} />
          </label>

          <label className="hkm-alan hkm-checkbox">
            <input
              type="checkbox"
              checked={bekleme}
              onChange={(e) => {
                setBekleme(e.target.checked);
                if (!e.target.checked && slot?.dolu) setSlot(null);
              }}
            />
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
