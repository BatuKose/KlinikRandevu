import { useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import '../hastaKayit/HastaKayitModal.css';
import Bildirim from '../bildirim/Bildirim';

function yerelTarih(d) {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function fiyatFormat(deger) {
  return (deger ?? 0).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' });
}

export default function TaahutnameModal({ kayit, muayeneId, borc, onKapat, onBasarili }) {
  const duzenleme = !!kayit;
  const [sonOdemeTarihi, setSonOdemeTarihi] = useState(duzenleme ? kayit.sonOdemeTarihi.slice(0, 10) : '');
  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const minTarih = duzenleme ? kayit.tahütTarihi.slice(0, 10) : yerelTarih(new Date());

  const submit = async (e) => {
    e.preventDefault();
    setHata('');
    setKaydediliyor(true);
    try {
      if (duzenleme) {
        await muayeneService.taahutnameGuncelle({ id: kayit.id, sonOdemeTarihi });
      } else {
        await muayeneService.taahutnameEkle({ muayeneId: Number(muayeneId), sonOdemeTarihi });
      }
      onBasarili();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Taahütname kaydedilemedi'));
    } finally {
      setKaydediliyor(false);
    }
  };

  return (
    <div className="hkm-overlay" onClick={onKapat}>
      <div className="hkm-kart" onClick={(e) => e.stopPropagation()}>
        <div className="hkm-baslik">
          <div>
            <h2>{duzenleme ? 'Taahütname Düzenle' : 'Yeni Taahütname'}</h2>
            <p>Muayene #{duzenleme ? kayit.muayeneId : muayeneId}</p>
          </div>
          <button className="hkm-kapat-btn" onClick={onKapat} type="button">✕</button>
        </div>

        <form onSubmit={submit}>
          <div className="mk-odenecek-tutar">
            <span>{duzenleme ? 'Taahüt Edilen Borç' : 'Kalan Borç'}</span>
            <strong>{fiyatFormat(duzenleme ? kayit.toplamBorc : borc)}</strong>
          </div>

          <label className="hkm-alan">
            <span>Son Ödeme Tarihi</span>
            <input
              type="date"
              value={sonOdemeTarihi}
              min={minTarih}
              onChange={(e) => setSonOdemeTarihi(e.target.value)}
              required
              autoFocus
            />
          </label>

          {duzenleme && (
            <p className="mk-bos-metin">
              Son ödeme tarihi değişirse SMS/e-posta hatırlatması yeni tarihe göre tekrar gönderilir.
            </p>
          )}

          <Bildirim mesaj={hata} onKapat={() => setHata('')} />

          <div className="hkm-footer">
            <button type="button" className="hkm-btn" onClick={onKapat} disabled={kaydediliyor}>
              Vazgeç
            </button>
            <button type="submit" className="hkm-btn hkm-btn-birincil" disabled={kaydediliyor}>
              {kaydediliyor ? 'Kaydediliyor...' : duzenleme ? 'Güncelle' : 'Taahütname Ekle'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
