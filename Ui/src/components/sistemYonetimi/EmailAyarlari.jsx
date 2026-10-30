import { useState, useEffect, useCallback } from 'react';
import { parametreService } from '../../services/parametreService';
import { getApiErrorMessage } from '../../utils/apiError';
import Bildirim from '../bildirim/Bildirim';
import './EmailAyarlari.css';

function EmailAyarModal({ ayar, onKaydet, onKapat }) {
  const [form, setForm] = useState({
    smtpServer: ayar?.smtpServer || '',
    port: ayar?.port ?? 587,
    senderName: ayar?.senderName || '',
    senderEmail: ayar?.senderEmail || '',
    password: '',
  });
  const [sifreGoster, setSifreGoster] = useState(false);
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
      await onKaydet({
        smtpServer: form.smtpServer.trim(),
        port: Number(form.port),
        senderName: form.senderName.trim(),
        senderEmail: form.senderEmail.trim(),
        password: form.password.trim(),
      });
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Email ayarı kaydedilemedi'));
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onKapat}>
      <div className="modal-kart" onClick={(e) => e.stopPropagation()}>
        <div className="modal-baslik">
          <h2>{ayar ? 'Email Ayarı Güncelle' : 'Yeni Email Ayarı'}</h2>
          <button className="kapat-btn" onClick={onKapat}>✕</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="email-satir">
            <div className="form-alani email-smtp">
              <label>SMTP Sunucu <span className="zorunlu">*</span></label>
              <input
                name="smtpServer"
                value={form.smtpServer}
                onChange={handleChange}
                required
                autoComplete="off"
                placeholder="smtp.gmail.com"
              />
            </div>
            <div className="form-alani email-port">
              <label>Port <span className="zorunlu">*</span></label>
              <input
                name="port"
                type="number"
                min={1}
                max={65535}
                value={form.port}
                onChange={handleChange}
                required
              />
            </div>
          </div>
          <div className="form-alani">
            <label>Gönderen Adı <span className="zorunlu">*</span></label>
            <input
              name="senderName"
              value={form.senderName}
              onChange={handleChange}
              required
              placeholder="Klinik Randevu"
            />
          </div>
          <div className="form-alani">
            <label>Gönderen Email <span className="zorunlu">*</span></label>
            <input
              name="senderEmail"
              type="email"
              value={form.senderEmail}
              onChange={handleChange}
              required
              autoComplete="off"
              placeholder="ornek@klinik.com"
            />
          </div>
          <div className="form-alani">
            <label>Şifre {!ayar && <span className="zorunlu">*</span>}</label>
            <div className="email-sifre-alani">
              <input
                name="password"
                type={sifreGoster ? 'text' : 'password'}
                value={form.password}
                onChange={handleChange}
                required={!ayar}
                autoComplete="new-password"
                placeholder={ayar ? `Mevcut: ${ayar.passwordMaskeli}` : 'Uygulama şifresi giriniz'}
              />
              <button type="button" className="email-goster-btn" onClick={() => setSifreGoster((g) => !g)}>
                {sifreGoster ? 'Gizle' : 'Göster'}
              </button>
            </div>
            {ayar && <small className="email-ipucu">Boş bırakırsanız mevcut şifre korunur.</small>}
          </div>
          {!ayar && (
            <small className="email-ipucu">
              Yeni kaydedilen ayar pasif olarak eklenir. Kullanmak için listeden aktif yapınız.
            </small>
          )}
          <Bildirim mesaj={hata} onKapat={() => setHata('')} />
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

export default function EmailAyarlari() {
  const [ayarlar, setAyarlar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [islemdekiId, setIslemdekiId] = useState(null);
  const [mesaj, setMesaj] = useState({ tip: '', metin: '' });
  const [modalAcik, setModalAcik] = useState(false);
  const [seciliAyar, setSeciliAyar] = useState(null);

  const ayarlariYukle = useCallback(async () => {
    setYukleniyor(true);
    try {
      const res = await parametreService.emailAyarlariGetir();
      setAyarlar(res.data?.data || []);
    } catch (err) {
      setMesaj({ tip: 'hata', metin: getApiErrorMessage(err, 'Email ayarları yüklenemedi') });
    } finally {
      setYukleniyor(false);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react/set-state-in-effect
    ayarlariYukle();
  }, [ayarlariYukle]);

  const modalAc = (ayar = null) => {
    setSeciliAyar(ayar);
    setModalAcik(true);
  };

  const modalKapat = () => {
    setModalAcik(false);
    setSeciliAyar(null);
  };

  const kaydet = async (form) => {
    if (seciliAyar) {
      await parametreService.emailAyariGuncelle({
        id: seciliAyar.id,
        ...form,
        password: form.password || null,
      });
      setMesaj({ tip: 'basari', metin: 'Email ayarı güncellendi.' });
    } else {
      await parametreService.emailAyariEkle(form);
      setMesaj({ tip: 'basari', metin: 'Email ayarı pasif olarak eklendi.' });
    }
    modalKapat();
    ayarlariYukle();
  };

  const aktifPasif = async (ayar) => {
    if (
      ayar.active &&
      !window.confirm('Bu ayar pasife alınırsa aktif email ayarı kalmayacak ve email gönderilemeyecek. Devam edilsin mi?')
    ) {
      return;
    }
    setIslemdekiId(ayar.id);
    try {
      const res = await parametreService.emailAyariAktifPasif(ayar.id);
      const aktifMi = res.data?.data;
      setMesaj({ tip: 'basari', metin: aktifMi ? 'Email ayarı aktif edildi.' : 'Email ayarı pasife alındı.' });
      ayarlariYukle();
    } catch (err) {
      setMesaj({ tip: 'hata', metin: getApiErrorMessage(err, 'Durum değiştirilemedi') });
    } finally {
      setIslemdekiId(null);
    }
  };

  const aktifVar = ayarlar.some((a) => a.active);

  return (
    <>
      <div className="ust-bar">
        <div>
          {!yukleniyor && !aktifVar && (
            <span className="email-uyari">Aktif email ayarı yok — hastalara email gönderilemez.</span>
          )}
        </div>
        <div className="buton-grup">
          <button className="ekle-btn" onClick={() => modalAc()}>+ Yeni Email Ayarı</button>
        </div>
      </div>

      <Bildirim
        mesaj={mesaj.metin}
        tip={mesaj.tip === 'basari' ? 'basari' : 'hata'}
        onKapat={() => setMesaj({ tip: '', metin: '' })}
      />

      {yukleniyor ? (
        <div className="yukleniyor">Yükleniyor...</div>
      ) : (
        <div className="tablo-kapsayici">
          <table className="tablo">
            <thead>
              <tr>
                <th>ID</th>
                <th>SMTP Sunucu</th>
                <th>Port</th>
                <th>Gönderen Adı</th>
                <th>Gönderen Email</th>
                <th>Şifre</th>
                <th>Durum</th>
                <th>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {ayarlar.length === 0 ? (
                <tr>
                  <td colSpan={8} className="bos-satir">Tanımlı email ayarı bulunamadı</td>
                </tr>
              ) : (
                ayarlar.map((a) => {
                  // Başka bir ayar aktifken bu ayar aktif yapılamaz; önce aktif olan pasife alınmalı.
                  const aktifYapilamaz = !a.active && aktifVar;
                  return (
                    <tr key={a.id}>
                      <td>{a.id}</td>
                      <td className="parametre-adi">{a.smtpServer}</td>
                      <td>{a.port}</td>
                      <td>{a.senderName}</td>
                      <td>{a.senderEmail}</td>
                      <td className="email-sifre">{a.passwordMaskeli}</td>
                      <td>
                        <span className={`badge ${a.active ? 'badge-aktif' : 'badge-pasif'}`}>
                          {a.active ? 'Aktif' : 'Pasif'}
                        </span>
                      </td>
                      <td>
                        <div className="email-islemler">
                          <button className="duzenle-btn" onClick={() => modalAc(a)}>Düzenle</button>
                          <button
                            className={`email-durum-btn ${a.active ? 'pasif-yap' : 'aktif-yap'}`}
                            onClick={() => aktifPasif(a)}
                            disabled={islemdekiId === a.id || aktifYapilamaz}
                            title={aktifYapilamaz ? 'Önce aktif olan email ayarını pasife alınız' : undefined}
                          >
                            {islemdekiId === a.id ? '...' : a.active ? 'Pasif Yap' : 'Aktif Yap'}
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      )}

      {modalAcik && (
        <EmailAyarModal
          key={seciliAyar?.id ?? 'yeni'}
          ayar={seciliAyar}
          onKaydet={kaydet}
          onKapat={modalKapat}
        />
      )}
    </>
  );
}
