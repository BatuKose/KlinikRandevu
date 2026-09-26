import { useCallback, useEffect, useMemo, useState } from 'react';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { muayeneService } from '../services/muayeneService';
import { hastaService } from '../services/hastaService';
import { getApiErrorMessage } from '../utils/apiError';
import { muayeneDurumEtiket } from '../utils/muayeneSecenekleri';
import {
  yerelTarih,
  bugun,
  haftaninPazartesisi,
  gunEkle,
  tarihFormat,
  saatFormat,
  uzunTarihFormat,
} from '../utils/tarih';
import { useDoktorVeServisListesi } from '../components/hastaKayit/useDoktorVeServisListesi';
import MuayeneAcForm from '../components/muayene/MuayeneAcForm';
import TeshisBolumu from '../components/muayene/TeshisBolumu';
import TedaviBolumu from '../components/muayene/TedaviBolumu';
import OdemeBolumu from '../components/muayene/OdemeBolumu';
import TaahutnameBolumu from '../components/muayene/TaahutnameBolumu';
import Bildirim from '../components/bildirim/Bildirim';
import Ikon from '../components/ui/Ikon';
import { SayfaBaslik, BosDurum, IskeletListe, IstatistikKart, Avatar } from '../components/ui/Ortak';
import './Poliklinik.css';

const SON_POLIKLINIK_ANAHTARI = 'poliklinik.sonSecilen';

// Son seçilen poliklinik sadece bu tarayıcıda hatırlanır; depolama kapalıysa sessizce boş döner.
function sonPoliklinigiOku() {
  try {
    return localStorage.getItem(SON_POLIKLINIK_ANAHTARI) || '';
  } catch {
    return '';
  }
}

function sonPoliklinigiYaz(deger) {
  try {
    if (deger) localStorage.setItem(SON_POLIKLINIK_ANAHTARI, deger);
    else localStorage.removeItem(SON_POLIKLINIK_ANAHTARI);
  } catch {
    // depolama erişilemiyorsa hatırlamadan devam
  }
}

// Muayene saati "HH:mm:ss" TimeSpan olarak geliyor.
function timeSpanSaat(deger) {
  return deger ? String(deger).slice(0, 5) : '-';
}

const DURUM_SEKMELERI = [
  { anahtar: 'tumu', etiket: 'Tümü' },
  { anahtar: 'bekleyen', etiket: 'Muayene Bekleyen' },
  { anahtar: 'acilan', etiket: 'Muayenesi Açılan' },
];

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
  const [aramaMetni, setAramaMetni] = useState('');
  const [durumFiltre, setDurumFiltre] = useState('tumu');

  const aralikGecersiz = baslangicTarih > bitisTarih;
  const tekGun = baslangicTarih === bitisTarih;
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
      setHastalar([]);
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
  }, [servisNo, hastalariYukle]);

  const sayilar = useMemo(() => {
    const acilan = hastalar.filter((h) => h.muayeneVarMi).length;
    return { tumu: hastalar.length, acilan, bekleyen: hastalar.length - acilan };
  }, [hastalar]);

  const gosterilecek = useMemo(() => {
    const metin = aramaMetni.trim().toLocaleLowerCase('tr-TR');
    return hastalar.filter((h) => {
      if (durumFiltre === 'bekleyen' && h.muayeneVarMi) return false;
      if (durumFiltre === 'acilan' && !h.muayeneVarMi) return false;
      if (!metin) return true;
      return `${h.ad} ${h.soyad} ${h.protokol} ${h.tc}`.toLocaleLowerCase('tr-TR').includes(metin);
    });
  }, [hastalar, aramaMetni, durumFiltre]);

  const aktifHizli =
    tekGun && baslangicTarih === bugun()
      ? 'bugun'
      : baslangicTarih === yerelTarih(haftaninPazartesisi(new Date())) &&
          bitisTarih === yerelTarih(gunEkle(haftaninPazartesisi(new Date()), 6))
        ? 'hafta'
        : '';

  const bugunSec = () => {
    setBaslangicTarih(bugun());
    setBitisTarih(bugun());
  };

  const buHaftaSec = () => {
    const pazartesi = haftaninPazartesisi(new Date());
    setBaslangicTarih(yerelTarih(pazartesi));
    setBitisTarih(yerelTarih(gunEkle(pazartesi, 6)));
  };

  return (
    <>
      <Bildirim mesaj={servisHata} />
      <Bildirim mesaj={hata} />
      {aralikGecersiz && <Bildirim mesaj="Başlangıç tarihi bitiş tarihinden büyük olamaz." tip="uyari" />}

      <section className="ui-kart pk-arac">
        <div className="ui-arac-cubugu">
          <label className="ui-alan pk-pol-alan">
            <span>Poliklinik</span>
            <select
              className="ui-input"
              value={servisNo}
              onChange={(e) => setServisNo(e.target.value)}
              disabled={servisYukleniyor}
            >
              <option value="">{servisYukleniyor ? 'Yükleniyor...' : 'Poliklinik seçin'}</option>
              {servisler.map((s) => (
                <option key={s.servisNo} value={s.servisNo}>{s.servisAdi}</option>
              ))}
            </select>
          </label>
          <label className="ui-alan pk-tarih-alan">
            <span>Başlangıç</span>
            <input className="ui-input" type="date" value={baslangicTarih} onChange={(e) => setBaslangicTarih(e.target.value)} />
          </label>
          <label className="ui-alan pk-tarih-alan">
            <span>Bitiş</span>
            <input className="ui-input" type="date" value={bitisTarih} onChange={(e) => setBitisTarih(e.target.value)} />
          </label>
          <div className="ui-segment pk-hizli">
            <button type="button" className={aktifHizli === 'bugun' ? 'ui-segment-aktif' : ''} onClick={bugunSec}>
              Bugün
            </button>
            <button type="button" className={aktifHizli === 'hafta' ? 'ui-segment-aktif' : ''} onClick={buHaftaSec}>
              Bu Hafta
            </button>
          </div>
          <div className="ui-arac-bosluk" />
          <button
            type="button"
            className="ui-btn ui-btn-ikon"
            onClick={hastalariYukle}
            disabled={!servisNo || yukleniyor}
            title="Listeyi yenile"
            aria-label="Listeyi yenile"
          >
            {yukleniyor ? <span className="ui-spinner" /> : <Ikon ad="yenile" />}
          </button>
        </div>
      </section>

      {!servisNo && (
        <section className="ui-kart">
          <BosDurum
            ikon="stetoskop"
            baslik="Poliklinik seçin"
            aciklama="Hasta listesini görmek için yukarıdan çalıştığınız polikliniği seçin. Seçiminiz bu tarayıcıda hatırlanır."
          />
        </section>
      )}

      {servisNo && (
        <>
          <div className="ui-istatistik-grid">
            <IstatistikKart ikon="kullanicilar" deger={sayilar.tumu} etiket="Toplam hasta" />
            <IstatistikKart ikon="bekleme" ton="uyari" deger={sayilar.bekleyen} etiket="Muayene bekleyen" />
            <IstatistikKart ikon="aktivite" ton="basari" deger={sayilar.acilan} etiket="Muayenesi açılan" />
          </div>

          <section className="ui-kart pk-liste">
            <div className="ui-kart-baslik pk-liste-baslik">
              <div className="ui-segment">
                {DURUM_SEKMELERI.map((d) => (
                  <button
                    key={d.anahtar}
                    type="button"
                    className={durumFiltre === d.anahtar ? 'ui-segment-aktif' : ''}
                    onClick={() => setDurumFiltre(d.anahtar)}
                  >
                    {d.etiket} <span className="ui-sayac">{sayilar[d.anahtar]}</span>
                  </button>
                ))}
              </div>
              <div className="pk-liste-ara">
                <Ikon ad="ara" />
                <input
                  type="search"
                  placeholder="Listede hasta ara"
                  value={aramaMetni}
                  onChange={(e) => setAramaMetni(e.target.value)}
                />
              </div>
            </div>

            {yukleniyor && <IskeletListe satir={5} />}

            {!yukleniyor && !aralikGecersiz && gosterilecek.length === 0 && (
              <BosDurum
                ikon="kullanicilar"
                baslik={hastalar.length === 0 ? 'Bu aralıkta hasta yok' : 'Eşleşen hasta yok'}
                aciklama={
                  hastalar.length === 0
                    ? `${secilenServisAdi || 'Seçili poliklinik'} için ${tarihFormat(baslangicTarih)}${tekGun ? '' : ` – ${tarihFormat(bitisTarih)}`} tarihinde randevu ya da muayene kaydı bulunmuyor.`
                    : 'Arama veya durum filtresini değiştirmeyi deneyin.'
                }
              />
            )}

            {!yukleniyor && gosterilecek.length > 0 && (
              <div className="ui-tablo-kapsayici">
                <table className="ui-tablo">
                  <thead>
                    <tr>
                      <th style={{ width: 56 }}>Sıra</th>
                      <th style={{ width: tekGun ? 80 : 150 }}>{tekGun ? 'Saat' : 'Tarih / Saat'}</th>
                      <th>Hasta</th>
                      <th>Doktor</th>
                      <th>Durum</th>
                      <th className="ui-tablo-sag" />
                    </tr>
                  </thead>
                  <tbody>
                    {gosterilecek.map((k, i) => (
                      <tr
                        key={k.muayeneId ?? `r-${k.dosyaId}`}
                        className="ui-tablo-tiklanabilir"
                        onClick={() => onHastaSecildi({ ...k, poliklinikAd: secilenServisAdi })}
                      >
                        <td><span className="pk-sira ui-sayi">{i + 1}</span></td>
                        <td>
                          <span className="pk-saat ui-sayi">{saatFormat(k.tarih)}</span>
                          {!tekGun && <div className="ui-soluk ui-sayi pk-alt">{tarihFormat(k.tarih)}</div>}
                        </td>
                        <td>
                          <div className="pk-hasta">
                            <Avatar ad={k.ad} soyad={k.soyad} />
                            <div>
                              <div className="pk-hasta-ad">{k.ad} {k.soyad}</div>
                              <div className="ui-soluk ui-sayi pk-alt">#{k.protokol} · TC {k.tc}</div>
                            </div>
                          </div>
                        </td>
                        <td>{k.doktor || '-'}</td>
                        <td>
                          {k.muayeneVarMi ? (
                            <span className="ui-rozet ui-rozet-nokta ui-rozet-basari">Muayene açıldı</span>
                          ) : (
                            <span className="ui-rozet ui-rozet-nokta ui-rozet-uyari">Bekliyor</span>
                          )}
                        </td>
                        <td className="ui-tablo-sag">
                          <span className={`ui-btn ui-btn-kucuk${k.muayeneVarMi ? '' : ' ui-btn-birincil'}`}>
                            {k.muayeneVarMi ? 'Dosyayı Aç' : 'Muayeneye Al'}
                            <Ikon ad="sagOk" />
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </section>
        </>
      )}
    </>
  );
}

// ── Muayene detayındaki hasta/karşılaşma özeti ──
function MuayeneBanner({ muayene, hastaAdSoyad, durumDegistiriliyor, onDurumDegistir }) {
  const [ad = '', ...soyadParcalari] = (hastaAdSoyad || '').split(' ');
  const kapali = !!muayene.bitisSaati;
  return (
    <section className={`ui-kart pk-banner${kapali ? ' pk-banner-kapali' : ''}`}>
      <Avatar ad={ad} soyad={soyadParcalari.join(' ')} boyut="buyuk" />
      <div className="pk-banner-bilgi">
        <div className="pk-banner-ust">
          <h2>{hastaAdSoyad || 'Hasta'}</h2>
          <span className={`ui-rozet ui-rozet-nokta ${kapali ? '' : 'ui-rozet-basari pk-canli'}`}>
            Muayene {muayeneDurumEtiket(muayene.bitisSaati).toLocaleLowerCase('tr-TR')}
          </span>
        </div>
        <dl className="pk-banner-meta">
          <div>
            <dt>Protokol</dt>
            <dd className="ui-sayi">#{muayene.protocolNo}</dd>
          </div>
          <div>
            <dt>Poliklinik</dt>
            <dd>{muayene.polAdi || '-'}</dd>
          </div>
          <div>
            <dt>Doktor</dt>
            <dd>{muayene.doktorAd || '-'}</dd>
          </div>
          <div>
            <dt>Tarih</dt>
            <dd>{uzunTarihFormat(muayene.muayeneTarihi)}</dd>
          </div>
          <div>
            <dt>Saat</dt>
            <dd className="ui-sayi">
              {timeSpanSaat(muayene.baslangicSaati)}
              {kapali && ` – ${timeSpanSaat(muayene.bitisSaati)}`}
            </dd>
          </div>
        </dl>
      </div>
      <div className="pk-banner-aksiyon">
        <button
          type="button"
          className={`ui-btn${kapali ? '' : ' ui-btn-birincil'}`}
          onClick={onDurumDegistir}
          disabled={durumDegistiriliyor}
        >
          {durumDegistiriliyor ? <span className="ui-spinner" /> : <Ikon ad={kapali ? 'kilitAcik' : 'kilit'} />}
          {kapali ? 'Muayeneyi Yeniden Aç' : 'Muayeneyi Kapat'}
        </button>
      </div>
    </section>
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
  const [servisNo, setServisNoState] = useState(sonPoliklinigiOku);
  const [baslangicTarih, setBaslangicTarih] = useState(bugun());
  const [bitisTarih, setBitisTarih] = useState(bugun());

  const setServisNo = (deger) => {
    setServisNoState(deger);
    sonPoliklinigiYaz(deger);
  };

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
    if (!muayene.bitisSaati && !window.confirm('Muayeneyi kapatmak istediğine emin misin?')) return;
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
        <SayfaBaslik
          ikon="stetoskop"
          baslik="Poliklinik"
          aciklama="Günlük hasta listesi; muayene, teşhis, tedavi ve ödeme işlemleri."
        />
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

  const kapali = !!muayene?.bitisSaati;

  return (
    <div className="sayfa">
      <nav className="pk-kirinti" aria-label="Konum">
        <button type="button" onClick={listeyeDon}>
          <Ikon ad="solOk" /> Hasta Listesi
        </button>
        <span aria-hidden="true">/</span>
        <span className="pk-kirinti-aktif">{hastaAdSoyad || baglamBilgisi?.ad || 'Muayene'}</span>
      </nav>

      <Bildirim mesaj={hata} />

      {yukleniyor && (
        <section className="ui-kart">
          <IskeletListe satir={3} />
        </section>
      )}

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
          <section className="ui-kart">
            <BosDurum
              ikon="dosya"
              baslik="Muayene kaydı açılmamış"
              aciklama="Bu randevu için henüz muayene kaydı yok. Muayene açmak için hasta listesinden ya da Hasta Kayıt ekranındaki kayıtlardan ilgili hastaya tıklayın."
            >
              <button type="button" className="ui-btn" onClick={listeyeDon}>
                <Ikon ad="solOk" /> Hasta listesine dön
              </button>
            </BosDurum>
          </section>
        )
      )}

      {!yukleniyor && muayene && (
        <>
          <MuayeneBanner
            muayene={muayene}
            hastaAdSoyad={hastaAdSoyad}
            durumDegistiriliyor={durumDegistiriliyor}
            onDurumDegistir={durumDegistir}
          />

          {kapali && (
            <div className="pk-kapali-uyari" role="status">
              <Ikon ad="kilit" />
              Bu muayene kapatılmış. Teşhis ve tedavi eklemek için muayeneyi yeniden açın.
            </div>
          )}

          <nav className="ui-sekmeler" aria-label="Muayene bölümleri">
            <button
              type="button"
              className={`ui-sekme${aktifSekme === 'muayene' ? ' ui-sekme-aktif' : ''}`}
              onClick={() => setAktifSekme('muayene')}
            >
              <Ikon ad="stetoskop" /> Muayene
            </button>
            <button
              type="button"
              className={`ui-sekme${aktifSekme === 'taahutname' ? ' ui-sekme-aktif' : ''}`}
              onClick={() => setAktifSekme('taahutname')}
            >
              <Ikon ad="dosya" /> Taahütname
            </button>
          </nav>

          {aktifSekme === 'muayene' && (
            <div className="mk-grid">
              <div className="mk-grid-klinik">
                <TeshisBolumu muayeneId={muayene.id} kapali={kapali} />
                <TedaviBolumu
                  muayeneId={muayene.id}
                  kapali={kapali}
                  onDegisti={() => setYenidenYukleTetik((t) => t + 1)}
                />
              </div>
              <div className="mk-grid-mali">
                <OdemeBolumu
                  muayeneId={muayene.id}
                  yenidenYukleTetik={yenidenYukleTetik}
                  onDegisti={() => setYenidenYukleTetik((t) => t + 1)}
                />
              </div>
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
