import { useMemo, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { ODEME_TIPI } from '../../utils/muayeneSecenekleri';
import '../hastaKayit/HastaKayitModal.css';
import Bildirim from '../bildirim/Bildirim';

function fiyatFormat(deger) {
  return (deger ?? 0).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' });
}

export default function OdemeYapModal({ muayeneId, borc, odenmemisTedaviler, onKapat, onBasarili }) {
  const [mod, setMod] = useState(ODEME_TIPI.TOPLAM_MUAYENE);
  const [seciliTedaviId, setSeciliTedaviId] = useState('');

  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const seciliTedavi = useMemo(
    () => odenmemisTedaviler.find((t) => String(t.id) === seciliTedaviId),
    [odenmemisTedaviler, seciliTedaviId]
  );

  const odenecekTutar = mod === ODEME_TIPI.TOPLAM_MUAYENE ? borc : seciliTedavi?.fiyat ?? 0;
  const odenebilir = mod === ODEME_TIPI.TOPLAM_MUAYENE ? borc > 0 : !!seciliTedavi;

  const submit = async (e) => {
    e.preventDefault();
    if (!odenebilir) return;
    setHata('');
    setKaydediliyor(true);
    try {
      await muayeneService.odemeYap({
        muayeneId: Number(muayeneId),
        odemeToplam: odenecekTutar,
        tedaviId: mod === ODEME_TIPI.TEDAVI_BAZLI ? Number(seciliTedaviId) : 0,
        odeme: mod,
      });
      onBasarili();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Ödeme alınamadı'));
    } finally {
      setKaydediliyor(false);
    }
  };

  return (
    <div className="hkm-overlay" onClick={onKapat}>
      <div className="hkm-kart" onClick={(e) => e.stopPropagation()}>
        <div className="hkm-baslik">
          <div>
            <h2>Ödeme Yap</h2>
            <p>Muayene kaydına ödeme al</p>
          </div>
          <button className="hkm-kapat-btn" onClick={onKapat} type="button">✕</button>
        </div>

        <form onSubmit={submit}>
          <label className="hkm-alan">
            <span>Ödeme Türü</span>
            <select value={mod} onChange={(e) => { setMod(Number(e.target.value)); setSeciliTedaviId(''); }}>
              <option value={ODEME_TIPI.TOPLAM_MUAYENE}>Muayenenin Tüm Borcu</option>
              <option value={ODEME_TIPI.TEDAVI_BAZLI}>Belirli Bir Tedavi</option>
            </select>
          </label>

          {mod === ODEME_TIPI.TEDAVI_BAZLI && (
            <label className="hkm-alan">
              <span>Tedavi</span>
              <select value={seciliTedaviId} onChange={(e) => setSeciliTedaviId(e.target.value)} required>
                <option value="">Seçiniz</option>
                {odenmemisTedaviler.map((t) => (
                  <option key={t.id} value={t.id}>{t.tedaviAdi} · {fiyatFormat(t.fiyat)}</option>
                ))}
              </select>
              {odenmemisTedaviler.length === 0 && (
                <span className="mk-bos-metin">Ödenmemiş tedavi bulunmuyor.</span>
              )}
            </label>
          )}

          <div className="mk-odenecek-tutar">
            <span>Ödenecek Tutar</span>
            <strong>{fiyatFormat(odenecekTutar)}</strong>
          </div>

          <Bildirim mesaj={hata} onKapat={() => setHata('')} />

          <div className="hkm-footer">
            <button type="button" className="hkm-btn" onClick={onKapat} disabled={kaydediliyor}>
              Vazgeç
            </button>
            <button type="submit" className="hkm-btn hkm-btn-birincil" disabled={kaydediliyor || !odenebilir}>
              {kaydediliyor ? 'Kaydediliyor...' : 'Ödemeyi Kaydet'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
