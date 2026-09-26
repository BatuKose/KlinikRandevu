import { useCallback, useEffect, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';
import { useDoktorVeServisListesi } from '../hastaKayit/useDoktorVeServisListesi';
import Bildirim from '../bildirim/Bildirim';
import '../hastaKayit/HastaKayitModal.css';
import './CalismaPlanlari.css';

// Backend DayOfWeek enum'u sayı olarak dönüyor (0 = Pazar). Liste Pazartesi'den başlıyor.
const GUNLER = [
  { deger: 1, ad: 'Pazartesi', kisa: 'Pzt' },
  { deger: 2, ad: 'Salı', kisa: 'Sal' },
  { deger: 3, ad: 'Çarşamba', kisa: 'Çar' },
  { deger: 4, ad: 'Perşembe', kisa: 'Per' },
  { deger: 5, ad: 'Cuma', kisa: 'Cum' },
  { deger: 6, ad: 'Cumartesi', kisa: 'Cmt' },
  { deger: 0, ad: 'Pazar', kisa: 'Paz' },
];
const gunAdi = (deger) => GUNLER.find((g) => g.deger === deger)?.ad ?? deger;
const saat = (ts) => (ts || '').slice(0, 5);

function GunSecici({ secili, onDegis, haric }) {
  const degistir = (deger) =>
    onDegis(secili.includes(deger) ? secili.filter((g) => g !== deger) : [...secili, deger]);
  return (
    <div className="cp-gun-secici">
      {GUNLER.filter((g) => g.deger !== haric).map((g) => (
        <button
          key={g.deger}
          type="button"
          className={`cp-gun-chip${secili.includes(g.deger) ? ' cp-gun-chip-secili' : ''}`}
          onClick={() => degistir(g.deger)}
        >
          {g.kisa}
        </button>
      ))}
    </div>
  );
}

// Her gün için ayrı istek atılıyor; biri hata verirse diğerleri yine kaydedilir.
async function gunlereUygula(gunler, istek) {
  const hatalar = [];
  let basarili = 0;
  for (const gun of gunler) {
    try {
      await istek(gun);
      basarili++;
    } catch (err) {
      hatalar.push({ gun, mesaj: getApiErrorMessage(err, 'Kaydedilemedi') });
    }
  }
  return { basarili, hatalar };
}

function hataMetni(hatalar) {
  return hatalar.map((h) => `${gunAdi(h.gun)}: ${h.mesaj}`).join(' • ');
}

function PlanModal({ doktorlar, servisler, onKapat, onKaydedildi }) {
  const [form, setForm] = useState({
    doktorNo: '',
    polNo: '',
    baslangic: '08:00',
    bitis: '12:00',
    sure: '15',
    yesilAlan: false,
  });
  const [gunler, setGunler] = useState([]);
  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const alan = (ad, deger) => setForm((f) => ({ ...f, [ad]: deger }));

  const dakika = (hhmm) => {
    const [s, d] = hhmm.split(':').map(Number);
    return s * 60 + d;
  };
  const slotSayisi =
    Number(form.sure) > 0 && form.bitis > form.baslangic
      ? Math.floor((dakika(form.bitis) - dakika(form.baslangic)) / Number(form.sure))
      : 0;

  const submit = async (e) => {
    e.preventDefault();
    if (gunler.length === 0) {
      setHata('En az bir gün seçiniz');
      return;
    }
    setHata('');
    setKaydediliyor(true);
    const { basarili, hatalar } = await gunlereUygula(gunler, (gun) =>
      muayeneService.calismaPlaniOlustur({
        doktorNo: Number(form.doktorNo),
        polNo: Number(form.polNo),
        gunAdi: gun,
        baslangicSaati: `${form.baslangic}:00`,
        bitisSaati: `${form.bitis}:00`,
        randevuSuresiDk: Number(form.sure),
        yesilAlan: form.yesilAlan,
      })
    );
    setKaydediliyor(false);
    if (hatalar.length === 0) {
      onKaydedildi(`${basarili} günlük çalışma planı oluşturuldu`);
      return;
    }
    // Kaydedilen günleri seçimden çıkar; kullanıcı sadece hatalıları düzeltip tekrar denesin.
    setGunler(hatalar.map((h) => h.gun));
    setHata(hataMetni(hatalar));
    if (basarili > 0) onKaydedildi(null);
  };

  return (
    <div className="hkm-overlay" onClick={onKapat}>
      <div className="hkm-kart" onClick={(e) => e.stopPropagation()}>
        <div className="hkm-baslik">
          <div>
            <h2>Yeni Çalışma Planı</h2>
            <p>Seçilen her gün için ayrı plan oluşturulur</p>
          </div>
          <button className="hkm-kapat-btn" onClick={onKapat} type="button">✕</button>
        </div>
        <form onSubmit={submit}>
          <div className="hkm-satir-2">
            <label className="hkm-alan">
              <span>Doktor</span>
              <select value={form.doktorNo} onChange={(e) => alan('doktorNo', e.target.value)} required>
                <option value="">Seçiniz</option>
                {doktorlar.map((d) => (
                  <option key={d.doktorNo} value={d.doktorNo}>{d.doktorAd}</option>
                ))}
              </select>
            </label>
            <label className="hkm-alan">
              <span>Poliklinik</span>
              <select value={form.polNo} onChange={(e) => alan('polNo', e.target.value)} required>
                <option value="">Seçiniz</option>
                {servisler.map((s) => (
                  <option key={s.servisNo} value={s.servisNo}>{s.servisAdi}</option>
                ))}
              </select>
            </label>
          </div>

          <div className="hkm-alan">
            <span>Çalışma Günleri</span>
            <GunSecici secili={gunler} onDegis={setGunler} />
          </div>

          <div className="hkm-satir-2">
            <label className="hkm-alan">
              <span>Başlangıç</span>
              <input type="time" value={form.baslangic} onChange={(e) => alan('baslangic', e.target.value)} required />
            </label>
            <label className="hkm-alan">
              <span>Bitiş</span>
              <input type="time" value={form.bitis} onChange={(e) => alan('bitis', e.target.value)} required />
            </label>
          </div>

          <label className="hkm-alan">
            <span>Randevu Süresi (Dakika)</span>
            <input type="number" min="1" value={form.sure} onChange={(e) => alan('sure', e.target.value)} required />
          </label>

          <p className="cp-onizleme">
            {slotSayisi > 0
              ? `Her gün ${saat(form.baslangic)}–${saat(form.bitis)} arası ${slotSayisi} randevu slotu oluşacak`
              : 'Saat aralığı ve süreyi kontrol ediniz'}
          </p>

          <label className="hkm-alan hkm-checkbox">
            <input type="checkbox" checked={form.yesilAlan} onChange={(e) => alan('yesilAlan', e.target.checked)} />
            <span>Yeşil alan zorunlu</span>
          </label>

          <Bildirim mesaj={hata} onKapat={() => setHata('')} />

          <div className="hkm-footer">
            <button type="button" className="hkm-btn" onClick={onKapat} disabled={kaydediliyor}>Vazgeç</button>
            <button type="submit" className="hkm-btn hkm-btn-birincil" disabled={kaydediliyor}>
              {kaydediliyor ? 'Kaydediliyor...' : 'Planı Oluştur'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function KopyalaModal({ plan, onKapat, onKaydedildi }) {
  const [gunler, setGunler] = useState([]);
  const [hata, setHata] = useState('');
  const [kaydediliyor, setKaydediliyor] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    if (gunler.length === 0) {
      setHata('En az bir gün seçiniz');
      return;
    }
    setHata('');
    setKaydediliyor(true);
    const { basarili, hatalar } = await gunlereUygula(gunler, (gun) =>
      muayeneService.calismaPlaniKopyala(plan.id, gun)
    );
    setKaydediliyor(false);
    if (hatalar.length === 0) {
      onKaydedildi(`Plan ${basarili} güne kopyalandı`);
      return;
    }
    setGunler(hatalar.map((h) => h.gun));
    setHata(hataMetni(hatalar));
    if (basarili > 0) onKaydedildi(null);
  };

  return (
    <div className="hkm-overlay" onClick={onKapat}>
      <div className="hkm-kart" onClick={(e) => e.stopPropagation()}>
        <div className="hkm-baslik">
          <div>
            <h2>Planı Kopyala</h2>
            <p>
              {plan.doktorAd} · {gunAdi(plan.gunAdi)} {saat(plan.baslangicSaati)}–{saat(plan.bitisSaati)}
            </p>
          </div>
          <button className="hkm-kapat-btn" onClick={onKapat} type="button">✕</button>
        </div>
        <form onSubmit={submit}>
          <div className="hkm-alan">
            <span>Kopyalanacak Günler</span>
            <GunSecici secili={gunler} onDegis={setGunler} haric={plan.gunAdi} />
          </div>
          <Bildirim mesaj={hata} onKapat={() => setHata('')} />
          <div className="hkm-footer">
            <button type="button" className="hkm-btn" onClick={onKapat} disabled={kaydediliyor}>Vazgeç</button>
            <button type="submit" className="hkm-btn hkm-btn-birincil" disabled={kaydediliyor}>
              {kaydediliyor ? 'Kopyalanıyor...' : 'Kopyala'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default function CalismaPlanlari() {
  const { doktorlar, servisler, hata: listeHata } = useDoktorVeServisListesi();
  const [filtre, setFiltre] = useState({ doktorNo: '', polNo: '', pasifleriGoster: false });
  const [planlar, setPlanlar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [hata, setHata] = useState('');
  const [basari, setBasari] = useState('');
  const [yeniAcik, setYeniAcik] = useState(false);
  const [kopyalanan, setKopyalanan] = useState(null);
  const [pasifeAlinan, setPasifeAlinan] = useState(null);

  const yukle = useCallback(async () => {
    setYukleniyor(true);
    setHata('');
    try {
      const { data } = await muayeneService.calismaPlanlariGetir({
        doktorNo: filtre.doktorNo || undefined,
        polNo: filtre.polNo || undefined,
        sadeceAktif: !filtre.pasifleriGoster,
      });
      setPlanlar(data?.data || []);
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Çalışma planları alınamadı'));
    } finally {
      setYukleniyor(false);
    }
  }, [filtre]);

  useEffect(() => {
    yukle();
  }, [yukle]);

  const kaydedildi = (mesaj) => {
    if (mesaj) {
      setYeniAcik(false);
      setKopyalanan(null);
      setBasari(mesaj);
    }
    yukle();
  };

  const pasifeAl = async (plan) => {
    if (!window.confirm(`${plan.doktorAd} - ${gunAdi(plan.gunAdi)} ${saat(plan.baslangicSaati)}–${saat(plan.bitisSaati)} planı pasife alınsın mı?`)) return;
    setPasifeAlinan(plan.id);
    setHata('');
    try {
      await muayeneService.calismaPlaniPasifeAl(plan.id);
      setBasari('Çalışma planı pasife alındı');
      yukle();
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Çalışma planı pasife alınamadı'));
    } finally {
      setPasifeAlinan(null);
    }
  };

  return (
    <div className="rd-panel">
      <div className="cp-ust">
        <div className="rd-filtre-bar">
          <label className="rd-alan">
            <span>Doktor</span>
            <select value={filtre.doktorNo} onChange={(e) => setFiltre((f) => ({ ...f, doktorNo: e.target.value }))}>
              <option value="">Tümü</option>
              {doktorlar.map((d) => (
                <option key={d.doktorNo} value={d.doktorNo}>{d.doktorAd}</option>
              ))}
            </select>
          </label>
          <label className="rd-alan">
            <span>Poliklinik</span>
            <select value={filtre.polNo} onChange={(e) => setFiltre((f) => ({ ...f, polNo: e.target.value }))}>
              <option value="">Tümü</option>
              {servisler.map((s) => (
                <option key={s.servisNo} value={s.servisNo}>{s.servisAdi}</option>
              ))}
            </select>
          </label>
          <label className="cp-checkbox">
            <input
              type="checkbox"
              checked={filtre.pasifleriGoster}
              onChange={(e) => setFiltre((f) => ({ ...f, pasifleriGoster: e.target.checked }))}
            />
            Pasif planları göster
          </label>
        </div>
        <button type="button" className="rd-btn rd-btn-birincil" onClick={() => setYeniAcik(true)}>
          + Yeni Çalışma Planı
        </button>
      </div>

      <Bildirim mesaj={listeHata} />
      <Bildirim mesaj={hata} onKapat={() => setHata('')} />
      <Bildirim mesaj={basari} tip="basari" onKapat={() => setBasari('')} />

      {yukleniyor && <p className="rd-bos-metin">Yükleniyor...</p>}
      {!yukleniyor && !hata && planlar.length === 0 && (
        <p className="rd-bos-metin">
          Kayıtlı çalışma planı yok. Çalışma planı olmayan doktora randevu verilemez.
        </p>
      )}
      {!yukleniyor && planlar.length > 0 && (
        <div className="rd-tablo-kapsayici">
          <table className="rd-tablo">
            <thead>
              <tr>
                <th>Doktor</th>
                <th>Poliklinik</th>
                <th>Gün</th>
                <th>Saat</th>
                <th>Randevu Süresi</th>
                <th>Slot</th>
                <th>Durum</th>
                <th>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {planlar.map((p) => (
                <tr key={p.id} className={p.isActive ? '' : 'cp-pasif-satir'}>
                  <td>{p.doktorAd ?? `#${p.doktorNo}`}</td>
                  <td>{p.polAdi ?? `#${p.polNo}`}</td>
                  <td>{gunAdi(p.gunAdi)}</td>
                  <td>{saat(p.baslangicSaati)} – {saat(p.bitisSaati)}</td>
                  <td>{p.randevuSuresiDk} dk</td>
                  <td>{p.slotSayisi}</td>
                  <td>
                    <span className={`rd-durum-rozet ${p.isActive ? 'cp-durum-aktif' : 'rd-durum-gecti'}`}>
                      {p.isActive ? 'Aktif' : 'Pasif'}
                    </span>
                    {p.yesilAlanZorunlu && <span className="rd-durum-rozet cp-durum-yesil">Yeşil alan</span>}
                  </td>
                  <td>
                    {p.isActive && (
                      <div className="cp-islemler">
                        <button type="button" className="cp-mini-btn" onClick={() => setKopyalanan(p)}>
                          Kopyala
                        </button>
                        <button
                          type="button"
                          className="rd-mini-btn"
                          onClick={() => pasifeAl(p)}
                          disabled={pasifeAlinan === p.id}
                        >
                          {pasifeAlinan === p.id ? 'Alınıyor...' : 'Pasife Al'}
                        </button>
                      </div>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {yeniAcik && (
        <PlanModal
          doktorlar={doktorlar}
          servisler={servisler}
          onKapat={() => setYeniAcik(false)}
          onKaydedildi={kaydedildi}
        />
      )}
      {kopyalanan && (
        <KopyalaModal plan={kopyalanan} onKapat={() => setKopyalanan(null)} onKaydedildi={kaydedildi} />
      )}
    </div>
  );
}
