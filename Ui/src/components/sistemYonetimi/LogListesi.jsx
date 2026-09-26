import { useCallback, useEffect, useMemo, useState } from 'react';
import { parametreService } from '../../services/parametreService';
import { getApiErrorMessage } from '../../utils/apiError';
import './LogListesi.css';
import Bildirim from '../bildirim/Bildirim';

function yerelTarih(d) {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function tarihSaatFormat(deger) {
  if (!deger) return '-';
  return new Date(deger).toLocaleString('tr-TR', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit',
  });
}

export default function LogListesi() {
  const [baslangic, setBaslangic] = useState(yerelTarih(new Date()));
  const [bitis, setBitis] = useState(yerelTarih(new Date()));
  const [aksiyonTipi, setAksiyonTipi] = useState('');
  const [arama, setArama] = useState('');
  const [tipler, setTipler] = useState([]);
  const [loglar, setLoglar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');

  const aralikGecersiz = baslangic > bitis;

  useEffect(() => {
    parametreService
      .logTipleriniGetir()
      .then(({ data }) => setTipler(data || []))
      .catch(() => {});
  }, []);

  const yukle = useCallback(async () => {
    if (aralikGecersiz) return;
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await parametreService.loglariGetir(baslangic, bitis, aksiyonTipi);
      setLoglar(data || []);
    } catch (err) {
      setLoglar([]);
      setHata(getApiErrorMessage(err, 'Loglar yüklenemedi'));
    } finally {
      setYukleniyor(false);
    }
  }, [baslangic, bitis, aksiyonTipi, aralikGecersiz]);

  useEffect(() => {
    // eslint-disable-next-line react/set-state-in-effect
    yukle();
  }, [yukle]);

  const bugunSec = () => {
    setBaslangic(yerelTarih(new Date()));
    setBitis(yerelTarih(new Date()));
  };

  const sonYediGunSec = () => {
    const gecmis = new Date();
    gecmis.setDate(gecmis.getDate() - 6);
    setBaslangic(yerelTarih(gecmis));
    setBitis(yerelTarih(new Date()));
  };

  const gosterilecek = useMemo(() => {
    const metin = arama.trim().toLocaleLowerCase('tr-TR');
    if (!metin) return loglar;
    return loglar.filter((l) =>
      [l.kullaniciAdi, l.entityTipi, l.detay, l.ipAdresi, l.entityId]
        .some((alan) => String(alan ?? '').toLocaleLowerCase('tr-TR').includes(metin))
    );
  }, [loglar, arama]);

  return (
    <div className="log-kart">
      <div className="log-filtre">
        <label className="log-alan">
          <span>Başlangıç Tarihi</span>
          <input type="date" value={baslangic} onChange={(e) => setBaslangic(e.target.value)} />
        </label>
        <label className="log-alan">
          <span>Bitiş Tarihi</span>
          <input type="date" value={bitis} onChange={(e) => setBitis(e.target.value)} />
        </label>
        <label className="log-alan">
          <span>Log Tipi</span>
          <select value={aksiyonTipi} onChange={(e) => setAksiyonTipi(e.target.value)}>
            <option value="">Tümü</option>
            {tipler.map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
        </label>
        <label className="log-alan log-alan-arama">
          <span>Ara (kullanıcı, detay, IP...)</span>
          <input
            type="text"
            value={arama}
            onChange={(e) => setArama(e.target.value)}
            placeholder="Sonuçlar içinde ara"
          />
        </label>
        <div className="log-hizli">
          <button type="button" className="log-btn" onClick={bugunSec}>Bugün</button>
          <button type="button" className="log-btn" onClick={sonYediGunSec}>Son 7 Gün</button>
          <button type="button" className="log-btn log-btn-birincil" onClick={yukle} disabled={yukleniyor}>
            {yukleniyor ? '...' : 'Yenile'}
          </button>
        </div>
      </div>

      {aralikGecersiz && <Bildirim mesaj="Başlangıç tarihi bitiş tarihinden büyük olamaz." tip="uyari" />}
      {!aralikGecersiz && <Bildirim mesaj={hata} />}

      {!aralikGecersiz && !hata && !yukleniyor && (
        <p className="log-ozet">
          {gosterilecek.length} kayıt listeleniyor
          {loglar.length >= 1000 && ' (en yeni 1000 kayıtla sınırlı — aralığı daraltın)'}
        </p>
      )}

      {yukleniyor && <p className="log-bos">Yükleniyor...</p>}

      {!yukleniyor && !aralikGecersiz && !hata && gosterilecek.length === 0 && (
        <p className="log-bos">Seçilen filtrelerde log kaydı bulunamadı.</p>
      )}

      {!yukleniyor && gosterilecek.length > 0 && (
        <div className="log-tablo-kapsayici">
          <table className="log-tablo">
            <thead>
              <tr>
                <th>Tarih</th>
                <th>Log Tipi</th>
                <th>Kullanıcı</th>
                <th>Tablo</th>
                <th>Kayıt ID</th>
                <th>Detay</th>
                <th>IP Adresi</th>
              </tr>
            </thead>
            <tbody>
              {gosterilecek.map((l) => (
                <tr key={l.id}>
                  <td className="log-tarih">{tarihSaatFormat(l.olusturmaTarihi)}</td>
                  <td><span className="log-rozet">{l.aksiyonTipi}</span></td>
                  <td>{l.kullaniciAdi || (l.userId ? `#${l.userId}` : '-')}</td>
                  <td>{l.entityTipi || '-'}</td>
                  <td>{l.entityId ?? '-'}</td>
                  <td className="log-detay">{l.detay || '-'}</td>
                  <td>{l.ipAdresi || '-'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
