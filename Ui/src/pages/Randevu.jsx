import { useEffect, useMemo, useState } from 'react';
import { muayeneService } from '../services/muayeneService';
import { getApiErrorMessage } from '../utils/apiError';
import {
  yerelTarih,
  bugun,
  haftaninPazartesisi,
  gunEkle,
  tarihFormat,
  uzunTarihFormat,
  saatFormat,
} from '../utils/tarih';
import { useDoktorVeServisListesi } from '../components/hastaKayit/useDoktorVeServisListesi';
import YeniRandevuModal from '../components/randevu/YeniRandevuModal';
import CalismaPlanlari from '../components/randevu/CalismaPlanlari';
import Bildirim from '../components/bildirim/Bildirim';
import Ikon from '../components/ui/Ikon';
import { SayfaBaslik, BosDurum, IskeletListe, IstatistikKart, Avatar } from '../components/ui/Ortak';
import './Randevu.css';

const GUN_ADLARI = ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi', 'Pazar'];

const DURUMLAR = {
  planlandi: { etiket: 'Planlandı', rozet: 'ui-rozet-bilgi' },
  gecti: { etiket: 'Geçti', rozet: '' },
  iptal: { etiket: 'İptal Edildi', rozet: 'ui-rozet-tehlike' },
};

function durumAnahtari(kayit, simdi = new Date()) {
  if (kayit.iptal) return 'iptal';
  if (new Date(kayit.randevuTarihi) < simdi) return 'gecti';
  return 'planlandi';
}

function ayinAraligi() {
  const simdi = new Date();
  return {
    baslangic: yerelTarih(new Date(simdi.getFullYear(), simdi.getMonth(), 1)),
    bitis: yerelTarih(new Date(simdi.getFullYear(), simdi.getMonth() + 1, 0)),
  };
}

function haftaninAraligi() {
  const pazartesi = haftaninPazartesisi(new Date());
  return { baslangic: yerelTarih(pazartesi), bitis: yerelTarih(gunEkle(pazartesi, 6)) };
}

const HIZLI_ARALIKLAR = [
  { anahtar: 'bugun', etiket: 'Bugün', hesapla: () => ({ baslangic: bugun(), bitis: bugun() }) },
  { anahtar: 'hafta', etiket: 'Bu Hafta', hesapla: haftaninAraligi },
  { anahtar: 'ay', etiket: 'Bu Ay', hesapla: ayinAraligi },
];

// Seçilen aralıktaki randevuları getirir; aralık değişince eski istek sonucunu yok sayar.
function useRandevular(baslangic, bitis, aktif, tetik) {
  const [randevular, setRandevular] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');

  useEffect(() => {
    if (!aktif) return undefined;
    let iptal = false;
    (async () => {
      setYukleniyor(true);
      setHata('');
      try {
        const { data } = await muayeneService.randevulariGetir(`${baslangic}T00:00:00`, `${bitis}T23:59:59`);
        if (!iptal) setRandevular(data || []);
      } catch (err) {
        if (iptal) return;
        setRandevular([]);
        // Backend seçilen aralıkta randevu yoksa 404 dönüyor; bu bir hata değil, boş liste demek.
        if (err?.response?.status !== 404) {
          setHata(getApiErrorMessage(err, 'Randevu listesi alınamadı'));
        }
      } finally {
        if (!iptal) setYukleniyor(false);
      }
    })();
    return () => {
      iptal = true;
    };
  }, [baslangic, bitis, aktif, tetik]);

  return { randevular, yukleniyor, hata };
}

function IptalButonu({ kayit, iptalEdilenId, onIptal }) {
  if (durumAnahtari(kayit) !== 'planlandi') return null;
  const isleniyor = iptalEdilenId === kayit.dosyaId;
  return (
    <button
      type="button"
      className="ui-btn ui-btn-kucuk ui-btn-tehlike"
      onClick={(e) => {
        e.stopPropagation();
        onIptal(kayit.dosyaId);
      }}
      disabled={isleniyor}
    >
      {isleniyor ? <span className="ui-spinner" /> : 'İptal Et'}
    </button>
  );
}

// ── Liste görünümü: güne göre gruplanmış tablo ──
function RandevuTablosu({ kayitlar, iptalEdilenId, onIptal }) {
  const gruplar = useMemo(() => {
    const harita = new Map();
    [...kayitlar]
      .sort((a, b) => new Date(a.randevuTarihi) - new Date(b.randevuTarihi))
      .forEach((k) => {
        const anahtar = yerelTarih(new Date(k.randevuTarihi));
        if (!harita.has(anahtar)) harita.set(anahtar, []);
        harita.get(anahtar).push(k);
      });
    return [...harita.entries()];
  }, [kayitlar]);

  return (
    <div className="ui-tablo-kapsayici">
      <table className="ui-tablo rd-tablo-yeni">
        <thead>
          <tr>
            <th style={{ width: 90 }}>Saat</th>
            <th>Hasta</th>
            <th>Poliklinik</th>
            <th>Doktor</th>
            <th>Durum</th>
            <th className="ui-tablo-sag">İşlem</th>
          </tr>
        </thead>
        <tbody>
          {gruplar.map(([gun, liste]) => (
            <GunGrubu key={gun} gun={gun} liste={liste} iptalEdilenId={iptalEdilenId} onIptal={onIptal} />
          ))}
        </tbody>
      </table>
    </div>
  );
}

function GunGrubu({ gun, liste, iptalEdilenId, onIptal }) {
  const bugunMu = gun === bugun();
  return (
    <>
      <tr className="ui-tablo-grup">
        <td colSpan={6}>
          {uzunTarihFormat(`${gun}T00:00:00`)}
          {bugunMu && <span className="ui-rozet ui-rozet-bilgi rd-bugun-rozet">Bugün</span>}
          <span className="rd-grup-sayi">{liste.length} randevu</span>
        </td>
      </tr>
      {liste.map((k) => {
        const durum = durumAnahtari(k);
        return (
          <tr key={k.dosyaId} className={durum === 'iptal' ? 'rd-satir-iptal' : ''}>
            <td>
              <span className="rd-saat ui-sayi">{saatFormat(k.randevuTarihi)}</span>
            </td>
            <td>
              <div className="rd-hasta">
                <Avatar ad={k.ad} soyad={k.soyad} boyut="kucuk" />
                <div>
                  <div className="rd-hasta-ad">{k.ad} {k.soyad}</div>
                  <div className="ui-soluk ui-sayi rd-alt-metin">#{k.protokol}</div>
                </div>
              </div>
            </td>
            <td>{k.poliklinik}</td>
            <td>
              <div>{k.doktor}</div>
              {k.uzmanlikDali && <div className="ui-soluk rd-alt-metin">{k.uzmanlikDali}</div>}
            </td>
            <td>
              <span className={`ui-rozet ui-rozet-nokta ${DURUMLAR[durum].rozet}`}>{DURUMLAR[durum].etiket}</span>
            </td>
            <td className="ui-tablo-sag">
              <IptalButonu kayit={k} iptalEdilenId={iptalEdilenId} onIptal={onIptal} />
            </td>
          </tr>
        );
      })}
    </>
  );
}

// ── Takvim görünümü: haftalık, gün bazlı sütunlar ──
function RandevuTakvimi({ haftaBaslangic, kayitlar, iptalEdilenId, onIptal }) {
  const bugunAnahtar = bugun();
  const gunler = useMemo(
    () =>
      Array.from({ length: 7 }, (_, i) => {
        const tarih = gunEkle(haftaBaslangic, i);
        const anahtar = yerelTarih(tarih);
        const liste = kayitlar
          .filter((k) => yerelTarih(new Date(k.randevuTarihi)) === anahtar)
          .sort((a, b) => new Date(a.randevuTarihi) - new Date(b.randevuTarihi));
        return { tarih, anahtar, ad: GUN_ADLARI[i], liste, haftaSonu: i >= 5 };
      }),
    [haftaBaslangic, kayitlar]
  );

  return (
    <div className="rd-takvim-kaydirma">
      <div className="rd-takvim">
        {gunler.map((gun) => (
          <div
            key={gun.anahtar}
            className={`rd-gun${gun.haftaSonu ? ' rd-gun-haftasonu' : ''}${gun.anahtar === bugunAnahtar ? ' rd-gun-bugun' : ''}`}
          >
            <div className="rd-gun-baslik">
              <span className="rd-gun-ad">{gun.ad}</span>
              <span className="rd-gun-no">{gun.tarih.getDate()}</span>
              {gun.liste.length > 0 && <span className="ui-sayac">{gun.liste.length}</span>}
            </div>
            <div className="rd-gun-govde">
              {gun.liste.length === 0 && <p className="rd-gun-bos">Randevu yok</p>}
              {gun.liste.map((k) => {
                const durum = durumAnahtari(k);
                return (
                  <article key={k.dosyaId} className={`rd-etkinlik rd-etkinlik-${durum}`}>
                    <div className="rd-etkinlik-ust">
                      <span className="ui-sayi">{saatFormat(k.randevuTarihi)}</span>
                      {durum !== 'planlandi' && <span className="rd-etkinlik-durum">{DURUMLAR[durum].etiket}</span>}
                    </div>
                    <div className="rd-etkinlik-hasta">{k.ad} {k.soyad}</div>
                    <div className="rd-etkinlik-detay">{k.poliklinik}</div>
                    <div className="rd-etkinlik-detay">{k.doktor}</div>
                    <IptalButonu kayit={k} iptalEdilenId={iptalEdilenId} onIptal={onIptal} />
                  </article>
                );
              })}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default function Randevu() {
  const { doktorlar, servisler, hata: listeHata } = useDoktorVeServisListesi();
  const [view, setView] = useState('liste');
  const [filtre, setFiltre] = useState({ doktor: '', poliklinik: '' });
  const [durumFiltre, setDurumFiltre] = useState('tumu');
  const [listeAralik, setListeAralik] = useState({ baslangic: bugun(), bitis: bugun() });
  const [haftaBaslangic, setHaftaBaslangic] = useState(() => haftaninPazartesisi(new Date()));
  const [yeniRandevuAcik, setYeniRandevuAcik] = useState(false);
  const [yenidenYukleTetik, setYenidenYukleTetik] = useState(0);
  const [iptalEdilenId, setIptalEdilenId] = useState(null);
  const [iptalHata, setIptalHata] = useState('');

  const tetikle = () => setYenidenYukleTetik((t) => t + 1);

  const aralik = view === 'takvim'
    ? { baslangic: yerelTarih(haftaBaslangic), bitis: yerelTarih(gunEkle(haftaBaslangic, 6)) }
    : listeAralik;
  const aralikGecersiz = aralik.baslangic > aralik.bitis;
  const randevuGorunumu = view !== 'plan';

  const { randevular, yukleniyor, hata } = useRandevular(
    aralik.baslangic,
    aralik.bitis,
    randevuGorunumu && !aralikGecersiz,
    yenidenYukleTetik
  );

  // Backend DTO'da doktor/poliklinik için sadece ad döndürüyor (id yok); filtre isim eşleşmesiyle.
  const filtrelenmis = useMemo(
    () =>
      randevular.filter(
        (k) => (!filtre.doktor || k.doktor === filtre.doktor) && (!filtre.poliklinik || k.poliklinik === filtre.poliklinik)
      ),
    [randevular, filtre]
  );

  const sayilar = useMemo(() => {
    const simdi = new Date();
    const s = { tumu: filtrelenmis.length, planlandi: 0, gecti: 0, iptal: 0 };
    filtrelenmis.forEach((k) => {
      s[durumAnahtari(k, simdi)] += 1;
    });
    return s;
  }, [filtrelenmis]);

  const gosterilecek = useMemo(
    () => (durumFiltre === 'tumu' ? filtrelenmis : filtrelenmis.filter((k) => durumAnahtari(k) === durumFiltre)),
    [filtrelenmis, durumFiltre]
  );

  const iptalEt = async (id) => {
    if (!window.confirm('Bu randevuyu iptal etmek istediğine emin misin?')) return;
    setIptalEdilenId(id);
    setIptalHata('');
    try {
      await muayeneService.randevuIptalEt(id);
      tetikle();
    } catch (err) {
      setIptalHata(getApiErrorMessage(err, 'Randevu iptal edilemedi'));
    } finally {
      setIptalEdilenId(null);
    }
  };

  const aktifHizliAralik = HIZLI_ARALIKLAR.find((h) => {
    const a = h.hesapla();
    return a.baslangic === listeAralik.baslangic && a.bitis === listeAralik.bitis;
  })?.anahtar;

  const filtreAktif = filtre.doktor || filtre.poliklinik || durumFiltre !== 'tumu';
  const haftaSonu = gunEkle(haftaBaslangic, 6);

  return (
    <div className="sayfa">
      <SayfaBaslik
        ikon="takvim"
        baslik="Randevu"
        aciklama="Randevuları planlayın, takvim üzerinden takip edin ve doktor çalışma planlarını yönetin."
      >
        <button type="button" className="ui-btn ui-btn-birincil" onClick={() => setYeniRandevuAcik(true)}>
          <Ikon ad="takvimArti" /> Yeni Randevu
        </button>
      </SayfaBaslik>

      <nav className="ui-sekmeler" aria-label="Randevu görünümü">
        {[
          { anahtar: 'liste', etiket: 'Liste', ikon: 'liste' },
          { anahtar: 'takvim', etiket: 'Haftalık Takvim', ikon: 'izgara' },
          { anahtar: 'plan', etiket: 'Çalışma Planları', ikon: 'saat' },
        ].map((s) => (
          <button
            key={s.anahtar}
            type="button"
            className={`ui-sekme${view === s.anahtar ? ' ui-sekme-aktif' : ''}`}
            onClick={() => setView(s.anahtar)}
          >
            <Ikon ad={s.ikon} /> {s.etiket}
          </button>
        ))}
      </nav>

      {view === 'plan' && <CalismaPlanlari />}

      {randevuGorunumu && (
        <>
          <Bildirim mesaj={listeHata} />
          <Bildirim mesaj={hata} />
          <Bildirim mesaj={iptalHata} onKapat={() => setIptalHata('')} />
          {aralikGecersiz && <Bildirim mesaj="Başlangıç tarihi bitiş tarihinden büyük olamaz." tip="uyari" />}

          <section className="ui-kart rd-arac">
            <div className="ui-arac-cubugu">
              {view === 'liste' ? (
                <>
                  <label className="ui-alan rd-tarih-alan">
                    <span>Başlangıç</span>
                    <input
                      className="ui-input"
                      type="date"
                      value={listeAralik.baslangic}
                      onChange={(e) => setListeAralik((a) => ({ ...a, baslangic: e.target.value }))}
                    />
                  </label>
                  <label className="ui-alan rd-tarih-alan">
                    <span>Bitiş</span>
                    <input
                      className="ui-input"
                      type="date"
                      value={listeAralik.bitis}
                      onChange={(e) => setListeAralik((a) => ({ ...a, bitis: e.target.value }))}
                    />
                  </label>
                  <div className="ui-segment rd-hizli">
                    {HIZLI_ARALIKLAR.map((h) => (
                      <button
                        key={h.anahtar}
                        type="button"
                        className={aktifHizliAralik === h.anahtar ? 'ui-segment-aktif' : ''}
                        onClick={() => setListeAralik(h.hesapla())}
                      >
                        {h.etiket}
                      </button>
                    ))}
                  </div>
                </>
              ) : (
                <div className="rd-hafta-nav">
                  <button
                    type="button"
                    className="ui-btn ui-btn-ikon"
                    onClick={() => setHaftaBaslangic((h) => gunEkle(h, -7))}
                    aria-label="Önceki hafta"
                  >
                    <Ikon ad="solOk" />
                  </button>
                  <button
                    type="button"
                    className="ui-btn ui-btn-ikon"
                    onClick={() => setHaftaBaslangic((h) => gunEkle(h, 7))}
                    aria-label="Sonraki hafta"
                  >
                    <Ikon ad="sagOk" />
                  </button>
                  <div className="rd-hafta-etiket">
                    <strong>{tarihFormat(haftaBaslangic)} – {tarihFormat(haftaSonu)}</strong>
                    <span className="ui-soluk">Haftalık görünüm</span>
                  </div>
                  <button type="button" className="ui-btn" onClick={() => setHaftaBaslangic(haftaninPazartesisi(new Date()))}>
                    Bu Hafta
                  </button>
                </div>
              )}

              <div className="ui-arac-bosluk" />

              <label className="ui-alan">
                <span>Doktor</span>
                <select
                  className="ui-input"
                  value={filtre.doktor}
                  onChange={(e) => setFiltre((f) => ({ ...f, doktor: e.target.value }))}
                >
                  <option value="">Tüm doktorlar</option>
                  {doktorlar.map((d) => (
                    <option key={d.doktorNo} value={d.doktorAd}>{d.doktorAd}</option>
                  ))}
                </select>
              </label>
              <label className="ui-alan">
                <span>Poliklinik</span>
                <select
                  className="ui-input"
                  value={filtre.poliklinik}
                  onChange={(e) => setFiltre((f) => ({ ...f, poliklinik: e.target.value }))}
                >
                  <option value="">Tüm poliklinikler</option>
                  {servisler.map((s) => (
                    <option key={s.servisNo} value={s.servisAdi}>{s.servisAdi}</option>
                  ))}
                </select>
              </label>
              <button
                type="button"
                className="ui-btn ui-btn-ikon"
                onClick={tetikle}
                disabled={yukleniyor}
                title="Yenile"
                aria-label="Yenile"
              >
                {yukleniyor ? <span className="ui-spinner" /> : <Ikon ad="yenile" />}
              </button>
            </div>
          </section>

          <div className="ui-istatistik-grid">
            <IstatistikKart ikon="takvim" deger={sayilar.tumu} etiket="Toplam randevu" />
            <IstatistikKart ikon="saat" ton="bilgi" deger={sayilar.planlandi} etiket="Planlanan" />
            <IstatistikKart ikon="takvimTik" ton="basari" deger={sayilar.gecti} etiket="Geçmiş" />
            <IstatistikKart ikon="takvimX" ton="tehlike" deger={sayilar.iptal} etiket="İptal edilen" />
          </div>

          <section className="ui-kart rd-icerik">
            <div className="ui-kart-baslik rd-icerik-baslik">
              <div className="ui-segment" role="tablist" aria-label="Durum filtresi">
                {[
                  { anahtar: 'tumu', etiket: 'Tümü' },
                  { anahtar: 'planlandi', etiket: 'Planlandı' },
                  { anahtar: 'gecti', etiket: 'Geçti' },
                  { anahtar: 'iptal', etiket: 'İptal' },
                ].map((d) => (
                  <button
                    key={d.anahtar}
                    type="button"
                    role="tab"
                    aria-selected={durumFiltre === d.anahtar}
                    className={durumFiltre === d.anahtar ? 'ui-segment-aktif' : ''}
                    onClick={() => setDurumFiltre(d.anahtar)}
                  >
                    {d.etiket} <span className="ui-sayac">{sayilar[d.anahtar]}</span>
                  </button>
                ))}
              </div>
              {filtreAktif && (
                <button
                  type="button"
                  className="ui-btn ui-btn-hayalet ui-btn-kucuk"
                  onClick={() => {
                    setFiltre({ doktor: '', poliklinik: '' });
                    setDurumFiltre('tumu');
                  }}
                >
                  <Ikon ad="kapat" /> Filtreleri temizle
                </button>
              )}
            </div>

            {yukleniyor && view === 'liste' && <IskeletListe satir={5} />}

            {!yukleniyor && view === 'liste' && gosterilecek.length === 0 && !aralikGecersiz && (
              <BosDurum
                ikon="takvim"
                baslik="Randevu bulunamadı"
                aciklama={`${tarihFormat(listeAralik.baslangic)} – ${tarihFormat(listeAralik.bitis)} aralığında seçili filtrelere uyan randevu yok.`}
              >
                <button type="button" className="ui-btn ui-btn-birincil" onClick={() => setYeniRandevuAcik(true)}>
                  <Ikon ad="takvimArti" /> Yeni Randevu
                </button>
              </BosDurum>
            )}

            {!yukleniyor && view === 'liste' && gosterilecek.length > 0 && (
              <RandevuTablosu kayitlar={gosterilecek} iptalEdilenId={iptalEdilenId} onIptal={iptalEt} />
            )}

            {view === 'takvim' && (
              <div className={yukleniyor ? 'rd-yukleniyor' : ''}>
                <RandevuTakvimi
                  haftaBaslangic={haftaBaslangic}
                  kayitlar={gosterilecek}
                  iptalEdilenId={iptalEdilenId}
                  onIptal={iptalEt}
                />
              </div>
            )}
          </section>
        </>
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
