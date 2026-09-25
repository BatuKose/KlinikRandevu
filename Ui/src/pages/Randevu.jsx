import { useCallback, useEffect, useMemo, useState } from 'react';
import { muayeneService } from '../services/muayeneService';
import { getApiErrorMessage } from '../utils/apiError';
import { useDoktorVeServisListesi } from '../components/hastaKayit/useDoktorVeServisListesi';
import YeniRandevuModal from '../components/randevu/YeniRandevuModal';
import './Randevu.css';

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
  return new Date(deger).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
}

function durumBilgisi(kayit) {
  if (kayit.iptal) return { etiket: 'İptal Edildi', sinif: 'rd-durum-iptal' };
  if (new Date(kayit.randevuTarihi) < new Date()) return { etiket: 'Geçti', sinif: 'rd-durum-gecti' };
  return { etiket: 'Planlandı', sinif: 'rd-durum-planlandi' };
}

// Backend, DTO'da doktor/poliklinik için sadece ad döndürüyor (id yok); bu yüzden
// filtreyi isim eşleşmesiyle uyguluyoruz.
function filtreleUygula(liste, filtre) {
  const simdi = new Date();
  return liste.filter((k) => {
    if (filtre.doktor && k.doktor !== filtre.doktor) return false;
    if (filtre.poliklinik && k.poliklinik !== filtre.poliklinik) return false;
    if (filtre.durum === 'aktif' && (k.iptal || new Date(k.randevuTarihi) < simdi)) return false;
    if (filtre.durum === 'iptal' && !k.iptal) return false;
    if (filtre.durum === 'gecmis' && (k.iptal || new Date(k.randevuTarihi) >= simdi)) return false;
    return true;
  });
}

function IptalButonu({ kayit, iptalEdilenId, onIptal }) {
  if (kayit.iptal || new Date(kayit.randevuTarihi) < new Date()) return null;
  return (
    <button
      type="button"
      className="rd-mini-btn"
      onClick={() => onIptal(kayit.dosyaId)}
      disabled={iptalEdilenId === kayit.dosyaId}
    >
      {iptalEdilenId === kayit.dosyaId ? 'İptal ediliyor...' : 'İptal Et'}
    </button>
  );
}

// ── Doktor / poliklinik / durum filtre çubuğu — hem liste hem takvim görünümünde kullanılıyor ──
function RandevuFiltreBar({ doktorlar, servisler, filtre, setFiltre }) {
  return (
    <div className="rd-filtre-bar">
      <label className="rd-alan">
        <span>Doktor</span>
        <select value={filtre.doktor} onChange={(e) => setFiltre((f) => ({ ...f, doktor: e.target.value }))}>
          <option value="">Tümü</option>
          {doktorlar.map((d) => (
            <option key={d.doktorNo} value={d.doktorAd}>{d.doktorAd}</option>
          ))}
        </select>
      </label>
      <label className="rd-alan">
        <span>Poliklinik</span>
        <select value={filtre.poliklinik} onChange={(e) => setFiltre((f) => ({ ...f, poliklinik: e.target.value }))}>
          <option value="">Tümü</option>
          {servisler.map((s) => (
            <option key={s.servisNo} value={s.servisAdi}>{s.servisAdi}</option>
          ))}
        </select>
      </label>
      <label className="rd-alan">
        <span>Durum</span>
        <select value={filtre.durum} onChange={(e) => setFiltre((f) => ({ ...f, durum: e.target.value }))}>
          <option value="tumu">Tümü</option>
          <option value="aktif">Planlandı</option>
          <option value="gecmis">Geçti</option>
          <option value="iptal">İptal Edildi</option>
        </select>
      </label>
    </div>
  );
}

// ── Liste görünümü: serbest tarih aralığı seçimi + tablo ──
function RandevuListesi({ filtre, yenidenYukleTetik, onIptalEdildi }) {
  const [baslangicTarih, setBaslangicTarih] = useState(bugun());
  const [bitisTarih, setBitisTarih] = useState(bugun());
  const [randevular, setRandevular] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');
  const [iptalEdilenId, setIptalEdilenId] = useState(null);

  const aralikGecersiz = baslangicTarih > bitisTarih;

  const yukle = useCallback(async () => {
    if (aralikGecersiz) return;
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await muayeneService.randevulariGetir(
        `${baslangicTarih}T00:00:00`,
        `${bitisTarih}T23:59:59`
      );
      setRandevular(data || []);
    } catch (err) {
      setRandevular([]);
      // Backend seçilen aralıkta randevu yoksa 404 dönüyor; bu bir hata değil, boş liste demek.
      if (err?.response?.status !== 404) {
        setHata(getApiErrorMessage(err, 'Randevu listesi alınamadı'));
      }
    } finally {
      setYukleniyor(false);
    }
  }, [baslangicTarih, bitisTarih, aralikGecersiz]);

  useEffect(() => {
    yukle();
  }, [yukle, yenidenYukleTetik]);

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

  const iptalEt = async (id) => {
    if (!window.confirm('Bu randevuyu iptal etmek istediğine emin misin?')) return;
    setIptalEdilenId(id);
    setHata('');
    try {
      await muayeneService.randevuIptalEt(id);
      await yukle();
      onIptalEdildi();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Randevu iptal edilemedi'));
    } finally {
      setIptalEdilenId(null);
    }
  };

  const gosterilecek = useMemo(() => filtreleUygula(randevular, filtre), [randevular, filtre]);

  return (
    <div className="rd-panel">
      <div className="rd-tarih-secim">
        <label className="rd-alan">
          <span>Başlangıç Tarihi</span>
          <input type="date" value={baslangicTarih} onChange={(e) => setBaslangicTarih(e.target.value)} />
        </label>
        <label className="rd-alan">
          <span>Bitiş Tarihi</span>
          <input type="date" value={bitisTarih} onChange={(e) => setBitisTarih(e.target.value)} />
        </label>
        <div className="rd-hizli-tarih">
          <button type="button" className="rd-btn" onClick={bugunSec}>Bugün</button>
          <button type="button" className="rd-btn" onClick={buHaftaSec}>Bu Hafta</button>
        </div>
      </div>

      {aralikGecersiz && <div className="rd-hata">Başlangıç tarihi bitiş tarihinden büyük olamaz.</div>}
      {!aralikGecersiz && hata && <div className="rd-hata">{hata}</div>}
      {!aralikGecersiz && yukleniyor && <p className="rd-bos-metin">Yükleniyor...</p>}
      {!aralikGecersiz && !yukleniyor && !hata && gosterilecek.length === 0 && (
        <p className="rd-bos-metin">Seçilen aralık ve filtrelerde randevu bulunmuyor.</p>
      )}

      {!aralikGecersiz && !yukleniyor && gosterilecek.length > 0 && (
        <div className="rd-tablo-kapsayici">
          <table className="rd-tablo">
            <thead>
              <tr>
                <th>Tarih</th>
                <th>Saat</th>
                <th>Hasta</th>
                <th>Poliklinik</th>
                <th>Doktor</th>
                <th>Durum</th>
                <th>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {gosterilecek.map((k) => {
                const durum = durumBilgisi(k);
                return (
                  <tr key={k.dosyaId}>
                    <td>{tarihFormat(k.randevuTarihi)}</td>
                    <td>{saatFormat(k.randevuTarihi)}</td>
                    <td>{k.ad} {k.soyad} <span className="rd-mute">#{k.protokol}</span></td>
                    <td>{k.poliklinik}</td>
                    <td>{k.doktor} <span className="rd-mute">({k.uzmanlikDali})</span></td>
                    <td><span className={`rd-durum-rozet ${durum.sinif}`}>{durum.etiket}</span></td>
                    <td><IptalButonu kayit={k} iptalEdilenId={iptalEdilenId} onIptal={iptalEt} /></td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

const GUN_ADLARI = ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi', 'Pazar'];

// ── Takvim görünümü: haftalık, gün bazlı sütunlar ──
function RandevuTakvimi({ filtre, yenidenYukleTetik, onIptalEdildi }) {
  const [haftaBaslangic, setHaftaBaslangic] = useState(() => haftaninPazartesisi(new Date()));
  const [randevular, setRandevular] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');
  const [iptalEdilenId, setIptalEdilenId] = useState(null);

  const haftaSonu = useMemo(() => {
    const d = new Date(haftaBaslangic);
    d.setDate(d.getDate() + 6);
    return d;
  }, [haftaBaslangic]);

  const yukle = useCallback(async () => {
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await muayeneService.randevulariGetir(
        `${yerelTarih(haftaBaslangic)}T00:00:00`,
        `${yerelTarih(haftaSonu)}T23:59:59`
      );
      setRandevular(data || []);
    } catch (err) {
      setRandevular([]);
      if (err?.response?.status !== 404) {
        setHata(getApiErrorMessage(err, 'Randevu listesi alınamadı'));
      }
    } finally {
      setYukleniyor(false);
    }
  }, [haftaBaslangic, haftaSonu]);

  useEffect(() => {
    yukle();
  }, [yukle, yenidenYukleTetik]);

  const iptalEt = async (id) => {
    if (!window.confirm('Bu randevuyu iptal etmek istediğine emin misin?')) return;
    setIptalEdilenId(id);
    setHata('');
    try {
      await muayeneService.randevuIptalEt(id);
      await yukle();
      onIptalEdildi();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Randevu iptal edilemedi'));
    } finally {
      setIptalEdilenId(null);
    }
  };

  const filtrelenmis = useMemo(() => filtreleUygula(randevular, filtre), [randevular, filtre]);

  const gunler = useMemo(() => (
    Array.from({ length: 7 }, (_, i) => {
      const gun = new Date(haftaBaslangic);
      gun.setDate(gun.getDate() + i);
      const gunAnahtari = yerelTarih(gun);
      const kayitlar = filtrelenmis
        .filter((k) => yerelTarih(new Date(k.randevuTarihi)) === gunAnahtari)
        .sort((a, b) => new Date(a.randevuTarihi) - new Date(b.randevuTarihi));
      return { tarih: gun, ad: GUN_ADLARI[i], kayitlar };
    })
  ), [haftaBaslangic, filtrelenmis]);

  const oncekiHafta = () => setHaftaBaslangic((h) => {
    const d = new Date(h);
    d.setDate(d.getDate() - 7);
    return d;
  });
  const sonrakiHafta = () => setHaftaBaslangic((h) => {
    const d = new Date(h);
    d.setDate(d.getDate() + 7);
    return d;
  });
  const buHafta = () => setHaftaBaslangic(haftaninPazartesisi(new Date()));

  return (
    <div className="rd-panel">
      <div className="rd-takvim-nav">
        <button type="button" className="rd-btn" onClick={oncekiHafta}>‹ Önceki Hafta</button>
        <span className="rd-hafta-araligi">{tarihFormat(haftaBaslangic)} – {tarihFormat(haftaSonu)}</span>
        <button type="button" className="rd-btn" onClick={buHafta}>Bu Hafta</button>
        <button type="button" className="rd-btn" onClick={sonrakiHafta}>Sonraki Hafta ›</button>
      </div>

      {hata && <div className="rd-hata">{hata}</div>}
      {yukleniyor && <p className="rd-bos-metin">Yükleniyor...</p>}

      {!yukleniyor && (
        <div className="rd-takvim-grid">
          {gunler.map((gun) => (
            <div key={gun.ad} className="rd-takvim-gun">
              <div className="rd-takvim-gun-baslik">
                <strong>{gun.ad}</strong>
                <span>{tarihFormat(gun.tarih)}</span>
              </div>
              {gun.kayitlar.length === 0 && <p className="rd-takvim-bos">Randevu yok</p>}
              <ul className="rd-takvim-liste">
                {gun.kayitlar.map((k) => {
                  const durum = durumBilgisi(k);
                  return (
                    <li key={k.dosyaId} className={`rd-takvim-kart ${durum.sinif}`}>
                      <div className="rd-takvim-kart-saat">{saatFormat(k.randevuTarihi)}</div>
                      <div className="rd-takvim-kart-hasta">{k.ad} {k.soyad}</div>
                      <div className="rd-takvim-kart-detay">{k.poliklinik} · {k.doktor}</div>
                      <IptalButonu kayit={k} iptalEdilenId={iptalEdilenId} onIptal={iptalEt} />
                    </li>
                  );
                })}
              </ul>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default function Randevu() {
  const { doktorlar, servisler, hata: listeHata } = useDoktorVeServisListesi();
  const [view, setView] = useState('liste');
  const [filtre, setFiltre] = useState({ doktor: '', poliklinik: '', durum: 'tumu' });
  const [yeniRandevuAcik, setYeniRandevuAcik] = useState(false);
  const [yenidenYukleTetik, setYenidenYukleTetik] = useState(0);

  const tetikle = () => setYenidenYukleTetik((t) => t + 1);

  return (
    <div className="sayfa">
      <div className="sayfa-baslik rd-baslik-satir">
        <h1>Randevu</h1>
        <button type="button" className="rd-btn rd-btn-birincil" onClick={() => setYeniRandevuAcik(true)}>
          + Yeni Randevu
        </button>
      </div>

      <div className="rd-panel rd-ust-panel">
        <div className="rd-view-toggle">
          <button
            type="button"
            className={`rd-btn ${view === 'liste' ? 'rd-btn-birincil' : ''}`}
            onClick={() => setView('liste')}
          >
            Liste
          </button>
          <button
            type="button"
            className={`rd-btn ${view === 'takvim' ? 'rd-btn-birincil' : ''}`}
            onClick={() => setView('takvim')}
          >
            Takvim
          </button>
        </div>

        {listeHata && <div className="rd-hata">{listeHata}</div>}
        <RandevuFiltreBar doktorlar={doktorlar} servisler={servisler} filtre={filtre} setFiltre={setFiltre} />
      </div>

      {view === 'liste' ? (
        <RandevuListesi filtre={filtre} yenidenYukleTetik={yenidenYukleTetik} onIptalEdildi={tetikle} />
      ) : (
        <RandevuTakvimi filtre={filtre} yenidenYukleTetik={yenidenYukleTetik} onIptalEdildi={tetikle} />
      )}

      {yeniRandevuAcik && (
        <YeniRandevuModal
          onKapat={() => setYeniRandevuAcik(false)}
          onBasarili={() => {
            setYeniRandevuAcik(false);
            tetikle();
          }}
        />
      )}
    </div>
  );
}
