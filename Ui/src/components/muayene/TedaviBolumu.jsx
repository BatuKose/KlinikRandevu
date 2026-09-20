import { useEffect, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { ODEME_TIPI } from '../../utils/muayeneSecenekleri';

function fiyatFormat(deger) {
  return (deger ?? 0).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' });
}

export default function TedaviBolumu({ muayeneId, kapali, onDegisti }) {
  const [tedaviler, setTedaviler] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(true);
  const [hata, setHata] = useState('');

  const [aramaMetni, setAramaMetni] = useState('');
  const [ekleniyor, setEkleniyor] = useState(false);
  const [ekleHata, setEkleHata] = useState('');
  const [iptalEdilenId, setIptalEdilenId] = useState(null);

  const listeyiYukle = async () => {
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await muayeneService.tedavileriGetir(muayeneId);
      setTedaviler(data?.data || []);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Tedaviler alınamadı'));
    } finally {
      setYukleniyor(false);
    }
  };

  useEffect(() => {
    listeyiYukle();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [muayeneId]);

  const tedaviEkle = async (e) => {
    e.preventDefault();
    if (!aramaMetni.trim()) return;
    setEkleniyor(true);
    setEkleHata('');
    try {
      await muayeneService.tedaviEkle({ muyaneId: muayeneId, tedaviKodu: null, tedaviAdi: aramaMetni.trim() });
      setAramaMetni('');
      await listeyiYukle();
      onDegisti?.();
    } catch (err) {
      setEkleHata(getApiErrorMessage(err, 'Tedavi eklenemedi'));
    } finally {
      setEkleniyor(false);
    }
  };

  const odemeyiIptalEt = async (tedaviId) => {
    if (!window.confirm('Bu tedavinin ödemesini iptal etmek istediğine emin misin?')) return;
    setIptalEdilenId(tedaviId);
    setEkleHata('');
    try {
      await muayeneService.odemeIptalEt({
        tedaviId,
        muyaneId: muayeneId,
        odemeIptalTipi: ODEME_TIPI.TEDAVI_BAZLI,
      });
      await listeyiYukle();
      onDegisti?.();
    } catch (err) {
      setEkleHata(getApiErrorMessage(err, 'Ödeme iptal edilemedi'));
    } finally {
      setIptalEdilenId(null);
    }
  };

  return (
    <div className="mk-panel mk-bolum">
      <h2>Tedaviler</h2>

      {yukleniyor && <p className="mk-bos-metin">Yükleniyor...</p>}
      {!yukleniyor && hata && <div className="mk-hata">{hata}</div>}
      {!yukleniyor && !hata && tedaviler.length === 0 && (
        <p className="mk-bos-metin">Henüz tedavi eklenmemiş.</p>
      )}
      {tedaviler.length > 0 && (
        <ul className="mk-liste">
          {tedaviler.map((t) => (
            <li key={t.id} className="mk-liste-item mk-liste-item-genis">
              <div>
                <span className="mk-kod">{t.tedaviKodu}</span>
                <span>{t.tedaviAdi}</span>
              </div>
              <div className="mk-liste-item-sag">
                <span className={`mk-durum-rozet ${t.odendi ? 'mk-durum-odendi' : 'mk-durum-bekliyor'}`}>
                  {t.odendi ? 'Ödendi' : 'Bekliyor'}
                </span>
                <span className="mk-fiyat">{fiyatFormat(t.fiyat)}</span>
                {t.odendi && (
                  <button
                    type="button"
                    className="mk-mini-btn"
                    onClick={() => odemeyiIptalEt(t.id)}
                    disabled={iptalEdilenId === t.id}
                  >
                    {iptalEdilenId === t.id ? '...' : 'Ödemeyi İptal Et'}
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}

      {kapali ? (
        <p className="mk-bos-metin mk-uyari-metin">Muayene kapalı, tedavi eklenemez.</p>
      ) : (
        <form className="mk-ekle-form" onSubmit={tedaviEkle}>
          <input
            type="text"
            placeholder="Tedavi kodu veya adı ile ara..."
            value={aramaMetni}
            onChange={(e) => setAramaMetni(e.target.value)}
            disabled={ekleniyor}
          />
          <button type="submit" className="mk-btn mk-btn-birincil" disabled={ekleniyor || !aramaMetni.trim()}>
            {ekleniyor ? '...' : 'Ekle'}
          </button>
        </form>
      )}
      {ekleHata && <div className="mk-hata">{ekleHata}</div>}
    </div>
  );
}
