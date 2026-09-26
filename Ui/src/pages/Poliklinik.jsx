import { useCallback, useEffect, useState } from 'react';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { muayeneService } from '../services/muayeneService';
import { hastaService } from '../services/hastaService';
import { getApiErrorMessage } from '../utils/apiError';
import { muayeneDurumEtiket } from '../utils/muayeneSecenekleri';
import { useDoktorVeServisListesi } from '../components/hastaKayit/useDoktorVeServisListesi';
import MuayeneAcForm from '../components/muayene/MuayeneAcForm';
import TeshisBolumu from '../components/muayene/TeshisBolumu';
import TedaviBolumu from '../components/muayene/TedaviBolumu';
import OdemeBolumu from '../components/muayene/OdemeBolumu';
import TaahutnameBolumu from '../components/muayene/TaahutnameBolumu';
import './Poliklinik.css';
import Bildirim from '../components/bildirim/Bildirim';

function yerelTarih(d) {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function bugun() {
  return yerelTarih(new Date());
}

function haftaninPazartesisi(d) {
  const kopya = new Date(d);
  const gun = kopya.getDay(); // 0 = Pazar, 1 = Pazartesi, ...
  const fark = gun === 0 ? -6 : 1 - gun;
  kopya.setDate(kopya.getDate() + fark);
  return kopya;
}

function tarihFormat(deger) {
  if (!deger) return '-';
  return new Date(deger).toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function saatFormat(deger) {
  if (!deger) return '-';
  return String(deger).slice(0, 5);
}

function randevuTarihSaat(deger) {
  return `${tarihFormat(deger)} ${new Date(deger).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })}`;
}

// ── Girişte: poliklinik + tarih aralığı seç, o aralığa ait muayene kayıtlarını (ve
// muayenesi henüz açılmamış randevuları) listele. Seçimler (servisNo/tarihler) üst
// bileşende tutuluyor ki hasta detayına girip listeye dönünce sıfırlanmasınlar. ──
function PoliklinikHastaListesi({
  onHastaSecildi,
  servisNo,
  setServisNo,
  baslangicTarih,
  setBaslangicTarih,
  bitisTarih,
  setBitisTarih,
}) {
  const { servisler, yukleniyor: servisYukleniyor, hata: servisHata } = useDoktorVeServisListesi();
  const [hastalar, setHastalar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');

  const aralikGecersiz = baslangicTarih > bitisTarih;
  const secilenServisAdi = servisler.find((s) => String(s.servisNo) === servisNo)?.servisAdi;

  const hastalariYukle = useCallback(async () => {
    if (!servisNo || aralikGecersiz) return;
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await muayeneService.poliklinikHastaListesiGetir(
        servisNo,
        `${baslangicTarih}T00:00:00`,
        `${bitisTarih}T23:59:59`
      );
      setHastalar(data?.data || []);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Hasta listesi alınamadı'));
    } finally {
      setYukleniyor(false);
    }
  }, [servisNo, baslangicTarih, bitisTarih, aralikGecersiz]);

  useEffect(() => {
    if (!servisNo) {
      setHastalar([]);
      return;
    }
    hastalariYukle();
  }, [servisNo, baslangicTarih, bitisTarih, hastalariYukle]);

  const bugunSec = () => {
    setBaslangicTarih(bugun());
    setBitisTarih(bugun());
  };

  const buHaftaSec = () => {
    const pazartesi = haftaninPazartesisi(new Date());
    const pazar = new Date(pazartesi);
    pazar.setDate(pazar.getDate() + 6);
    setBaslangicTarih(yerelTarih(pazartesi));
    setBitisTarih(yerelTarih(pazar));
  };

  return (
    <div className="mk-panel">
      <div className="mk-pol-secim">
        <label className="mk-alan">
          <span>Poliklinik</span>
          <select value={servisNo} onChange={(e) => setServisNo(e.target.value)} disabled={servisYukleniyor}>
            <option value="">{servisYukleniyor ? 'Yükleniyor...' : 'Seçiniz'}</option>
            {servisler.map((s) => (
              <option key={s.servisNo} value={s.servisNo}>{s.servisAdi}</option>
            ))}
          </select>
        </label>

        <label className="mk-alan">
          <span>Başlangıç Tarihi</span>
          <input type="date" value={baslangicTarih} onChange={(e) => setBaslangicTarih(e.target.value)} />
        </label>

        <label className="mk-alan">
          <span>Bitiş Tarihi</span>
          <input type="date" value={bitisTarih} onChange={(e) => setBitisTarih(e.target.value)} />
        </label>

        <div className="mk-hizli-tarih">
          <button type="button" className="mk-btn" onClick={bugunSec}>Bugün</button>
          <button type="button" className="mk-btn" onClick={buHaftaSec}>Bu Hafta</button>
        </div>
      </div>

      <Bildirim mesaj={servisHata} />
      {aralikGecersiz && <Bildirim mesaj="Başlangıç tarihi bitiş tarihinden büyük olamaz." tip="uyari" />}

      {!servisNo && <p className="mk-bos-metin">Hasta listesini görmek için bir poliklinik seç.</p>}
      {servisNo && yukleniyor && <p className="mk-bos-metin">Yükleniyor...</p>}
      {servisNo && !yukleniyor && <Bildirim mesaj={hata} />}
      {servisNo && !yukleniyor && !hata && !aralikGecersiz && hastalar.length === 0 && (
        <p className="mk-bos-metin">
          {tarihFormat(baslangicTarih)} – {tarihFormat(bitisTarih)} aralığında bu poliklinikte hasta bulunmuyor.
        </p>
      )}

      {hastalar.length > 0 && (
        <ul className="mk-liste">
          {hastalar.map((k) => (
            <li
              key={k.muayeneId ?? `r-${k.dosyaId}`}
              className="mk-liste-item mk-liste-item-tiklanabilir"
              onClick={() => onHastaSecildi({ ...k, poliklinikAd: secilenServisAdi })}
            >
              <div>
                <strong>{k.ad} {k.soyad}</strong> · Protokol {k.protokol} · {k.doktor}
                {!k.muayeneVarMi && <span className="mk-durum-rozet mk-durum-randevu">Randevu</span>}
              </div>
              <span className="mk-kayit-tarih">{randevuTarihSaat(k.tarih)}</span>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default function Poliklinik() {
  const [searchParams] = useSearchParams();
  const location = useLocation();
  const navigate = useNavigate();
  const randevuId = searchParams.get('randevuId');
  const muayeneIdParam = searchParams.get('muayeneId');
  const girisVarMi = !!(randevuId || muayeneIdParam);
  const baglamBilgisi = location.state || null;

  // Hasta listesi seçimleri (poliklinik + tarih aralığı) — hasta detayına girip
  // listeye dönüldüğünde sıfırlanmasın diye burada, üst bileşende tutuluyor.
  const [servisNo, setServisNo] = useState('');
  const [baslangicTarih, setBaslangicTarih] = useState(bugun());
  const [bitisTarih, setBitisTarih] = useState(bugun());

  const [muayene, setMuayene] = useState(null);
  const [hastaAdSoyad, setHastaAdSoyad] = useState('');
  const [randevuBulunamadi, setRandevuBulunamadi] = useState(false);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');
  const [durumDegistiriliyor, setDurumDegistiriliyor] = useState(false);
  const [yenidenYukleTetik, setYenidenYukleTetik] = useState(0);
  const [aktifSekme, setAktifSekme] = useState('muayene');

  const detayiYukle = useCallback(async () => {
    if (!randevuId && !muayeneIdParam) return;
    setYukleniyor(true);
    setHata('');
    setRandevuBulunamadi(false);
    setMuayene(null);
    try {
      const { data } = muayeneIdParam
        ? await muayeneService.muayeneKaydiGetir(muayeneIdParam)
        : await muayeneService.muayeneRandevuIleGetir(randevuId);
      setMuayene(data.data);
    } catch (err) {
      if (!muayeneIdParam && err?.response?.status === 404) {
        setRandevuBulunamadi(true);
      } else {
        setHata(getApiErrorMessage(err, 'Muayene bilgisi alınamadı'));
      }
    } finally {
      setYukleniyor(false);
    }
  }, [randevuId, muayeneIdParam]);

  useEffect(() => {
    detayiYukle();
  }, [detayiYukle]);

  // Yeni açılan muayenenin RandevuId'si, backend tarafında sadece doktor+tarih+saat
  // orijinal randevuyla birebir eşleşirse bağlanıyor. Bu yüzden oluşturduktan sonra
  // randevuId ile değil, dönen id ile doğrudan getiriyoruz — eşleşmese bile çalışsın diye.
  const muayeneIdIleYukle = async (id) => {
    setYukleniyor(true);
    setHata('');
    setRandevuBulunamadi(false);
    try {
      const { data } = await muayeneService.muayeneKaydiGetir(id);
      setMuayene(data.data);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Muayene bilgisi alınamadı'));
    } finally {
      setYukleniyor(false);
    }
  };

  useEffect(() => {
    if (baglamBilgisi?.ad) {
      setHastaAdSoyad(`${baglamBilgisi.ad} ${baglamBilgisi.soyad}`);
      return;
    }
    if (!muayene) return;
    hastaService
      .ara(String(muayene.protocolNo))
      .then(({ data }) => {
        const bulunan = data?.[0];
        if (bulunan) setHastaAdSoyad(`${bulunan.name} ${bulunan.surname}`);
      })
      .catch(() => {});
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [muayene]);

  const durumDegistir = async () => {
    setDurumDegistiriliyor(true);
    setHata('');
    try {
      await muayeneService.muayeneKapat(muayene.id);
      await detayiYukle();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Durum güncellenemedi'));
    } finally {
      setDurumDegistiriliyor(false);
    }
  };

  const hastaSecildi = (kayit) => {
    const state = {
      ad: kayit.ad,
      soyad: kayit.soyad,
      tc: kayit.tc,
      protokol: kayit.protokol,
      doktorAd: kayit.doktor,
      poliklinikAd: kayit.poliklinikAd,
      randevuTarihi: kayit.tarih,
    };
    // Muayene kaydı zaten açılmışsa doğrudan ona gidiyoruz; sadece randevusu
    // varsa (henüz muayene açılmamış) randevu bağlamıyla "aç" akışına giriyoruz.
    if (kayit.muayeneVarMi && kayit.muayeneId) {
      navigate(`/poliklinik?muayeneId=${kayit.muayeneId}`, { state });
    } else {
      navigate(`/poliklinik?randevuId=${kayit.dosyaId}`, { state });
    }
  };

  const listeyeDon = () => {
    setAktifSekme('muayene');
    navigate('/poliklinik');
  };

  // ── Randevu/muayene bağlamı olmadan sayfa açıldıysa: poliklinik + tarih bazlı hasta listesi ──
  if (!girisVarMi) {
    return (
      <div className="sayfa">
        <div className="sayfa-baslik">
          <h1>Poliklinik</h1>
        </div>
        <PoliklinikHastaListesi
          onHastaSecildi={hastaSecildi}
          servisNo={servisNo}
          setServisNo={setServisNo}
          baslangicTarih={baslangicTarih}
          setBaslangicTarih={setBaslangicTarih}
          bitisTarih={bitisTarih}
          setBitisTarih={setBitisTarih}
        />
      </div>
    );
  }

  return (
    <div className="sayfa">
      <div className="sayfa-baslik">
        <h1>Poliklinik</h1>
      </div>

      <button type="button" className="mk-btn mk-geri-btn" onClick={listeyeDon}>‹ Hasta Listesine Dön</button>

      {yukleniyor && <p className="mk-bos-metin">Yükleniyor...</p>}
      {!yukleniyor && <Bildirim mesaj={hata} />}

      {!yukleniyor && randevuBulunamadi && (
        baglamBilgisi?.tc && baglamBilgisi?.protokol ? (
          <MuayeneAcForm
            tc={baglamBilgisi.tc}
            protokol={baglamBilgisi.protokol}
            adSoyad={`${baglamBilgisi.ad} ${baglamBilgisi.soyad}`}
            doktorAdi={baglamBilgisi.doktorAd}
            poliklinikAdi={baglamBilgisi.poliklinikAd}
            randevuTarihi={baglamBilgisi.randevuTarihi}
            onOlusturuldu={muayeneIdIleYukle}
          />
        ) : (
          <div className="mk-panel">
            <p className="mk-bos-metin">
              Bu randevu için henüz muayene kaydı açılmamış. Muayene açmak için hasta listesinden ya da
              Hasta Kayıt ekranındaki Poliklinik Kayıtları listesinden ilgili kayda tıkla.
            </p>
          </div>
        )
      )}

      {!yukleniyor && muayene && (
        <>
          <div className="mk-panel mk-header">
            <div className="mk-header-bilgi">
              <h2>{hastaAdSoyad || 'Hasta'}</h2>
              <p>
                Protokol {muayene.protocolNo} · {muayene.doktorAd} · {muayene.polAdi}
              </p>
              <p className="mk-alt-baslik">
                {tarihFormat(muayene.muayeneTarihi)} {saatFormat(muayene.baslangicSaati)}
              </p>
            </div>
            <div className="mk-header-durum">
              <span className={`mk-durum-rozet ${muayene.bitisSaati ? 'mk-durum-kapali' : 'mk-durum-acik'}`}>
                {muayeneDurumEtiket(muayene.bitisSaati)}
              </span>
              <button type="button" className="mk-btn" onClick={durumDegistir} disabled={durumDegistiriliyor}>
                {durumDegistiriliyor ? '...' : muayene.bitisSaati ? 'Muayeneyi Yeniden Aç' : 'Muayeneyi Kapat'}
              </button>
            </div>
          </div>

          <div className="mk-tab-bar">
            <button
              type="button"
              className={`mk-tab-btn ${aktifSekme === 'muayene' ? 'mk-tab-aktif' : ''}`}
              onClick={() => setAktifSekme('muayene')}
            >
              Muayene
            </button>
            <button
              type="button"
              className={`mk-tab-btn ${aktifSekme === 'taahutname' ? 'mk-tab-aktif' : ''}`}
              onClick={() => setAktifSekme('taahutname')}
            >
              Taahütname
            </button>
          </div>

          {aktifSekme === 'muayene' && (
            <div className="mk-grid">
              <TeshisBolumu muayeneId={muayene.id} kapali={!!muayene.bitisSaati} />
              <TedaviBolumu
                muayeneId={muayene.id}
                kapali={!!muayene.bitisSaati}
                onDegisti={() => setYenidenYukleTetik((t) => t + 1)}
              />
              <OdemeBolumu
                muayeneId={muayene.id}
                yenidenYukleTetik={yenidenYukleTetik}
                onDegisti={() => setYenidenYukleTetik((t) => t + 1)}
              />
            </div>
          )}

          {aktifSekme === 'taahutname' && (
            <TaahutnameBolumu
              muayeneId={muayene.id}
              protokol={muayene.protocolNo}
              yenidenYukleTetik={yenidenYukleTetik}
              onDegisti={() => setYenidenYukleTetik((t) => t + 1)}
            />
          )}
        </>
      )}
    </div>
  );
}
