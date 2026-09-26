import { useEffect, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import Bildirim from '../bildirim/Bildirim';
import './SlotSecici.css';

function bugun() {
  const d = new Date();
  return new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 10);
}

function saatEtiketi(iso) {
  return iso.slice(11, 16);
}

// Doktorun çalışma planından üretilen slotları gösterir. Dolu slotlar sadece
// "bekleme listesine ekle" seçiliyken seçilebilir (backend o durumda hastayı bekletmeye alıyor).
export default function SlotSecici({ doktorNo, polNo, secili, onSec, beklemeIzinli }) {
  const [tarih, setTarih] = useState(bugun());
  const [slotlar, setSlotlar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');

  const hazir = doktorNo && polNo && tarih;

  useEffect(() => {
    if (!hazir) return;
    let iptal = false;
    (async () => {
      setYukleniyor(true);
      setHata('');
      setSlotlar([]);
      onSec(null);
      try {
        const { data } = await muayeneService.musaitSlotlariGetir(doktorNo, polNo, tarih);
        if (!iptal) setSlotlar(data?.data || []);
      } catch (err) {
        if (!iptal) setHata(getApiErrorMessage(err, 'Uygun saatler alınamadı'));
      } finally {
        if (!iptal) setYukleniyor(false);
      }
    })();
    return () => {
      iptal = true;
    };
    // onSec parent'ta her render'da yeniden oluşuyor; sadece seçim kriterleri değişince yükle.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [doktorNo, polNo, tarih, hazir]);

  const bosSayisi = slotlar.filter((s) => !s.dolu && !s.gecmis).length;

  return (
    <div className="slot-secici">
      <label className="hkm-alan">
        <span>Randevu Günü</span>
        <input type="date" value={tarih} min={bugun()} onChange={(e) => setTarih(e.target.value)} required />
      </label>

      <div className="hkm-alan">
        <span>Randevu Saati</span>
        {!hazir && <p className="slot-bilgi">Saatleri görmek için doktor ve poliklinik seçiniz.</p>}
        {hazir && yukleniyor && <p className="slot-bilgi">Uygun saatler yükleniyor...</p>}
        {hazir && !yukleniyor && !hata && slotlar.length === 0 && (
          <p className="slot-bilgi slot-bilgi-uyari">
            Doktorun bu gün için bu poliklinikte çalışma planı yok. Randevu verebilmek için önce
            Randevu → Çalışma Planları ekranından plan oluşturunuz.
          </p>
        )}
        {hazir && !yukleniyor && slotlar.length > 0 && (
          <>
            <div className="slot-grid">
              {slotlar.map((s) => {
                const secilebilir = !s.gecmis && (!s.dolu || beklemeIzinli);
                const aktif = secili?.baslangic === s.baslangic;
                return (
                  <button
                    key={s.baslangic}
                    type="button"
                    className={`slot-btn${s.dolu ? ' slot-dolu' : ''}${s.gecmis ? ' slot-gecmis' : ''}${aktif ? ' slot-secili' : ''}`}
                    disabled={!secilebilir}
                    onClick={() => onSec(s)}
                    title={s.gecmis ? 'Geçmiş saat' : s.dolu ? 'Dolu' : `${s.sureDk} dk`}
                  >
                    {saatEtiketi(s.baslangic)}
                  </button>
                );
              })}
            </div>
            <p className="slot-bilgi">
              {bosSayisi} boş saat · slot süresi {slotlar[0].sureDk} dk
              {!beklemeIzinli && slotlar.some((s) => s.dolu && !s.gecmis) && ' · dolu saatler için "Bekleme listesine ekle"yi işaretleyin'}
            </p>
          </>
        )}
      </div>
      <Bildirim mesaj={hata} />
    </div>
  );
}
