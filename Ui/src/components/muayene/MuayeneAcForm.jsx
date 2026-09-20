import { useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from '../hastaKayit/useDoktorVeServisListesi';

function yerelTarih(d) {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function yerelSaat(d) {
  return `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;
}

export default function MuayeneAcForm({ tc, protokol, adSoyad, doktorAdi, poliklinikAdi, randevuTarihi, onOlusturuldu }) {
  const { doktorlar, servisler, yukleniyor: listeYukleniyor, hata: listeHata } = useDoktorVeServisListesi();

  const doktorEsle = doktorlar.find((d) => d.doktorAd === doktorAdi);
  const servisEsle = servisler.find((s) => s.servisAdi === poliklinikAdi);
  const randevuTarihObj = randevuTarihi ? new Date(randevuTarihi) : null;

  const [doktorNo, setDoktorNo] = useState('');
  const [polNo, setPolNo] = useState('');
  const [muayeneTarihi, setMuayeneTarihi] = useState(() => yerelTarih(randevuTarihObj || new Date()));
  const [baslangicSaati, setBaslangicSaati] = useState(() => yerelSaat(randevuTarihObj || new Date()));

  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const doktorSecili = doktorNo || (doktorEsle ? String(doktorEsle.doktorNo) : '');
  const polSecili = polNo || (servisEsle ? String(servisEsle.servisNo) : '');

  const submit = async (e) => {
    e.preventDefault();
    setHata('');
    setKaydediliyor(true);
    try {
      const { data } = await muayeneService.muayeneOlustur({
        protocolNo: protokol,
        doktorNo: Number(doktorSecili),
        polNo: Number(polSecili),
        hastaTc: tc,
        muayeneTarihi,
        baslangicSaati: `${baslangicSaati}:00`,
      });
      onOlusturuldu(data.id);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Muayene kaydı oluşturulamadı'));
    } finally {
      setKaydediliyor(false);
    }
  };

  return (
    <div className="mk-panel mk-ac-form">
      <h2>Muayene Kaydı Aç</h2>
      <p className="mk-alt-baslik">{adSoyad} · Protokol {protokol}</p>

      {listeHata && <div className="mk-hata">{listeHata}</div>}

      <form onSubmit={submit}>
        <div className="mk-satir-2">
          <label className="mk-alan">
            <span>Doktor</span>
            <select value={doktorSecili} onChange={(e) => setDoktorNo(e.target.value)} required disabled={listeYukleniyor}>
              <option value="">{listeYukleniyor ? 'Yükleniyor...' : 'Seçiniz'}</option>
              {doktorlar.map((d) => (
                <option key={d.doktorNo} value={d.doktorNo}>{d.doktorAd}</option>
              ))}
            </select>
          </label>
          <label className="mk-alan">
            <span>Poliklinik</span>
            <select value={polSecili} onChange={(e) => setPolNo(e.target.value)} required disabled={listeYukleniyor}>
              <option value="">{listeYukleniyor ? 'Yükleniyor...' : 'Seçiniz'}</option>
              {servisler.map((s) => (
                <option key={s.servisNo} value={s.servisNo}>{s.servisAdi}</option>
              ))}
            </select>
          </label>
        </div>

        <div className="mk-satir-2">
          <label className="mk-alan">
            <span>Muayene Tarihi</span>
            <input type="date" value={muayeneTarihi} onChange={(e) => setMuayeneTarihi(e.target.value)} required />
          </label>
          <label className="mk-alan">
            <span>Başlangıç Saati</span>
            <input type="time" value={baslangicSaati} onChange={(e) => setBaslangicSaati(e.target.value)} required />
          </label>
        </div>

        {hata && <div className="mk-hata">{hata}</div>}

        <button type="submit" className="mk-btn mk-btn-birincil" disabled={kaydediliyor}>
          {kaydediliyor ? 'Kaydediliyor...' : 'Muayene Kaydı Aç'}
        </button>
      </form>
    </div>
  );
}
