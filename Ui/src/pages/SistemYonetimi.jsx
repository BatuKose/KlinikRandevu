import { useState, useEffect, useCallback, useRef } from 'react';
import { parametreService } from '../services/parametreService';
import { baseURL } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';
import LogListesi from '../components/sistemYonetimi/LogListesi';
import DoktorEkle from '../components/sistemYonetimi/DoktorEkle';
import ServisEkle from '../components/sistemYonetimi/ServisEkle';
import './SistemYonetimi.css';

const BOSH_PARAMETRE = {
  parametreAdi: '',
  deger1: '',
  deger2: '',
  deger3: '',
  deger4: '',
  deger5: '',
  aciklama: '',
};

function ParametreModal({ parametre, onKaydet, onKapat }) {
  const [form, setForm] = useState(() =>
    parametre
      ? {
          parametreAdi: parametre.parametreAdi || '',
          deger1: parametre.deger1 || '',
          deger2: parametre.deger2 || '',
          deger3: parametre.deger3 || '',
          deger4: parametre.deger4 || '',
          deger5: parametre.deger5 || '',
          aciklama: parametre.aciklama || '',
        }
      : BOSH_PARAMETRE
  );
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setHata('');
    setYukleniyor(true);
    try {
      await onKaydet(form);
    } catch (err) {
      setHata(getApiErrorMessage(err));
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onKapat}>
      <div className="modal-kart" onClick={(e) => e.stopPropagation()}>
        <div className="modal-baslik">
          <h2>{parametre ? 'Parametre Güncelle' : 'Yeni Parametre Ekle'}</h2>
          <button className="kapat-btn" onClick={onKapat}>✕</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="form-alani">
            <label>Parametre Adı <span className="zorunlu">*</span></label>
            <input
              name="parametreAdi"
              value={form.parametreAdi}
              onChange={handleChange}
              required
              disabled={!!parametre}
              placeholder="Parametre adını giriniz"
            />
          </div>
          {['deger1', 'deger2', 'deger3', 'deger4', 'deger5'].map((alan, i) => (
            <div className="form-alani" key={alan}>
              <label>Değer {i + 1}</label>
              <input
                name={alan}
                value={form[alan]}
                onChange={handleChange}
                placeholder={`Değer ${i + 1} giriniz`}
              />
            </div>
          ))}
          <div className="form-alani">
            <label>Açıklama</label>
            <textarea
              name="aciklama"
              value={form.aciklama}
              onChange={handleChange}
              rows={3}
              placeholder="Açıklama giriniz"
            />
          </div>
          {hata && <div className="hata-mesaj">{hata}</div>}
          <div className="modal-footer">
            <button type="button" className="iptal-btn" onClick={onKapat}>İptal</button>
            <button type="submit" className="kaydet-btn" disabled={yukleniyor}>
              {yukleniyor ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function KullaniciEkle() {
  const [form, setForm] = useState({
    username: '',
    password: '',
    email: '',
    name: '',
    surname: '',
  });
  const [yukleniyor, setYukleniyor] = useState(false);
  const [mesaj, setMesaj] = useState({ tip: '', metin: '' });

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMesaj({ tip: '', metin: '' });
    setYukleniyor(true);
    try {
      await parametreService.kullaniciEkle(form);
      setMesaj({ tip: 'basari', metin: 'Kullanıcı başarıyla eklendi.' });
      setForm({ username: '', password: '', email: '', name: '', surname: '' });
    } catch (err) {
      setMesaj({
        tip: 'hata',
        metin: getApiErrorMessage(err),
      });
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="kullanici-ekle-kart">
      <h3>Kullanıcı Ekle</h3>
      <form onSubmit={handleSubmit} className="kullanici-form">
        <div className="form-satir">
          <div className="form-alani">
            <label>Ad <span className="zorunlu">*</span></label>
            <input name="name" value={form.name} onChange={handleChange} required placeholder="Ad" />
          </div>
          <div className="form-alani">
            <label>Soyad <span className="zorunlu">*</span></label>
            <input name="surname" value={form.surname} onChange={handleChange} required placeholder="Soyad" />
          </div>
        </div>
        <div className="form-satir">
          <div className="form-alani">
            <label>Kullanıcı Adı <span className="zorunlu">*</span></label>
            <input name="username" value={form.username} onChange={handleChange} required placeholder="Kullanıcı adı" />
          </div>
          <div className="form-alani">
            <label>E-posta</label>
            <input name="email" type="email" value={form.email} onChange={handleChange} placeholder="ornek@mail.com" />
          </div>
        </div>
        <div className="form-alani">
          <label>Şifre <span className="zorunlu">*</span></label>
          <input name="password" type="password" value={form.password} onChange={handleChange} required placeholder="Şifre (min 4 karakter)" />
        </div>
        {mesaj.metin && (
          <div className={`bildirim ${mesaj.tip === 'basari' ? 'bildirim-basari' : 'bildirim-hata'}`}>
            {mesaj.metin}
          </div>
        )}
        <button type="submit" className="kaydet-btn" disabled={yukleniyor}>
          {yukleniyor ? 'Ekleniyor...' : 'Kullanıcı Ekle'}
        </button>
      </form>
    </div>
  );
}

export default function SistemYonetimi() {
  const [aktifTab, setAktifTab] = useState('parametreler');
  const [parametreler, setParametreler] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');
  const [modalAcik, setModalAcik] = useState(false);
  const [seciliParametre, setSeciliParametre] = useState(null);
  const [cacheBildirim, setCacheBildirim] = useState('');
  const cacheTimerRef = useRef(null);

  const parametreleriYukle = useCallback(async () => {
    setYukleniyor(true);
    setHata('');
    try {
      const res = await parametreService.hepsiniGetir();
      setParametreler(res.data);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Parametreler yüklenemedi'));
    } finally {
      setYukleniyor(false);
    }
  }, []);

  useEffect(() => {
    if (aktifTab === 'parametreler') {
      // eslint-disable-next-line react/set-state-in-effect
      parametreleriYukle();
    }
  }, [aktifTab, parametreleriYukle]);

  const modalAc = (parametre = null) => {
    setSeciliParametre(parametre);
    setModalAcik(true);
  };

  const modalKapat = () => {
    setModalAcik(false);
    setSeciliParametre(null);
  };

  const kaydet = async (form) => {
    if (seciliParametre) {
      await parametreService.guncelle(seciliParametre.id, form);
    } else {
      await parametreService.ekle(form);
    }
    modalKapat();
    parametreleriYukle();
  };

  const cacheTemizle = async () => {
    const bildirimGoster = (metin) => {
      setCacheBildirim(metin);
      if (cacheTimerRef.current) clearTimeout(cacheTimerRef.current);
      cacheTimerRef.current = setTimeout(() => setCacheBildirim(''), 3000);
    };
    try {
      await parametreService.cacheTemizle();
      bildirimGoster('Cache başarıyla temizlendi');
    } catch (err) {
      bildirimGoster(getApiErrorMessage(err, 'Cache temizlenemedi'));
    }
  };

  useEffect(() => () => { if (cacheTimerRef.current) clearTimeout(cacheTimerRef.current); }, []);

  const formatTarih = (tarih) => {
    if (!tarih) return '-';
    return new Date(tarih).toLocaleDateString('tr-TR', {
      day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
    });
  };

  return (
    <div className="sayfa">
      <div className="sayfa-baslik">
        <h1>Sistem Yönetimi</h1>
      </div>

      <div className="tab-bar">
        <button
          className={`tab-btn ${aktifTab === 'parametreler' ? 'aktif' : ''}`}
          onClick={() => setAktifTab('parametreler')}
        >
          Parametreler
        </button>
        <button
          className={`tab-btn ${aktifTab === 'kullanici' ? 'aktif' : ''}`}
          onClick={() => setAktifTab('kullanici')}
        >
          Kullanıcı Ekle
        </button>
        <button
          className={`tab-btn ${aktifTab === 'doktor' ? 'aktif' : ''}`}
          onClick={() => setAktifTab('doktor')}
        >
          Doktor Ekle
        </button>
        <button
          className={`tab-btn ${aktifTab === 'servis' ? 'aktif' : ''}`}
          onClick={() => setAktifTab('servis')}
        >
          Servis Ekle
        </button>
        <button
          className={`tab-btn ${aktifTab === 'loglar' ? 'aktif' : ''}`}
          onClick={() => setAktifTab('loglar')}
        >
          Loglar
        </button>
        <a
          className="tab-btn tab-link"
          href={`${baseURL}/hangfire/recurring`}
          target="_blank"
          rel="noopener noreferrer"
          title="Hangfire job panelini yeni sekmede açar"
        >
          Job Servisi ↗
        </a>
      </div>

      {aktifTab === 'parametreler' && (
        <div className="icerik">
          <div className="ust-bar">
            <div>
              {cacheBildirim && <span className="cache-bildirim">{cacheBildirim}</span>}
            </div>
            <div className="buton-grup">
              <button className="cache-btn" onClick={cacheTemizle}>Cache Temizle</button>
              <button className="ekle-btn" onClick={() => modalAc()}>+ Yeni Parametre</button>
            </div>
          </div>

          {hata && <div className="bildirim bildirim-hata">{hata}</div>}

          {yukleniyor ? (
            <div className="yukleniyor">Yükleniyor...</div>
          ) : (
            <div className="tablo-kapsayici">
              <table className="tablo">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Parametre Adı</th>
                    <th>Değer 1</th>
                    <th>Değer 2</th>
                    <th>Değer 3</th>
                    <th>Değer 4</th>
                    <th>Değer 5</th>
                    <th>Açıklama</th>
                    <th>Aktif</th>
                    <th>Oluşturma</th>
                    <th>Güncelleme</th>
                    <th>İşlem</th>
                  </tr>
                </thead>
                <tbody>
                  {parametreler.length === 0 ? (
                    <tr>
                      <td colSpan={12} className="bos-satir">Kayıt bulunamadı</td>
                    </tr>
                  ) : (
                    parametreler.map((p) => (
                      <tr key={p.id}>
                        <td>{p.id}</td>
                        <td className="parametre-adi">{p.parametreAdi}</td>
                        <td>{p.deger1 || '-'}</td>
                        <td>{p.deger2 || '-'}</td>
                        <td>{p.deger3 || '-'}</td>
                        <td>{p.deger4 || '-'}</td>
                        <td>{p.deger5 || '-'}</td>
                        <td className="aciklama-cell">{p.aciklama || '-'}</td>
                        <td>
                          <span className={`badge ${p.aktif ? 'badge-aktif' : 'badge-pasif'}`}>
                            {p.aktif ? 'Aktif' : 'Pasif'}
                          </span>
                        </td>
                        <td className="tarih-cell">{formatTarih(p.olusturmaTarihi)}</td>
                        <td className="tarih-cell">{formatTarih(p.guncellemeTarihi)}</td>
                        <td>
                          <button className="duzenle-btn" onClick={() => modalAc(p)}>Düzenle</button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {aktifTab === 'kullanici' && (
        <div className="icerik">
          <KullaniciEkle />
        </div>
      )}

      {aktifTab === 'doktor' && (
        <div className="icerik">
          <DoktorEkle />
        </div>
      )}

      {aktifTab === 'servis' && (
        <div className="icerik">
          <ServisEkle />
        </div>
      )}

      {aktifTab === 'loglar' && (
        <div className="icerik">
          <LogListesi />
        </div>
      )}

      {modalAcik && (
        <ParametreModal
          key={seciliParametre?.id ?? 'yeni'}
          parametre={seciliParametre}
          onKaydet={kaydet}
          onKapat={modalKapat}
        />
      )}
    </div>
  );
}
