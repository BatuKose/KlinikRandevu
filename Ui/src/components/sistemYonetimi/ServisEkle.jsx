import { useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from '../hastaKayit/useDoktorVeServisListesi';
import { useUzmanlikBranslari } from './useUzmanlikBranslari';
import Bildirim from '../bildirim/Bildirim';

const BOS_FORM = {
  name: '',
  uzmanlikKodu: '',
  doktorNo: '',
  aciklama: '',
  katNo: '',
  odaNo: '',
  maxRandevuSuresi: '',
  gunlukMaksRandevuSayisi: '',
  telefon: '',
  onlineRandevuAktif: false,
};

const sayiVeyaNull = (deger) => (deger === '' ? null : Number(deger));

export default function ServisEkle() {
  const { doktorlar, yukleniyor: doktorYukleniyor } = useDoktorVeServisListesi();
  const { branslar, yukleniyor: bransYukleniyor, hata: bransHata } = useUzmanlikBranslari();
  const [form, setForm] = useState(BOS_FORM);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [mesaj, setMesaj] = useState({ tip: '', metin: '' });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm((prev) => ({ ...prev, [name]: type === 'checkbox' ? checked : value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setMesaj({ tip: '', metin: '' });
    setYukleniyor(true);
    try {
      const { data } = await muayeneService.servisEkle({
        name: form.name.trim(),
        uzmanlikKodu: Number(form.uzmanlikKodu),
        doktorNo: form.doktorNo ? Number(form.doktorNo) : null,
        aciklama: form.aciklama.trim() || null,
        katNo: sayiVeyaNull(form.katNo),
        odaNo: form.odaNo.trim() || null,
        maxRandevuSuresi: sayiVeyaNull(form.maxRandevuSuresi),
        gunlukMaksRandevuSayisi: sayiVeyaNull(form.gunlukMaksRandevuSayisi),
        telefon: form.telefon.trim() || null,
        onlineRandevuAktif: form.onlineRandevuAktif,
      });
      setMesaj({ tip: 'basari', metin: `Poliklinik eklendi (Servis No: ${data?.data?.no ?? '-'}).` });
      setForm(BOS_FORM);
    } catch (err) {
      setMesaj({ tip: 'hata', metin: getApiErrorMessage(err, 'Poliklinik eklenemedi') });
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="kullanici-ekle-kart">
      <h3>Servis (Poliklinik) Ekle</h3>
      <form onSubmit={handleSubmit} className="kullanici-form">
        <div className="form-alani">
          <label>Poliklinik Adı <span className="zorunlu">*</span></label>
          <input
            name="name"
            value={form.name}
            onChange={handleChange}
            required
            maxLength={100}
            placeholder="Örn: KARDİYOLOJİ AYŞE DEMİR"
          />
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
            <label>Sorumlu Doktor</label>
            <select name="doktorNo" value={form.doktorNo} onChange={handleChange} disabled={doktorYukleniyor}>
              <option value="">{doktorYukleniyor ? 'Yükleniyor...' : 'Atanmadı'}</option>
              {doktorlar.map((d) => (
                <option key={d.doktorNo} value={d.doktorNo}>{d.doktorAd}</option>
              ))}
            </select>
          </div>
        </div>
        <div className="form-alani">
          <label>Açıklama</label>
          <textarea name="aciklama" value={form.aciklama} onChange={handleChange} rows={2} maxLength={500} placeholder="Açıklama giriniz" />
        </div>
        <div className="form-satir">
          <div className="form-alani">
            <label>Kat No</label>
            <input name="katNo" type="number" value={form.katNo} onChange={handleChange} placeholder="Kat" />
          </div>
          <div className="form-alani">
            <label>Oda</label>
            <input name="odaNo" value={form.odaNo} onChange={handleChange} maxLength={20} placeholder="Oda / bölüm bilgisi" />
          </div>
        </div>
        <div className="form-satir">
          <div className="form-alani">
            <label>Maks. Randevu Süresi (dk)</label>
            <input name="maxRandevuSuresi" type="number" min="1" value={form.maxRandevuSuresi} onChange={handleChange} placeholder="Örn: 15" />
          </div>
          <div className="form-alani">
            <label>Günlük Maks. Randevu Sayısı</label>
            <input name="gunlukMaksRandevuSayisi" type="number" min="1" value={form.gunlukMaksRandevuSayisi} onChange={handleChange} placeholder="Örn: 30" />
          </div>
        </div>
        <div className="form-alani">
          <label>Telefon</label>
          <input name="telefon" value={form.telefon} onChange={handleChange} maxLength={20} placeholder="Dahili / telefon" />
        </div>
        <label className="form-onay">
          <input name="onlineRandevuAktif" type="checkbox" checked={form.onlineRandevuAktif} onChange={handleChange} />
          Online randevuya açık
        </label>
        <Bildirim mesaj={bransHata} />
        <Bildirim
          mesaj={mesaj.metin}
          tip={mesaj.tip === 'basari' ? 'basari' : 'hata'}
          onKapat={() => setMesaj({ tip: '', metin: '' })}
        />
        <button type="submit" className="kaydet-btn" disabled={yukleniyor}>
          {yukleniyor ? 'Ekleniyor...' : 'Poliklinik Ekle'}
        </button>
      </form>
    </div>
  );
}
