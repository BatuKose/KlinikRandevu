import { useCallback, useEffect, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import TaahutnameModal from './TaahutnameModal';

function fiyatFormat(deger) {
  return (deger ?? 0).toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' });
}

function tarihFormat(deger) {
  if (!deger) return '-';
  return new Date(deger).toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function bugununBasi() {
  const d = new Date();
  d.setHours(0, 0, 0, 0);
  return d;
}

function durumBilgisi(t) {
  if (t.iptal) return { etiket: 'İptal Edildi', sinif: 'mk-durum-kapali' };
  if (t.odendi) return { etiket: 'Ödendi', sinif: 'mk-durum-odendi' };
  if (new Date(t.sonOdemeTarihi) < bugununBasi()) return { etiket: 'Vadesi Geçti', sinif: 'mk-durum-iptal' };
  return { etiket: 'Aktif', sinif: 'mk-durum-bekliyor' };
}

export default function TaahutnameBolumu({ muayeneId, protokol, yenidenYukleTetik, onDegisti }) {
  const [taahutnameler, setTaahutnameler] = useState([]);
  const [borc, setBorc] = useState(0);
  const [yukleniyor, setYukleniyor] = useState(true);
  const [hata, setHata] = useState('');
  const [modal, setModal] = useState(null); // { kayit } — kayit null ise yeni ekleme
  const [iptalEdilenId, setIptalEdilenId] = useState(null);

  const veriyiYukle = useCallback(async () => {
    setYukleniyor(true);
    setHata('');
    try {
      const [listeRes, borcRes] = await Promise.all([
        muayeneService.hastaninTaahutnameleriniGetir(protokol),
        muayeneService.muayeneBorcGetir(muayeneId),
      ]);
      setTaahutnameler(listeRes.data?.data || []);
      setBorc(borcRes.data?.data || 0);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Taahütname bilgileri alınamadı'));
    } finally {
      setYukleniyor(false);
    }
  }, [protokol, muayeneId]);

  useEffect(() => {
    veriyiYukle();
  }, [veriyiYukle, yenidenYukleTetik]);

  const buMuayenedeAktifVar = taahutnameler.some((t) => t.muayeneId === Number(muayeneId) && !t.iptal);
  const yeniEklenemez = borc <= 0 || buMuayenedeAktifVar;

  const iptalEt = async (t) => {
    if (!window.confirm('Bu taahütnameyi iptal etmek istediğine emin misin?')) return;
    setIptalEdilenId(t.id);
    setHata('');
    try {
      await muayeneService.taahutnameIptalEt(t.id);
      await veriyiYukle();
      onDegisti?.();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Taahütname iptal edilemedi'));
    } finally {
      setIptalEdilenId(null);
    }
  };

  return (
    <div className="mk-panel mk-bolum">
      <div className="mk-tah-baslik">
        <h2>Taahütnameler</h2>
        <button
          type="button"
          className="mk-btn mk-btn-birincil"
          onClick={() => setModal({ kayit: null })}
          disabled={yukleniyor || yeniEklenemez}
          title={
            buMuayenedeAktifVar
              ? 'Bu muayenenin aktif taahütnamesi var; yenisi için önce iptal et'
              : borc <= 0
                ? 'Bu muayenede borç bulunmuyor'
                : undefined
          }
        >
          + Yeni Taahütname
        </button>
      </div>

      {yukleniyor && <p className="mk-bos-metin">Yükleniyor...</p>}
      {!yukleniyor && hata && <div className="mk-hata">{hata}</div>}

      {!yukleniyor && !hata && (
        <>
          <p className="mk-bos-metin">
            Bu muayenenin kalan borcu: <strong>{fiyatFormat(borc)}</strong>
            {buMuayenedeAktifVar && ' · Bu muayenenin aktif bir taahütnamesi var.'}
          </p>

          {taahutnameler.length === 0 ? (
            <p className="mk-bos-metin">Hastaya ait taahütname kaydı bulunmuyor.</p>
          ) : (
            <ul className="mk-liste">
              {taahutnameler.map((t) => {
                const durum = durumBilgisi(t);
                const islemYapilabilir = !t.iptal && !t.odendi;
                return (
                  <li
                    key={t.id}
                    className={`mk-liste-item mk-tah-item ${t.muayeneId === Number(muayeneId) ? 'mk-tah-item-guncel' : ''}`}
                  >
                    <div className="mk-tah-bilgi">
                      <div className="mk-tah-satir1">
                        <strong>Muayene #{t.muayeneId}</strong>
                        <span>{t.polAdi} · {tarihFormat(t.muayeneTarihi)}</span>
                        <span className={`mk-durum-rozet ${durum.sinif}`}>{durum.etiket}</span>
                        {t.muayeneId === Number(muayeneId) && <span className="mk-tah-guncel-etiket">Bu muayene</span>}
                      </div>
                      <div className="mk-tah-satir2">
                        Borç: <strong>{fiyatFormat(t.toplamBorc)}</strong>
                        {' · '}Taahüt: {tarihFormat(t.tahütTarihi)}
                        {' · '}Son ödeme: {tarihFormat(t.sonOdemeTarihi)}
                        {(t.bilgilendirmeSms || t.bilgilendirmeMail) && (
                          <> · Hatırlatma: {[t.bilgilendirmeSms && 'SMS', t.bilgilendirmeMail && 'E-posta'].filter(Boolean).join(' + ')} gönderildi</>
                        )}
                      </div>
                    </div>
                    {islemYapilabilir && (
                      <div className="mk-liste-item-sag">
                        <button type="button" className="mk-btn mk-tah-btn" onClick={() => setModal({ kayit: t })}>
                          Düzenle
                        </button>
                        <button
                          type="button"
                          className="mk-mini-btn"
                          onClick={() => iptalEt(t)}
                          disabled={iptalEdilenId === t.id}
                        >
                          {iptalEdilenId === t.id ? '...' : 'İptal Et'}
                        </button>
                      </div>
                    )}
                  </li>
                );
              })}
            </ul>
          )}
        </>
      )}

      {modal && (
        <TaahutnameModal
          kayit={modal.kayit}
          muayeneId={muayeneId}
          borc={borc}
          onKapat={() => setModal(null)}
          onBasarili={async () => {
            setModal(null);
            await veriyiYukle();
            onDegisti?.();
          }}
        />
      )}
    </div>
  );
}
