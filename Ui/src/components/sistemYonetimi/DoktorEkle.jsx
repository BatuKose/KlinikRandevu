import { useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from '../hastaKayit/useDoktorVeServisListesi';
import { useUzmanlikBranslari } from './useUzmanlikBranslari';
import Bildirim from '../bildirim/Bildirim';

const BOS_FORM = {
  doktorAd: '',
  doktorTc: '',
  tescilNo: '',
  uzmanlikKodu: '',
  servisNo: '',
  email: '',
};

export default function DoktorEkle() {
  const { servisler, yukleniyor: servisYukleniyor } = useDoktorVeServisListesi();
  const { branslar, yukleniyor: bransYukleniyor, hata: bransHata } = useUzmanlikBranslari();
  const [form, setForm] = useState(BOS_FORM);
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
      const { data } = await muayeneService.doktorEkle({
        doktorAd: form.doktorAd.trim(),
        doktorTc: Number(form.doktorTc),
        tescilNo: Number(form.tescilNo),
        uzmanlikKodu: Number(form.uzmanlikKodu),
        servisNo: form.servisNo ? Number(form.servisNo) : null,
        email: form.email.trim() || null,
      });
      setMesaj({ tip: 'basari', metin: `Doktor eklendi (Doktor No: ${data?.data?.no ?? '-'}).` });
      setForm(BOS_FORM);
    } catch (err) {
      setMesaj({ tip: 'hata', metin: getApiErrorMessage(err, 'Doktor eklenemedi') });
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="kullanici-ekle-kart">
      <h3>Doktor Ekle</h3>
      <form onSubmit={handleSubmit} className="kullanici-form">
        <div className="form-alani">
          <label>Ad Soyad <span className="zorunlu">*</span></label>
          <input
            name="doktorAd"
            value={form.doktorAd}
            onChange={handleChange}
            required
            maxLength={100}
            placeholder="Örn: AYŞE DEMİR"
          />
        </div>
        <div className="form-satir">
          <div className="form-alani">
            <label>TC Kimlik No <span className="zorunlu">*</span></label>
            <input
              name="doktorTc"
              value={form.doktorTc}
              onChange={(e) => setForm((prev) => ({ ...prev, doktorTc: e.target.value.replace(/\D/g, '') }))}
              required
              maxLength={11}
              minLength={11}
              placeholder="11 haneli TC"
            />
          </div>
          <div className="form-alani">
            <label>Tescil No <span className="zorunlu">*</span></label>
            <input
              name="tescilNo"
              type="number"
              min="1"
              value={form.tescilNo}
              onChange={handleChange}
              required
              placeholder="Tescil numarası"
            />
          </div>
        </div>
        <div className="form-satir">
          <div className="form-alani">
            <label>Uzmanlık Branşı <span className="zorunlu">*</span></label>
            <select name="uzmanlikKodu" value={form.uzmanlikKodu} onChange={handleChange} required disabled={bransYukleniyor}>
              <option value="">{bransYukleniyor ? 'Yükleniyor...' : 'Seçiniz'}</option>
              {branslar.map((b) => (
                <option key={b.kod} value={b.kod}>{b.ad}</option>
              ))}
            </select>
          </div>
          <div className="form-alani">
            <label>Poliklinik (Servis)</label>
            <select name="servisNo" value={form.servisNo} onChange={handleChange} disabled={servisYukleniyor}>
              <option value="">{servisYukleniyor ? 'Yükleniyor...' : 'Atanmadı'}</option>
              {servisler.map((s) => (
                <option key={s.servisNo} value={s.servisNo}>{s.servisAdi}</option>
              ))}
            </select>
          </div>
        </div>
        <div className="form-alani">
          <label>E-posta</label>
          <input name="email" type="email" value={form.email} onChange={handleChange} placeholder="doktor@mail.com (günlük program maili için)" />
        </div>
        <Bildirim mesaj={bransHata} />
        <Bildirim
          mesaj={mesaj.metin}
          tip={mesaj.tip === 'basari' ? 'basari' : 'hata'}
          onKapat={() => setMesaj({ tip: '', metin: '' })}
        />
        <button type="submit" className="kaydet-btn" disabled={yukleniyor}>
          {yukleniyor ? 'Ekleniyor...' : 'Doktor Ekle'}
        </button>
      </form>
    </div>
  );
}
