import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { hastaService } from '../services/hastaService';
import { muayeneService } from '../services/muayeneService';
import { getApiErrorMessage } from '../utils/apiError';
import {
  CINSIYET_SECENEKLERI,
  KAN_GRUBU_SECENEKLERI,
  cinsiyetEtiket,
  kanGrubuEtiket,
} from '../utils/hastaSecenekleri';
import { tarihFormat, saatFormat, yasHesapla } from '../utils/tarih';
import RandevuVerModal from '../components/hastaKayit/RandevuVerModal';
import MuayeneKaydiModal from '../components/hastaKayit/MuayeneKaydiModal';
import Bildirim from '../components/bildirim/Bildirim';
import Ikon from '../components/ui/Ikon';
import { SayfaBaslik, BosDurum, IskeletListe, Avatar, TarihKutu } from '../components/ui/Ortak';
import './HastaKayit.css';

const BOS_FORM = {
  name: '',
  surname: '',
  tcKimlik: '',
  phone: '',
  birthDate: '',
  gender: '',
  bloodType: '',
  address: '',
  email: '',
};

function tariheDonustur(deger) {
  return deger ? deger.slice(0, 10) : '';
}

function formaDonustur(hasta) {
  return {
    name: hasta.name || '',
    surname: hasta.surname || '',
    tcKimlik: hasta.tcKimlik ?? '',
    phone: hasta.phone || '',
    birthDate: tariheDonustur(hasta.birthDate),
    gender: hasta.gender ?? '',
    bloodType: hasta.bloodType ?? '',
    address: hasta.address || '',
    email: hasta.email || '',
  };
}

function formDegistiMi(form, hasta) {
  if (!hasta) return false;
  const orijinal = formaDonustur(hasta);
  return Object.keys(orijinal).some((alan) => String(form[alan]) !== String(orijinal[alan]));
}

// ── Seçili hastanın özet kartı: kimlik bilgileri + hızlı işlemler ──
function HastaBanner({ hasta, onRandevuVer, onMuayeneAc, onKapat }) {
  const yas = yasHesapla(hasta.birthDate);
  return (
    <section className="ui-kart hk-banner">
      <Avatar ad={hasta.name} soyad={hasta.surname} boyut="buyuk" />
      <div className="hk-banner-bilgi">
        <div className="hk-banner-ust">
          <h2>{hasta.name} {hasta.surname}</h2>
          <span className="ui-rozet ui-rozet-bilgi">Protokol #{hasta.protocol}</span>
        </div>
        <dl className="hk-banner-meta">
          <div>
            <Ikon ad="kimlik" />
            <dt>TC</dt>
            <dd className="ui-sayi">{hasta.tcKimlik || '-'}</dd>
          </div>
          <div>
            <Ikon ad="takvim" />
            <dt>Doğum</dt>
            <dd>{tarihFormat(hasta.birthDate)}{yas !== null && <span className="ui-soluk"> · {yas} yaş</span>}</dd>
          </div>
          <div>
            <Ikon ad="kullanici" />
            <dt>Cinsiyet</dt>
            <dd>{cinsiyetEtiket(hasta.gender)}</dd>
          </div>
          <div>
            <Ikon ad="damla" />
            <dt>Kan Grubu</dt>
            <dd><span className="hk-kan-grubu">{kanGrubuEtiket(hasta.bloodType)}</span></dd>
          </div>
          <div>
            <Ikon ad="telefon" />
            <dt>Telefon</dt>
            <dd className="ui-sayi">{hasta.phone || '-'}</dd>
          </div>
        </dl>
      </div>
      <div className="hk-banner-aksiyonlar">
        <button type="button" className="ui-btn ui-btn-birincil" onClick={onRandevuVer}>
          <Ikon ad="takvimArti" /> Randevu Ver
        </button>
        <button type="button" className="ui-btn" onClick={onMuayeneAc}>
          <Ikon ad="stetoskop" /> Muayene Aç
        </button>
        <button
          type="button"
          className="ui-btn ui-btn-hayalet ui-btn-ikon"
          onClick={onKapat}
          title="Hastayı kapat"
          aria-label="Hastayı kapat"
        >
          <Ikon ad="kapat" />
        </button>
      </div>
    </section>
  );
}

function YaklasanRandevular({ randevular, yukleniyor, iptalEdilenId, onIptal, onSec }) {
  return (
    <section className="ui-kart">
      <div className="ui-kart-baslik">
        <h2><Ikon ad="takvim" /> Yaklaşan Randevular</h2>
        {!yukleniyor && <span className="ui-sayac">{randevular.length}</span>}
      </div>
      {yukleniyor && <IskeletListe satir={2} avatar={false} />}
      {!yukleniyor && randevular.length === 0 && (
        <BosDurum kompakt ikon="takvim" aciklama="Planlanmış randevu bulunmuyor." />
      )}
      {!yukleniyor && randevular.length > 0 && (
        <ul className="hk-randevu-liste">
          {randevular.map((r) => (
            <li key={r.dosyaId} className={`hk-randevu${r.iptal ? ' hk-randevu-iptal' : ''}`}>
              <button type="button" className="hk-randevu-govde" onClick={() => onSec(r)} title="Muayene kaydına git">
                <TarihKutu tarih={r.randevuTarihi} pasif={r.iptal} />
                <div className="hk-randevu-bilgi">
                  <div className="hk-randevu-ust">
                    <strong>{r.poliklinik}</strong>
                    <span className="ui-sayi">{saatFormat(r.randevuTarihi)}</span>
                  </div>
                  <div className="ui-soluk">{r.doktor}{r.uzmanlikDali ? ` · ${r.uzmanlikDali}` : ''}</div>
                </div>
              </button>
              {r.iptal ? (
                <span className="ui-rozet ui-rozet-tehlike">İptal</span>
              ) : (
                <button
                  type="button"
                  className="ui-btn ui-btn-kucuk ui-btn-tehlike"
                  onClick={() => onIptal(r.dosyaId)}
                  disabled={iptalEdilenId === r.dosyaId}
                >
                  {iptalEdilenId === r.dosyaId ? <span className="ui-spinner" /> : 'İptal'}
                </button>
              )}
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}

function PoliklinikGecmisi({ kayitlar, yukleniyor, onSec }) {
  return (
    <section className="ui-kart">
      <div className="ui-kart-baslik">
        <h2><Ikon ad="aktivite" /> Poliklinik Geçmişi</h2>
        {!yukleniyor && <span className="ui-sayac">{kayitlar.length}</span>}
      </div>
      {yukleniyor && <IskeletListe satir={3} avatar={false} />}
      {!yukleniyor && kayitlar.length === 0 && (
        <BosDurum kompakt ikon="dosya" aciklama="Hastanın henüz poliklinik kaydı yok." />
      )}
      {!yukleniyor && kayitlar.length > 0 && (
        <ol className="hk-zaman">
          {kayitlar.map((k) => (
            <li key={k.muayeneId} className={`hk-zaman-oge${k.kapali ? '' : ' hk-zaman-acik'}`}>
              <button type="button" className="hk-zaman-kart" onClick={() => onSec(k)}>
                <div className="hk-zaman-ust">
                  <strong>{k.poliklinik || 'Poliklinik'}</strong>
                  <span className={`ui-rozet ui-rozet-nokta ${k.kapali ? '' : 'ui-rozet-basari'}`}>
                    {k.kapali ? 'Kapalı' : 'Açık'}
                  </span>
                </div>
                <div className="ui-soluk">
                  {k.doktor || '-'}{k.uzmanlikDali ? ` · ${k.uzmanlikDali}` : ''}
                </div>
                <div className="hk-zaman-alt">
                  <span className="ui-sayi">{tarihFormat(k.tarih)} · {saatFormat(k.tarih)}</span>
                  {!k.randevuId && <span className="hk-etiket">Randevusuz</span>}
                  <Ikon ad="sagOk" className="hk-zaman-ok" />
                </div>
              </button>
            </li>
          ))}
        </ol>
      )}
    </section>
  );
}

export default function HastaKayit() {
  const navigate = useNavigate();
  const [arama, setArama] = useState('');
  const [aramaHata, setAramaHata] = useState('');
  const [aramaYukleniyor, setAramaYukleniyor] = useState(false);
  const [aramaSonuclari, setAramaSonuclari] = useState([]);

  const [hasta, setHasta] = useState(null);
  const [yeniKayitModu, setYeniKayitModu] = useState(false);
  const [form, setForm] = useState(BOS_FORM);

  const [kaydediliyor, setKaydediliyor] = useState(false);
  const [formHata, setFormHata] = useState('');
  const [formBasari, setFormBasari] = useState('');

  const [randevular, setRandevular] = useState([]);
  const [gecmisKayitlar, setGecmisKayitlar] = useState([]);
  const [listeYukleniyor, setListeYukleniyor] = useState(false);
  const [listeHata, setListeHata] = useState('');

  const [randevuModalAcik, setRandevuModalAcik] = useState(false);
  const [muayeneModalAcik, setMuayeneModalAcik] = useState(false);
  const [iptalEdilenId, setIptalEdilenId] = useState(null);

  const secimiTemizle = () => {
    setHasta(null);
    setYeniKayitModu(false);
    setForm(BOS_FORM);
    setRandevular([]);
    setGecmisKayitlar([]);
    setListeHata('');
  };

  const randevuGecmisiniYukle = async (protokol) => {
    setListeYukleniyor(true);
    setListeHata('');
    try {
      // Randevular: sadece ileri tarihli randevular. Poliklinik kayıtları: randevulu ya da
      // randevusuz açılmış tüm muayene kayıtları (ayrı endpoint'ten).
      const [randevuSonuc, kayitSonuc] = await Promise.allSettled([
        muayeneService.hastaninRandevulariniGetir(protokol),
        muayeneService.hastaninPoliklinikKayitlariniGetir(protokol),
      ]);

      if (randevuSonuc.status === 'fulfilled') {
        const simdi = new Date();
        const gelecek = (randevuSonuc.value.data || [])
          .filter((kayit) => new Date(kayit.randevuTarihi) >= simdi)
          .sort((a, b) => new Date(a.randevuTarihi) - new Date(b.randevuTarihi));
        setRandevular(gelecek);
      } else {
        setRandevular([]);
        // Backend randevusu olmayan hasta için 404 dönüyor; bu bir hata değil, boş liste demek.
        if (randevuSonuc.reason?.response?.status !== 404) {
          setListeHata(getApiErrorMessage(randevuSonuc.reason, 'Randevu bilgileri alınamadı'));
        }
      }

      if (kayitSonuc.status === 'fulfilled') {
        setGecmisKayitlar(kayitSonuc.value.data?.data || []);
      } else {
        setGecmisKayitlar([]);
        setListeHata(getApiErrorMessage(kayitSonuc.reason, 'Poliklinik kayıtları alınamadı'));
      }
    } finally {
      setListeYukleniyor(false);
    }
  };

  const hastaSec = (secilen) => {
    setHasta(secilen);
    setYeniKayitModu(false);
    setForm(formaDonustur(secilen));
    setAramaSonuclari([]);
    setFormBasari('');
    setFormHata('');
    randevuGecmisiniYukle(secilen.protocol);
  };

  const randevuyuIptalEt = async (dosyaId) => {
    if (!window.confirm('Bu randevuyu iptal etmek istediğine emin misin?')) return;
    setIptalEdilenId(dosyaId);
    setListeHata('');
    try {
      await muayeneService.randevuIptalEt(dosyaId);
      await randevuGecmisiniYukle(hasta.protocol);
    } catch (err) {
      setListeHata(getApiErrorMessage(err, 'Randevu iptal edilemedi'));
    } finally {
      setIptalEdilenId(null);
    }
  };

  const muayeneyeGit = (kayit) => {
    navigate(`/poliklinik?randevuId=${kayit.dosyaId}`, {
      state: {
        ad: kayit.ad,
        soyad: kayit.soyad,
        tc: kayit.tc,
        protokol: kayit.protokol,
        doktorAd: kayit.doktor,
        poliklinikAd: kayit.poliklinik,
        randevuTarihi: kayit.randevuTarihi,
      },
    });
  };

  // Muayene kaydı zaten var; randevusuz açılmış olabileceği için muayeneId ile gidiyoruz.
  const poliklinikKaydinaGit = (kayit) => {
    navigate(`/poliklinik?muayeneId=${kayit.muayeneId}`, {
      state: {
        ad: kayit.ad,
        soyad: kayit.soyad,
        tc: kayit.tc,
        protokol: kayit.protokol,
        doktorAd: kayit.doktor,
        poliklinikAd: kayit.poliklinik,
        randevuTarihi: kayit.tarih,
      },
    });
  };

  const aramaYap = async (e) => {
    e?.preventDefault();
    const metin = arama.trim();
    if (metin.length < 3) {
      setAramaHata('Arama metni en az 3 karakter olmalıdır');
      return;
    }

    setAramaHata('');
    setFormBasari('');
    setFormHata('');
    setAramaSonuclari([]);
    setAramaYukleniyor(true);
    try {
      const { data } = await hastaService.ara(metin);
      const sonuclar = data || [];
      if (sonuclar.length === 0) {
        secimiTemizle();
        setAramaHata('Hasta kaydı bulunamadı');
      } else if (sonuclar.length === 1) {
        hastaSec(sonuclar[0]);
      } else {
        secimiTemizle();
        setAramaSonuclari(sonuclar);
      }
    } catch (err) {
      secimiTemizle();
      setAramaHata(getApiErrorMessage(err, 'Hasta kaydı bulunamadı'));
    } finally {
      setAramaYukleniyor(false);
    }
  };

  const yeniKayitBaslat = () => {
    secimiTemizle();
    setAramaSonuclari([]);
    setYeniKayitModu(true);
    setAramaHata('');
    setFormBasari('');
    const sayisalArama = arama.trim();
    if (/^\d{11}$/.test(sayisalArama)) {
      setForm({ ...BOS_FORM, tcKimlik: sayisalArama });
    }
  };

  const formDegistir = (alan, deger) => {
    setForm((prev) => ({ ...prev, [alan]: deger }));
  };

  const kaydet = async (e) => {
    e.preventDefault();
    setFormHata('');
    setFormBasari('');
    setKaydediliyor(true);
    try {
      if (yeniKayitModu) {
        const dto = {
          name: form.name.trim(),
          surname: form.surname.trim(),
          address: form.address.trim(),
          phone: form.phone.trim(),
          birthDate: form.birthDate,
          gender: Number(form.gender),
          bloodType: Number(form.bloodType),
          tcKimlik: Number(form.tcKimlik),
          email: form.email.trim() || null,
        };
        await hastaService.ekle(dto);
        setFormBasari('Hasta başarıyla kaydedildi.');
        setArama(String(dto.tcKimlik));
        const { data } = await hastaService.ara(String(dto.tcKimlik));
        const olusan = data?.[0];
        if (olusan) hastaSec(olusan);
      } else {
        const dto = {
          name: form.name.trim() || null,
          surname: form.surname.trim() || null,
          address: form.address.trim() || null,
          phone: form.phone.trim() || null,
          birthDate: form.birthDate || null,
          gender: form.gender !== '' ? Number(form.gender) : null,
          bloodType: form.bloodType !== '' ? Number(form.bloodType) : null,
          tcKimlik: form.tcKimlik ? Number(form.tcKimlik) : null,
          email: form.email.trim() || null,
        };
        await hastaService.guncelle(hasta.protocol, dto);
        // Banner ve "değişiklik var" durumu yeni değerleri göstersin diye yerel kaydı da güncelliyoruz.
        setHasta((h) => ({
          ...h,
          ...Object.fromEntries(Object.entries(dto).filter(([, v]) => v !== null)),
        }));
        setFormBasari('Hasta bilgileri güncellendi.');
      }
    } catch (err) {
      setFormHata(getApiErrorMessage(err));
    } finally {
      setKaydediliyor(false);
    }
  };

  const hastaSecili = !!hasta;
  const degisiklikVar = hastaSecili && formDegistiMi(form, hasta);

  return (
    <div className="sayfa">
      <SayfaBaslik
        ikon="kullanicilar"
        baslik="Hasta Kayıt"
        aciklama="Hasta arayın, kimlik bilgilerini güncelleyin; randevu ve muayene işlemlerini başlatın."
      >
        <button type="button" className="ui-btn ui-btn-birincil" onClick={yeniKayitBaslat}>
          <Ikon ad="kullaniciEkle" /> Yeni Hasta
        </button>
      </SayfaBaslik>

      {/* ── Arama ── */}
      <form className="ui-kart hk-arama" onSubmit={aramaYap}>
        <div className="hk-arama-kutu">
          <Ikon ad="ara" className="hk-arama-ikon" />
          <input
            type="search"
            placeholder="Protokol no, TC kimlik no veya ad soyad ile ara"
            value={arama}
            onChange={(e) => setArama(e.target.value)}
            disabled={aramaYukleniyor}
            autoFocus
          />
          {aramaYukleniyor ? (
            <span className="ui-spinner hk-arama-spinner" />
          ) : (
            <kbd className="hk-kbd">Enter</kbd>
          )}
        </div>
        <button type="submit" className="ui-btn ui-btn-birincil" disabled={aramaYukleniyor}>
          Ara
        </button>
      </form>

      {aramaHata && (
        <div className="hk-arama-uyari" role="status">
          <Ikon ad="bilgi" />
          <span>{aramaHata}</span>
          <button type="button" className="ui-btn ui-btn-kucuk" onClick={yeniKayitBaslat}>
            <Ikon ad="kullaniciEkle" /> Yeni hasta kaydı oluştur
          </button>
        </div>
      )}

      {aramaSonuclari.length > 1 && (
        <section className="ui-kart hk-sonuclar">
          <div className="ui-kart-baslik">
            <h2><Ikon ad="kullanicilar" /> Arama Sonuçları</h2>
            <span className="ui-sayac">{aramaSonuclari.length}</span>
          </div>
          <ul>
            {aramaSonuclari.map((s) => (
              <li key={s.protocol}>
                <button type="button" className="hk-sonuc" onClick={() => hastaSec(s)}>
                  <Avatar ad={s.name} soyad={s.surname} />
                  <div className="hk-sonuc-bilgi">
                    <strong>{s.name} {s.surname}</strong>
                    <span className="ui-soluk ui-sayi">
                      #{s.protocol} · TC {s.tcKimlik || '-'} · {tarihFormat(s.birthDate)}
                    </span>
                  </div>
                  <Ikon ad="sagOk" className="hk-sonuc-ok" />
                </button>
              </li>
            ))}
          </ul>
        </section>
      )}

      {!hastaSecili && !yeniKayitModu && aramaSonuclari.length === 0 && (
        <section className="ui-kart">
          <BosDurum
            ikon="ara"
            baslik="Henüz hasta seçilmedi"
            aciklama="Yukarıdan protokol numarası, TC kimlik numarası ya da ad soyad ile arayın. Kayıtlı değilse yeni hasta kaydı oluşturabilirsiniz."
          />
        </section>
      )}

      {hastaSecili && (
        <HastaBanner
          hasta={hasta}
          onRandevuVer={() => setRandevuModalAcik(true)}
          onMuayeneAc={() => setMuayeneModalAcik(true)}
          onKapat={secimiTemizle}
        />
      )}

      {(hastaSecili || yeniKayitModu) && (
        <div className={`hk-duzen${yeniKayitModu ? ' hk-duzen-tek' : ''}`}>
          <form className="ui-kart hk-form" onSubmit={kaydet}>
            <div className="ui-kart-baslik">
              <h2>
                <Ikon ad={yeniKayitModu ? 'kullaniciEkle' : 'dosya'} />
                {yeniKayitModu ? 'Yeni Hasta Kaydı' : 'Hasta Bilgileri'}
              </h2>
              {degisiklikVar && <span className="ui-rozet ui-rozet-uyari ui-rozet-nokta">Kaydedilmemiş değişiklik</span>}
            </div>

            <div className="hk-form-govde">
              <fieldset className="hk-bolum">
                <legend>Kimlik Bilgileri</legend>
                <div className="hk-alan-grid">
                  <label className="ui-alan">
                    <span>Ad<b className="ui-zorunlu">*</b></span>
                    <input className="ui-input" value={form.name} onChange={(e) => formDegistir('name', e.target.value)} required />
                  </label>
                  <label className="ui-alan">
                    <span>Soyad<b className="ui-zorunlu">*</b></span>
                    <input className="ui-input" value={form.surname} onChange={(e) => formDegistir('surname', e.target.value)} required />
                  </label>
                  <label className="ui-alan">
                    <span>TC Kimlik No{yeniKayitModu && <b className="ui-zorunlu">*</b>}</span>
                    <input
                      className="ui-input ui-sayi"
                      value={form.tcKimlik}
                      onChange={(e) => formDegistir('tcKimlik', e.target.value.replace(/\D/g, ''))}
                      disabled={!yeniKayitModu}
                      inputMode="numeric"
                      maxLength={11}
                      minLength={11}
                      required={yeniKayitModu}
                    />
                  </label>
                  <label className="ui-alan">
                    <span>Doğum Tarihi{yeniKayitModu && <b className="ui-zorunlu">*</b>}</span>
                    <input
                      className="ui-input"
                      type="date"
                      value={form.birthDate}
                      onChange={(e) => formDegistir('birthDate', e.target.value)}
                      required={yeniKayitModu}
                    />
                  </label>
                  <label className="ui-alan">
                    <span>Cinsiyet{yeniKayitModu && <b className="ui-zorunlu">*</b>}</span>
                    <select className="ui-input" value={form.gender} onChange={(e) => formDegistir('gender', e.target.value)} required={yeniKayitModu}>
                      <option value="">Seçiniz</option>
                      {CINSIYET_SECENEKLERI.map((c) => (
                        <option key={c.deger} value={c.deger}>{c.etiket}</option>
                      ))}
                    </select>
                  </label>
                  <label className="ui-alan">
                    <span>Kan Grubu{yeniKayitModu && <b className="ui-zorunlu">*</b>}</span>
                    <select className="ui-input" value={form.bloodType} onChange={(e) => formDegistir('bloodType', e.target.value)} required={yeniKayitModu}>
                      <option value="">Seçiniz</option>
                      {KAN_GRUBU_SECENEKLERI.map((k) => (
                        <option key={k.deger} value={k.deger}>{k.etiket}</option>
                      ))}
                    </select>
                  </label>
                </div>
              </fieldset>

              <fieldset className="hk-bolum">
                <legend>İletişim Bilgileri</legend>
                <div className="hk-alan-grid">
                  <label className="ui-alan">
                    <span>Telefon<b className="ui-zorunlu">*</b></span>
                    <input
                      className="ui-input ui-sayi"
                      type="tel"
                      placeholder="05xx xxx xx xx"
                      value={form.phone}
                      onChange={(e) => formDegistir('phone', e.target.value)}
                      required
                    />
                  </label>
                  <label className="ui-alan">
                    <span>E-posta</span>
                    <input
                      className="ui-input"
                      type="email"
                      placeholder="ornek@eposta.com"
                      value={form.email}
                      onChange={(e) => formDegistir('email', e.target.value)}
                    />
                  </label>
                  <label className="ui-alan hk-alan-genis">
                    <span>Adres<b className="ui-zorunlu">*</b></span>
                    <textarea
                      className="ui-input"
                      value={form.address}
                      onChange={(e) => formDegistir('address', e.target.value)}
                      rows={2}
                      required
                    />
                  </label>
                </div>
              </fieldset>
            </div>

            <div className="hk-form-alt">
              {yeniKayitModu ? (
                <button type="button" className="ui-btn ui-btn-hayalet" onClick={secimiTemizle} disabled={kaydediliyor}>
                  Vazgeç
                </button>
              ) : (
                <button
                  type="button"
                  className="ui-btn ui-btn-hayalet"
                  onClick={() => setForm(formaDonustur(hasta))}
                  disabled={!degisiklikVar || kaydediliyor}
                >
                  Değişiklikleri geri al
                </button>
              )}
              <button
                type="submit"
                className="ui-btn ui-btn-birincil"
                disabled={kaydediliyor || (hastaSecili && !degisiklikVar)}
              >
                {kaydediliyor ? <span className="ui-spinner" /> : <Ikon ad="kaydet" />}
                {yeniKayitModu ? 'Hastayı Kaydet' : 'Değişiklikleri Kaydet'}
              </button>
            </div>

            <Bildirim mesaj={formHata} onKapat={() => setFormHata('')} />
            <Bildirim mesaj={formBasari} tip="basari" onKapat={() => setFormBasari('')} />
          </form>

          {hastaSecili && (
            <aside className="hk-yan">
              <YaklasanRandevular
                randevular={randevular}
                yukleniyor={listeYukleniyor}
                iptalEdilenId={iptalEdilenId}
                onIptal={randevuyuIptalEt}
                onSec={muayeneyeGit}
              />
              <PoliklinikGecmisi
                kayitlar={gecmisKayitlar}
                yukleniyor={listeYukleniyor}
                onSec={poliklinikKaydinaGit}
              />
              <Bildirim mesaj={listeHata} />
            </aside>
          )}
        </div>
      )}

      {randevuModalAcik && hasta && (
        <RandevuVerModal
          hasta={hasta}
          onKapat={() => setRandevuModalAcik(false)}
          onBasarili={() => {
            setRandevuModalAcik(false);
            randevuGecmisiniYukle(hasta.protocol);
          }}
        />
      )}

      {muayeneModalAcik && hasta && (
        <MuayeneKaydiModal
          hasta={hasta}
          onKapat={() => setMuayeneModalAcik(false)}
          onBasarili={() => {
            setMuayeneModalAcik(false);
            randevuGecmisiniYukle(hasta.protocol);
          }}
        />
      )}
    </div>
  );
}
