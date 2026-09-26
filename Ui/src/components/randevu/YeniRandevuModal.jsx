import { useState } from 'react';
import { hastaService } from '../../services/hastaService';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from '../hastaKayit/useDoktorVeServisListesi';
import '../hastaKayit/HastaKayitModal.css';
import Bildirim from '../bildirim/Bildirim';

export default function YeniRandevuModal({ onKapat, onBasarili }) {
  const { doktorlar, servisler, yukleniyor: listeYukleniyor, hata: listeHata } = useDoktorVeServisListesi();

  const [arama, setArama] = useState('');
  const [aramaYukleniyor, setAramaYukleniyor] = useState(false);
  const [aramaHata, setAramaHata] = useState('');
  const [hasta, setHasta] = useState(null);

  const [doktorNo, setDoktorNo] = useState('');
  const [polNo, setPolNo] = useState('');
  const [randevuTarihi, setRandevuTarihi] = useState('');
  const [sureDakika, setSureDakika] = useState('15');
  const [notlar, setNotlar] = useState('');
  const [bekleme, setBekleme] = useState(false);

  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const aramaYap = async () => {
    const metin = arama.trim();
    if (metin.length < 3) {
      setAramaHata('Arama metni en az 3 karakter olmalıdır');
      return;
    }
    setAramaHata('');
    setAramaYukleniyor(true);
    try {
      const { data } = await hastaService.ara(metin);
      const bulunan = data?.[0];
      if (!bulunan) {
        setAramaHata('Hasta kaydı bulunamadı');
        return;
      }
      setHasta(bulunan);
    } catch (err) {
      setAramaHata(getApiErrorMessage(err, 'Hasta kaydı bulunamadı'));
    } finally {
      setAramaYukleniyor(false);
    }
  };

  const aramaKeyDown = (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      aramaYap();
    }
  };

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
            <h2>Yeni Randevu</h2>
            {hasta && <p>{hasta.name} {hasta.surname} · Protokol {hasta.protocol}</p>}
          </div>
          <button className="hkm-kapat-btn" onClick={onKapat} type="button">✕</button>
        </div>

        {!hasta && (
          <div className="hkm-arama-bolum">
            <div className="hkm-arama-satir">
              <input
                type="text"
                placeholder="Protokol, TC veya isimle ara ve Enter'a bas..."
                value={arama}
                onChange={(e) => setArama(e.target.value)}
                onKeyDown={aramaKeyDown}
                disabled={aramaYukleniyor}
                autoFocus
              />
              <button type="button" onClick={aramaYap} disabled={aramaYukleniyor} className="hkm-btn hkm-btn-birincil">
                {aramaYukleniyor ? '...' : 'Ara'}
              </button>
            </div>
            <Bildirim mesaj={aramaHata} onKapat={() => setAramaHata('')} />
          </div>
        )}

        {hasta && (
          <div className="hkm-secili-hasta">
            <span>TC {hasta.tcKimlik}</span>
            <button type="button" className="hkm-link-btn" onClick={() => setHasta(null)}>
              Farklı hasta seç
            </button>
          </div>
        )}

        {hasta && (
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
        )}
      </div>
    </div>
  );
}
