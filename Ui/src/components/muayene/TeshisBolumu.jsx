import { useEffect, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import Bildirim from '../bildirim/Bildirim';

export default function TeshisBolumu({ muayeneId, kapali }) {
  const [teshisler, setTeshisler] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(true);
  const [hata, setHata] = useState('');

  const [aramaMetni, setAramaMetni] = useState('');
  const [ekleniyor, setEkleniyor] = useState(false);
  const [ekleHata, setEkleHata] = useState('');

  const listeyiYukle = async () => {
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await muayeneService.teshisleriGetir(muayeneId);
      setTeshisler(data?.data || []);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Teşhisler alınamadı'));
    } finally {
      setYukleniyor(false);
    }
  };

  useEffect(() => {
    listeyiYukle();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [muayeneId]);

  const teshisEkle = async (e) => {
    e.preventDefault();
    if (!aramaMetni.trim()) return;
    setEkleniyor(true);
    setEkleHata('');
    try {
      await muayeneService.teshisEkle(muayeneId, aramaMetni.trim());
      setAramaMetni('');
      await listeyiYukle();
    } catch (err) {
      setEkleHata(getApiErrorMessage(err, 'Teşhis eklenemedi'));
    } finally {
      setEkleniyor(false);
    }
  };

  return (
    <div className="mk-panel mk-bolum">
      <h2>Teşhisler</h2>

      {yukleniyor && <p className="mk-bos-metin">Yükleniyor...</p>}
      {!yukleniyor && <Bildirim mesaj={hata} />}
      {!yukleniyor && !hata && teshisler.length === 0 && (
        <p className="mk-bos-metin">Henüz teşhis eklenmemiş.</p>
      )}
      {teshisler.length > 0 && (
        <ul className="mk-liste">
          {teshisler.map((t) => (
            <li key={t.id} className="mk-liste-item">
              <span className="mk-kod">{t.teshisKod}</span>
              <span>{t.teshisAd}</span>
            </li>
          ))}
        </ul>
      )}

      {kapali ? (
        <p className="mk-bos-metin mk-uyari-metin">Muayene kapalı, teşhis eklenemez.</p>
      ) : (
        <form className="mk-ekle-form" onSubmit={teshisEkle}>
          <input
            type="text"
            placeholder="ICD kodu veya tanı adı ile ara..."
            value={aramaMetni}
            onChange={(e) => setAramaMetni(e.target.value)}
            disabled={ekleniyor}
          />
          <button type="submit" className="mk-btn mk-btn-birincil" disabled={ekleniyor || !aramaMetni.trim()}>
            {ekleniyor ? '...' : 'Ekle'}
          </button>
        </form>
      )}
      <Bildirim mesaj={ekleHata} onKapat={() => setEkleHata('')} />
    </div>
  );
}
