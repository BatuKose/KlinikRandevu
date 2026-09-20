import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { hastaService } from '../services/hastaService';
import { muayeneService } from '../services/muayeneService';
import { getApiErrorMessage } from '../utils/apiError';
import { CINSIYET_SECENEKLERI, KAN_GRUBU_SECENEKLERI } from '../utils/hastaSecenekleri';
import RandevuVerModal from '../components/hastaKayit/RandevuVerModal';
import MuayeneKaydiModal from '../components/hastaKayit/MuayeneKaydiModal';
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

function tarihSaatFormat(deger) {
  if (!deger) return '-';
  return new Date(deger).toLocaleString('tr-TR', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
  });
}

export default function HastaKayit() {
  const navigate = useNavigate();
  const [arama, setArama] = useState('');
  const [aramaHata, setAramaHata] = useState('');
  const [aramaYukleniyor, setAramaYukleniyor] = useState(false);

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
      const { data } = await muayeneService.hastaninRandevulariniGetir(protokol);
      const simdi = new Date();
      const gecmis = [];
      const gelecek = [];
      (data || []).forEach((kayit) => {
        (new Date(kayit.randevuTarihi) < simdi ? gecmis : gelecek).push(kayit);
      });
      gecmis.sort((a, b) => new Date(b.randevuTarihi) - new Date(a.randevuTarihi));
      gelecek.sort((a, b) => new Date(a.randevuTarihi) - new Date(b.randevuTarihi));
      setGecmisKayitlar(gecmis);
      setRandevular(gelecek);
    } catch (err) {
      setGecmisKayitlar([]);
      setRandevular([]);
      // Backend randevusu olmayan hasta için 404 dönüyor; bu bir hata değil, boş liste demek.
      if (err?.response?.status !== 404) {
        setListeHata(getApiErrorMessage(err, 'Randevu bilgileri alınamadı'));
      }
    } finally {
      setListeYukleniyor(false);
    }
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

  const aramaYap = async () => {
    const metin = arama.trim();
    if (metin.length < 3) {
      setAramaHata('Arama metni en az 3 karakter olmalıdır');
      return;
    }

    setAramaHata('');
    setFormBasari('');
    setFormHata('');
    setAramaYukleniyor(true);
    try {
      const { data } = await hastaService.ara(metin);
      const bulunan = data?.[0];
      if (!bulunan) {
        secimiTemizle();
        setAramaHata('Hasta kaydı bulunamadı');
        return;
      }
      setHasta(bulunan);
      setYeniKayitModu(false);
      setForm(formaDonustur(bulunan));
      randevuGecmisiniYukle(bulunan.protocol);
    } catch (err) {
      secimiTemizle();
      setAramaHata(getApiErrorMessage(err, 'Hasta kaydı bulunamadı'));
    } finally {
      setAramaYukleniyor(false);
    }
  };

  const aramaKeyDown = (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      aramaYap();
    }
  };

  const yeniKayitBaslat = () => {
    secimiTemizle();
    setYeniKayitModu(true);
    setAramaHata('');
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
        if (olusan) {
          setHasta(olusan);
          setYeniKayitModu(false);
          setForm(formaDonustur(olusan));
          randevuGecmisiniYukle(olusan.protocol);
        }
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
        setFormBasari('Hasta bilgileri güncellendi.');
      }
    } catch (err) {
      setFormHata(getApiErrorMessage(err));
    } finally {
      setKaydediliyor(false);
    }
  };

  const hastaSecili = !!hasta;

  return (
    <div className="sayfa hasta-kayit-sayfa">
      <div className="sayfa-baslik">
        <h1>Hasta Kayıt</h1>
      </div>

      <div className="hasta-kayit-grid">
        {/* ── SOL: Arama + Hasta Bilgileri ── */}
        <div className="hk-panel hk-sol">
          <div className="hk-arama">
            <input
              type="text"
              placeholder="Protokol, TC veya isimle ara ve Enter'a bas..."
              value={arama}
              onChange={(e) => setArama(e.target.value)}
              onKeyDown={aramaKeyDown}
              disabled={aramaYukleniyor}
            />
            <button onClick={aramaYap} disabled={aramaYukleniyor} className="hk-arama-btn">
              {aramaYukleniyor ? '...' : 'Ara'}
            </button>
          </div>

          {aramaHata && (
            <div className="hk-uyari">
              {aramaHata}
              <button type="button" className="hk-link-btn" onClick={yeniKayitBaslat}>
                Yeni hasta kaydı oluştur
              </button>
            </div>
          )}

          {(hastaSecili || yeniKayitModu) && (
            <form className="hk-form" onSubmit={kaydet}>
              <div className="hk-form-baslik">
                <h2>{yeniKayitModu ? 'Yeni Hasta Kaydı' : `Protokol: ${hasta.protocol}`}</h2>
                {!yeniKayitModu && (
                  <button type="button" className="hk-link-btn" onClick={secimiTemizle}>
                    Kapat
                  </button>
                )}
              </div>

              <div className="hk-alan-grid">
                <label className="hk-alan">
                  <span>Ad</span>
                  <input value={form.name} onChange={(e) => formDegistir('name', e.target.value)} required />
                </label>
                <label className="hk-alan">
                  <span>Soyad</span>
                  <input value={form.surname} onChange={(e) => formDegistir('surname', e.target.value)} required />
                </label>
                <label className="hk-alan">
                  <span>TC Kimlik No</span>
                  <input
                    value={form.tcKimlik}
                    onChange={(e) => formDegistir('tcKimlik', e.target.value.replace(/\D/g, ''))}
                    disabled={!yeniKayitModu}
                    maxLength={11}
                    required={yeniKayitModu}
                  />
                </label>
                <label className="hk-alan">
                  <span>Telefon</span>
                  <input value={form.phone} onChange={(e) => formDegistir('phone', e.target.value)} required />
                </label>
                <label className="hk-alan">
                  <span>Doğum Tarihi</span>
                  <input
                    type="date"
                    value={form.birthDate}
                    onChange={(e) => formDegistir('birthDate', e.target.value)}
                    required={yeniKayitModu}
                  />
                </label>
                <label className="hk-alan">
                  <span>Cinsiyet</span>
                  <select value={form.gender} onChange={(e) => formDegistir('gender', e.target.value)} required={yeniKayitModu}>
                    <option value="">Seçiniz</option>
                    {CINSIYET_SECENEKLERI.map((c) => (
                      <option key={c.deger} value={c.deger}>{c.etiket}</option>
                    ))}
                  </select>
                </label>
                <label className="hk-alan">
                  <span>Kan Grubu</span>
                  <select value={form.bloodType} onChange={(e) => formDegistir('bloodType', e.target.value)} required={yeniKayitModu}>
                    <option value="">Seçiniz</option>
                    {KAN_GRUBU_SECENEKLERI.map((k) => (
                      <option key={k.deger} value={k.deger}>{k.etiket}</option>
                    ))}
                  </select>
                </label>
                <label className="hk-alan">
                  <span>E-posta</span>
                  <input type="email" value={form.email} onChange={(e) => formDegistir('email', e.target.value)} />
                </label>
                <label className="hk-alan hk-alan-genis">
                  <span>Adres</span>
                  <textarea value={form.address} onChange={(e) => formDegistir('address', e.target.value)} rows={2} required />
                </label>
              </div>

              {formHata && <div className="hk-uyari hk-uyari-hata">{formHata}</div>}
              {formBasari && <div className="hk-uyari hk-uyari-basari">{formBasari}</div>}

              <div className="hk-form-aksiyonlar">
                <button type="submit" className="hk-btn hk-btn-birincil" disabled={kaydediliyor}>
                  {kaydediliyor ? 'Kaydediliyor...' : yeniKayitModu ? 'Hastayı Kaydet' : 'Bilgileri Güncelle'}
                </button>
                {!yeniKayitModu && (
                  <>
                    <button type="button" className="hk-btn" onClick={() => setRandevuModalAcik(true)}>
                      Randevu Ver
                    </button>
                    <button type="button" className="hk-btn" onClick={() => setMuayeneModalAcik(true)}>
                      Muayene Kaydı Aç
                    </button>
                  </>
                )}
              </div>
            </form>
          )}

          {!hastaSecili && !yeniKayitModu && !aramaHata && (
            <div className="hk-bos-durum">
              <p>Hasta bilgilerini görmek için protokol, TC veya isimle arama yap.</p>
              <button type="button" className="hk-link-btn" onClick={yeniKayitBaslat}>
                veya yeni hasta kaydı oluştur
              </button>
            </div>
          )}
        </div>

        {/* ── SAĞ: Randevular + Poliklinik kayıtları ── */}
        <div className="hk-panel hk-sag">
          <div className="hk-liste-bolum">
            <h2>Randevular</h2>
            {!hastaSecili && <p className="hk-bos-metin">Bir hasta seçildiğinde planlı randevular burada listelenir.</p>}
            {hastaSecili && !listeYukleniyor && !listeHata && randevular.length === 0 && (
              <p className="hk-bos-metin">Planlı randevu bulunmuyor.</p>
            )}
            {randevular.length > 0 && (
              <ul className="hk-kayit-liste">
                {randevular.map((r) => (
                  <li
                    key={r.dosyaId}
                    className={`hk-kayit-item hk-kayit-item-tiklanabilir ${r.iptal ? 'hk-kayit-item-iptal' : 'hk-kayit-item-aktif'}`}
                    onClick={() => muayeneyeGit(r)}
                    title="Muayene kaydını görüntülemek için tıkla"
                  >
                    <div className="hk-kayit-satir1">
                      <span className="hk-kayit-poliklinik">{r.poliklinik}</span>
                      <span className="hk-kayit-tarih">{tarihSaatFormat(r.randevuTarihi)}</span>
                    </div>
                    <div className="hk-kayit-satir2">{r.doktor} · {r.uzmanlikDali}</div>
                    {r.iptal ? (
                      <span className="hk-kayit-iptal-etiket">İptal Edildi</span>
                    ) : (
                      <button
                        type="button"
                        className="hk-kayit-iptal-btn"
                        onClick={(e) => {
                          e.stopPropagation();
                          randevuyuIptalEt(r.dosyaId);
                        }}
                        disabled={iptalEdilenId === r.dosyaId}
                      >
                        {iptalEdilenId === r.dosyaId ? 'İptal ediliyor...' : 'İptal Et'}
                      </button>
                    )}
                  </li>
                ))}
              </ul>
            )}
          </div>

          <div className="hk-liste-bolum">
            <h2>Poliklinik Kayıtları</h2>
            {!hastaSecili && <p className="hk-bos-metin">Bir hasta seçildiğinde geçmiş kayıtlar burada listelenir.</p>}
            {hastaSecili && listeYukleniyor && <p className="hk-bos-metin">Yükleniyor...</p>}
            {hastaSecili && !listeYukleniyor && listeHata && <div className="hk-uyari hk-uyari-hata">{listeHata}</div>}
            {hastaSecili && !listeYukleniyor && !listeHata && gecmisKayitlar.length === 0 && (
              <p className="hk-bos-metin">Geçmiş poliklinik kaydı bulunmuyor.</p>
            )}
            {gecmisKayitlar.length > 0 && (
              <ul className="hk-kayit-liste">
                {gecmisKayitlar.map((k) => (
                  <li
                    key={k.dosyaId}
                    className="hk-kayit-item hk-kayit-item-tiklanabilir"
                    onClick={() => muayeneyeGit(k)}
                    title="Muayene kaydını görüntülemek için tıkla"
                  >
                    <div className="hk-kayit-satir1">
                      <span className="hk-kayit-poliklinik">{k.poliklinik}</span>
                      <span className="hk-kayit-tarih">{tarihSaatFormat(k.randevuTarihi)}</span>
                    </div>
                    <div className="hk-kayit-satir2">{k.doktor} · {k.uzmanlikDali}</div>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      </div>

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
