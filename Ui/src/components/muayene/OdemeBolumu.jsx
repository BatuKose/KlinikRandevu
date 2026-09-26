import { useEffect, useState, useCallback } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { ODEME_TIPI } from '../../utils/muayeneSecenekleri';
import OdemeYapModal from './OdemeYapModal';
import Bildirim from '../bildirim/Bildirim';

function fiyatFormat(deger) {
  return (deger ?? 0).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' });
}

function tarihFormat(deger) {
  return new Date(deger).toLocaleString('tr-TR', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
  });
}

export default function OdemeBolumu({ muayeneId, yenidenYukleTetik, onDegisti }) {
  const [odemeler, setOdemeler] = useState([]);
  const [borc, setBorc] = useState(0);
  const [odenenToplam, setOdenenToplam] = useState(0);
  const [odenmemisTedaviler, setOdenmemisTedaviler] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(true);
  const [hata, setHata] = useState('');
  const [modalAcik, setModalAcik] = useState(false);
  const [iptalEdiliyor, setIptalEdiliyor] = useState(false);

  const veriyiYukle = useCallback(async () => {
    setYukleniyor(true);
    setHata('');
    try {
      const [odemeRes, borcRes, odenenRes, tedaviRes] = await Promise.all([
        muayeneService.odemeleriGetir(muayeneId),
        muayeneService.muayeneBorcGetir(muayeneId),
        muayeneService.muayeneOdemeToplamGetir(muayeneId),
        muayeneService.tedavileriGetir(muayeneId),
      ]);
      setOdemeler(odemeRes.data?.data || []);
      setBorc(borcRes.data?.data || 0);
      setOdenenToplam(odenenRes.data?.data || 0);
      setOdenmemisTedaviler((tedaviRes.data?.data || []).filter((t) => !t.odendi));
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Ödeme bilgileri alınamadı'));
    } finally {
      setYukleniyor(false);
    }
  }, [muayeneId]);

  useEffect(() => {
    veriyiYukle();
  }, [veriyiYukle, yenidenYukleTetik]);

  const tumOdemeleriIptalEt = async () => {
    if (!window.confirm('Bu muayenedeki tüm ödemeleri iptal etmek istediğine emin misin?')) return;
    setIptalEdiliyor(true);
    setHata('');
    try {
      await muayeneService.odemeIptalEt({
        tedaviId: null,
        muyaneId: muayeneId,
        odemeIptalTipi: ODEME_TIPI.TOPLAM_MUAYENE,
      });
      await veriyiYukle();
      onDegisti?.();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Ödemeler iptal edilemedi'));
    } finally {
      setIptalEdiliyor(false);
    }
  };

  return (
    <div className="mk-panel mk-bolum">
      <h2>Ödemeler</h2>

      {yukleniyor && <p className="mk-bos-metin">Yükleniyor...</p>}
      {!yukleniyor && <Bildirim mesaj={hata} />}

      {!yukleniyor && !hata && (
        <>
          <div className="mk-ozet-grid">
            <div className="mk-ozet-kart">
              <span>Kalan Borç</span>
              <strong className={borc > 0 ? 'mk-borc-var' : ''}>{fiyatFormat(borc)}</strong>
            </div>
            <div className="mk-ozet-kart">
              <span>Toplam Ödenen</span>
              <strong>{fiyatFormat(odenenToplam)}</strong>
            </div>
          </div>

          {odemeler.length === 0 ? (
            <p className="mk-bos-metin">Henüz ödeme alınmamış.</p>
          ) : (
            <ul className="mk-liste">
              {odemeler.map((o) => (
                <li key={o.id} className="mk-liste-item">
                  <span>{tarihFormat(o.odemeTarihi)}</span>
                  <span className="mk-fiyat">{fiyatFormat(o.odemeToplam)}</span>
                </li>
              ))}
            </ul>
          )}

          <div className="mk-form-aksiyonlar">
            <button type="button" className="mk-btn mk-btn-birincil" onClick={() => setModalAcik(true)}>
              Ödeme Yap
            </button>
            {odenenToplam > 0 && (
              <button type="button" className="mk-btn" onClick={tumOdemeleriIptalEt} disabled={iptalEdiliyor}>
                {iptalEdiliyor ? '...' : 'Tüm Ödemeleri İptal Et'}
              </button>
            )}
          </div>
        </>
      )}

      {modalAcik && (
        <OdemeYapModal
          muayeneId={muayeneId}
          borc={borc}
          odenmemisTedaviler={odenmemisTedaviler}
          onKapat={() => setModalAcik(false)}
          onBasarili={async () => {
            setModalAcik(false);
            await veriyiYukle();
            onDegisti?.();
          }}
        />
      )}
    </div>
  );
}
