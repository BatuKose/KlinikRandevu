import { useState, useEffect, useCallback } from 'react';
import { parametreService } from '../../services/parametreService';
import { getApiErrorMessage } from '../../utils/apiError';
import Bildirim from '../bildirim/Bildirim';
import './SmsAyarlari.css';

function SmsAyarModal({ ayar, onKaydet, onKapat }) {
  const [form, setForm] = useState({
    accountId: ayar?.accountId || '',
    token: '',
    fromNumber: ayar?.fromNumber || '',
  });
  const [tokenGoster, setTokenGoster] = useState(false);
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
        accountId: form.accountId.trim(),
        token: form.token.trim(),
        fromNumber: form.fromNumber.trim(),
      });
    } catch (err) {
      setHata(getApiErrorMessage(err, 'SMS ayarı kaydedilemedi'));
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onKapat}>
      <div className="modal-kart" onClick={(e) => e.stopPropagation()}>
        <div className="modal-baslik">
          <h2>{ayar ? 'SMS Ayarı Güncelle' : 'Yeni SMS Ayarı'}</h2>
          <button className="kapat-btn" onClick={onKapat}>✕</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="form-alani">
            <label>Account SID <span className="zorunlu">*</span></label>
            <input
              name="accountId"
              value={form.accountId}
              onChange={handleChange}
              required
              autoComplete="off"
              placeholder="ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            />
          </div>
          <div className="form-alani">
            <label>Auth Token {!ayar && <span className="zorunlu">*</span>}</label>
            <div className="sms-token-alani">
              <input
                name="token"
                type={tokenGoster ? 'text' : 'password'}
                value={form.token}
                onChange={handleChange}
                required={!ayar}
                autoComplete="new-password"
                placeholder={ayar ? `Mevcut: ${ayar.tokenMaskeli}` : 'Auth token giriniz'}
              />
              <button type="button" className="sms-goster-btn" onClick={() => setTokenGoster((g) => !g)}>
                {tokenGoster ? 'Gizle' : 'Göster'}
              </button>
            </div>
            {ayar && <small className="sms-ipucu">Boş bırakırsanız mevcut token korunur.</small>}
          </div>
          <div className="form-alani">
            <label>Gönderen Numara <span className="zorunlu">*</span></label>
            <input
              name="fromNumber"
              value={form.fromNumber}
              onChange={handleChange}
              required
              placeholder="+1xxxxxxxxxx"
            />
          </div>
          {!ayar && (
            <small className="sms-ipucu">
              Yeni kaydedilen ayar aktif olur, diğer tüm SMS ayarları pasife alınır.
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

export default function SmsAyarlari() {
  const [ayarlar, setAyarlar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [islemdekiId, setIslemdekiId] = useState(null);
  const [mesaj, setMesaj] = useState({ tip: '', metin: '' });
  const [modalAcik, setModalAcik] = useState(false);
  const [seciliAyar, setSeciliAyar] = useState(null);

  const ayarlariYukle = useCallback(async () => {
    setYukleniyor(true);
    try {
      const res = await parametreService.smsAyarlariGetir();
      setAyarlar(res.data?.data || []);
    } catch (err) {
      setMesaj({ tip: 'hata', metin: getApiErrorMessage(err, 'SMS ayarları yüklenemedi') });
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
      await parametreService.smsAyariGuncelle({
        id: seciliAyar.id,
        accountId: form.accountId,
        token: form.token || null,
        fromNumber: form.fromNumber,
      });
      setMesaj({ tip: 'basari', metin: 'SMS ayarı güncellendi.' });
    } else {
      await parametreService.smsAyariEkle({ ...form, isActive: true });
      setMesaj({ tip: 'basari', metin: 'SMS ayarı eklendi ve aktif edildi.' });
    }
    modalKapat();
    ayarlariYukle();
  };

  const aktifPasif = async (ayar) => {
    if (
      ayar.isActive &&
      !window.confirm('Bu ayar pasife alınırsa aktif SMS ayarı kalmayacak ve SMS gönderilemeyecek. Devam edilsin mi?')
    ) {
      return;
    }
    setIslemdekiId(ayar.id);
    try {
      const res = await parametreService.smsAyariAktifPasif(ayar.id);
      const aktifMi = res.data?.data;
      setMesaj({ tip: 'basari', metin: aktifMi ? 'SMS ayarı aktif edildi.' : 'SMS ayarı pasife alındı.' });
      ayarlariYukle();
    } catch (err) {
      setMesaj({ tip: 'hata', metin: getApiErrorMessage(err, 'Durum değiştirilemedi') });
    } finally {
      setIslemdekiId(null);
    }
  };

  const aktifVar = ayarlar.some((a) => a.isActive);

  return (
    <>
      <div className="ust-bar">
        <div>
          {!yukleniyor && !aktifVar && (
            <span className="sms-uyari">Aktif SMS ayarı yok — hastalara SMS gönderilemez.</span>
          )}
        </div>
        <div className="buton-grup">
          <button className="ekle-btn" onClick={() => modalAc()}>+ Yeni SMS Ayarı</button>
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
                <th>Account SID</th>
                <th>Auth Token</th>
                <th>Gönderen Numara</th>
                <th>Durum</th>
                <th>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {ayarlar.length === 0 ? (
                <tr>
                  <td colSpan={6} className="bos-satir">Tanımlı SMS ayarı bulunamadı</td>
                </tr>
              ) : (
                ayarlar.map((a) => (
                  <tr key={a.id}>
                    <td>{a.id}</td>
                    <td className="parametre-adi">{a.accountId}</td>
                    <td className="sms-token">{a.tokenMaskeli}</td>
                    <td>{a.fromNumber}</td>
                    <td>
                      <span className={`badge ${a.isActive ? 'badge-aktif' : 'badge-pasif'}`}>
                        {a.isActive ? 'Aktif' : 'Pasif'}
                      </span>
                    </td>
                    <td>
                      <div className="sms-islemler">
                        <button className="duzenle-btn" onClick={() => modalAc(a)}>Düzenle</button>
                        <button
                          className={`sms-durum-btn ${a.isActive ? 'pasif-yap' : 'aktif-yap'}`}
                          onClick={() => aktifPasif(a)}
                          disabled={islemdekiId === a.id}
                        >
                          {islemdekiId === a.id ? '...' : a.isActive ? 'Pasif Yap' : 'Aktif Yap'}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      {modalAcik && (
        <SmsAyarModal
          key={seciliAyar?.id ?? 'yeni'}
          ayar={seciliAyar}
          onKaydet={kaydet}
          onKapat={modalKapat}
        />
      )}
    </>
  );
}
