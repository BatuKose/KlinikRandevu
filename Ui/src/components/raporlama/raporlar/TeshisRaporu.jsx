import { useMemo, useState } from 'react';
import { raporlamaService } from '../../../services/raporlamaService';
import Ikon from '../../ui/Ikon';
import { IstatistikKart } from '../../ui/Ortak';
import YiginGrafik from '../grafik/YiginGrafik';
import SiralamaGrafik from '../grafik/SiralamaGrafik';
import SeriLejanti from '../grafik/SeriLejanti';
import { DIGER_RENGI, TEK_SERI_RENGI, sayiFormat } from '../grafik/grafikYardimci';
import { aylariUret, ayAnahtari, csvIndir, serileriKatla, useTarihliRapor } from '../raporYardimci';
import TarihAraligiFiltresi, { RaporDurumu } from '../TarihAraligiFiltresi';

const AYLIK_GORUNUMLER = [
  { id: 'toplam', etiket: 'Toplam' },
  { id: 'brans', etiket: 'Branşa Göre' },
];

const LISTE_ADETLERI = [
  { id: 10, etiket: 'İlk 10' },
  { id: 20, etiket: 'İlk 20' },
  { id: 0, etiket: 'Tümü' },
];

const yuzde = (pay, toplam) => (toplam ? `%${((pay / toplam) * 100).toFixed(1)}` : '%0');

// Bir teşhisin branşlara dağılımı: tek satırlık yığılmış çubuk (branş renkleri raporun geneliyle aynı).
function BransPayCubugu({ branslar, toplam, renkHaritasi }) {
  return (
    <span className="ts-pay" aria-hidden="true">
      {branslar.map((b) => (
        <span
          key={b.ad}
          className="ts-pay-dilim"
          style={{ flexGrow: b.sayi, background: renkHaritasi.get(b.ad) ?? DIGER_RENGI }}
          title={`${b.ad}: ${sayiFormat(b.sayi)} (${yuzde(b.sayi, toplam)})`}
        />
      ))}
    </span>
  );
}

export default function TeshisRaporu() {
  const veri = useTarihliRapor(raporlamaService.teshisRaporu);
  const { baslangic, bitis, kayitlar, yukleniyor, aralikHatasi } = veri;
  const [aylikGorunum, setAylikGorunum] = useState('toplam');
  const [gizliSeriler, setGizliSeriler] = useState(() => new Set());
  const [listeAdedi, setListeAdedi] = useState(10);
  const [arama, setArama] = useState('');
  const [seciliKod, setSeciliKod] = useState(null);

  // Ham satırlar (ay, teşhis, branş, sayı) → teşhis bazlı, branş bazlı ve aylık özetler.
  const rapor = useMemo(() => {
    if (aralikHatasi) return null;
    const aylar = aylariUret(baslangic, bitis);
    const ayIndeks = new Map(aylar.map((a, i) => [a.anahtar, i]));
    const teshisler = new Map();
    const branslar = new Map();

    for (const k of kayitlar) {
      const i = ayIndeks.get(ayAnahtari(k.tarih));
      if (i === undefined) continue;
      const sayi = k.teshisSayisi ?? 0;
      const kod = k.teshisKod || k.teshisAd || 'Kodsuz';
      const bransAd = k.uzmanlikAdi || 'Belirtilmemiş';

      if (!teshisler.has(kod)) {
        teshisler.set(kod, { kod, ad: k.teshisAd || kod, toplam: 0, aylik: new Array(aylar.length).fill(0), branslar: new Map() });
      }
      const t = teshisler.get(kod);
      t.toplam += sayi;
      t.aylik[i] += sayi;
      t.branslar.set(bransAd, (t.branslar.get(bransAd) ?? 0) + sayi);

      if (!branslar.has(bransAd)) branslar.set(bransAd, { ad: bransAd, toplam: 0, degerler: new Array(aylar.length).fill(0) });
      const b = branslar.get(bransAd);
      b.toplam += sayi;
      b.degerler[i] += sayi;
    }

    const tumBranslar = [...branslar.values()].sort((a, b) => b.toplam - a.toplam || a.ad.localeCompare(b.ad, 'tr'));
    const { seriler, renkHaritasi } = serileriKatla(tumBranslar, aylar.length);

    const tumTeshisler = [...teshisler.values()]
      .map((t) => {
        const bransListesi = [...t.branslar.entries()]
          .map(([ad, sayi]) => ({ ad, sayi }))
          .sort((a, b) => b.sayi - a.sayi || a.ad.localeCompare(b.ad, 'tr'));
        return { ...t, bransListesi, enCokBrans: bransListesi[0] };
      })
      .sort((a, b) => b.toplam - a.toplam || a.kod.localeCompare(b.kod, 'tr'));

    const aylikToplam = aylar.map((_, i) => tumBranslar.reduce((t, b) => t + b.degerler[i], 0));
    const genelToplam = aylikToplam.reduce((t, v) => t + v, 0);
    const enYogunAyI = aylikToplam.indexOf(Math.max(...aylikToplam));

    return { aylar, tumBranslar, tumTeshisler, seriler, renkHaritasi, aylikToplam, genelToplam, enYogunAyI };
  }, [kayitlar, baslangic, bitis, aralikHatasi]);

  const listelenen = useMemo(() => {
    if (!rapor) return [];
    const metin = arama.trim().toLocaleLowerCase('tr-TR');
    const filtreli = metin
      ? rapor.tumTeshisler.filter((t) => `${t.kod} ${t.ad}`.toLocaleLowerCase('tr-TR').includes(metin))
      : rapor.tumTeshisler;
    return listeAdedi ? filtreli.slice(0, listeAdedi) : filtreli;
  }, [rapor, arama, listeAdedi]);

  // Seçim yoksa (veya seçilen teşhis yeni aralıkta yoksa) en sık teşhis gösterilir.
  const secili = rapor?.tumTeshisler.find((t) => t.kod === seciliKod) ?? rapor?.tumTeshisler[0];

  const csv = () => {
    const satirlar = [['Teşhis Kodu', 'Teşhis Adı', 'Uzmanlık Dalı', 'Teşhis Sayısı', 'Teşhis İçindeki Payı']];
    for (const t of rapor.tumTeshisler) {
      for (const b of t.bransListesi) satirlar.push([t.kod, t.ad, b.ad, b.sayi, yuzde(b.sayi, t.toplam)]);
    }
    csvIndir(`teshis-raporu_${baslangic}_${bitis}.csv`, satirlar);
  };

  const veriVar = rapor && rapor.genelToplam > 0;
  const aylikSeriler =
    aylikGorunum === 'toplam'
      ? [{ ad: 'Teşhis', renk: TEK_SERI_RENGI, degerler: rapor?.aylikToplam ?? [] }]
      : (rapor?.seriler.filter((s) => !gizliSeriler.has(s.ad)) ?? []);

  return (
    <>
      <TarihAraligiFiltresi rapor={veri} onCsv={csv} csvAktif={veriVar} />
      <RaporDurumu rapor={veri} veriVar={veriVar} bosBaslik="Bu aralıkta girilmiş teşhis yok" />

      {veriVar && (
        <div className={`rp-sonuc${yukleniyor ? ' rp-sonuc-yenileniyor' : ''}`}>
          <div className="ui-istatistik-grid rp-ozet">
            <IstatistikKart ikon="dosya" ton="bilgi" deger={sayiFormat(rapor.genelToplam)} etiket="Girilen teşhis" />
            <IstatistikKart ikon="liste" deger={sayiFormat(rapor.tumTeshisler.length)} etiket="Farklı teşhis kodu" />
            <IstatistikKart
              ikon="trend"
              ton="basari"
              deger={rapor.tumTeshisler[0].kod}
              etiket={`En sık teşhis · ${sayiFormat(rapor.tumTeshisler[0].toplam)} kez`}
            />
            <IstatistikKart
              ikon="stetoskop"
              ton="uyari"
              deger={rapor.tumBranslar[0].ad}
              etiket={`En çok teşhis giren branş · ${sayiFormat(rapor.tumBranslar[0].toplam)}`}
            />
          </div>

          <section className="ui-kart">
            <div className="ui-kart-baslik rp-grafik-baslik">
              <h2>
                <Ikon ad="grafik" /> Aylara göre girilen teşhis sayısı
              </h2>
              <div className="ui-segment" role="tablist" aria-label="Aylık grafik görünümü">
                {AYLIK_GORUNUMLER.map((g) => (
                  <button
                    type="button"
                    role="tab"
                    aria-selected={aylikGorunum === g.id}
                    key={g.id}
                    className={aylikGorunum === g.id ? 'ui-segment-aktif' : ''}
                    onClick={() => setAylikGorunum(g.id)}
                  >
                    {g.etiket}
                  </button>
                ))}
              </div>
            </div>
            <div className="ui-kart-govde">
              {aylikGorunum === 'brans' && (
                <SeriLejanti seriler={rapor.seriler} gizliSeriler={gizliSeriler} onDegis={setGizliSeriler} />
              )}
              <YiginGrafik
                etiketler={rapor.aylar.map((a) => a.kisa)}
                uzunEtiketler={rapor.aylar.map((a) => a.uzun)}
                seriler={aylikSeriler}
                ariaEtiket="Aylara göre girilen teşhis sayısı"
              />
              <p className="ts-not">
                En yoğun ay <strong>{rapor.aylar[rapor.enYogunAyI].uzun}</strong> ·{' '}
                {sayiFormat(rapor.aylikToplam[rapor.enYogunAyI])} teşhis
              </p>
            </div>
          </section>

          <div className="ts-izgara">
            <section className="ui-kart ts-liste-kart">
              <div className="ui-kart-baslik rp-grafik-baslik">
                <h2>
                  <Ikon ad="liste" /> En sık girilen teşhisler
                </h2>
                <div className="ui-segment" role="group" aria-label="Listelenecek teşhis sayısı">
                  {LISTE_ADETLERI.map((a) => (
                    <button
                      type="button"
                      key={a.id}
                      className={listeAdedi === a.id ? 'ui-segment-aktif' : ''}
                      onClick={() => setListeAdedi(a.id)}
                    >
                      {a.etiket}
                    </button>
                  ))}
                </div>
              </div>
              <div className="ts-liste-arac">
                <label className="rp-arama">
                  <Ikon ad="ara" />
                  <input
                    type="search"
                    className="ui-input"
                    placeholder="Teşhis kodu veya adı ara"
                    value={arama}
                    onChange={(e) => setArama(e.target.value)}
                  />
                </label>
              </div>
              {listelenen.length === 0 ? (
                <p className="rp-katalog-bos">Eşleşen teşhis yok.</p>
              ) : (
                <ol className="ts-liste">
                  {listelenen.map((t) => {
                    const sira = rapor.tumTeshisler.indexOf(t) + 1;
                    const aktif = t.kod === secili?.kod;
                    return (
                      <li key={t.kod}>
                        <button
                          type="button"
                          className={`ts-satir${aktif ? ' ts-satir-aktif' : ''}`}
                          aria-pressed={aktif}
                          onClick={() => setSeciliKod(t.kod)}
                        >
                          <span className="ts-sira">{sira}</span>
                          <span className="ts-govde">
                            <span className="ts-ust">
                              <span className="ts-kod">{t.kod}</span>
                              <span className="ts-ad" title={t.ad}>{t.ad}</span>
                              <span className="ts-sayi">{sayiFormat(t.toplam)}</span>
                            </span>
                            <BransPayCubugu branslar={t.bransListesi} toplam={t.toplam} renkHaritasi={rapor.renkHaritasi} />
                            <span className="ts-alt">
                              En çok <strong>{t.enCokBrans.ad}</strong> · {yuzde(t.enCokBrans.sayi, t.toplam)}
                              {t.bransListesi.length > 1 && ` · ${t.bransListesi.length} branş`}
                            </span>
                          </span>
                        </button>
                      </li>
                    );
                  })}
                </ol>
              )}
            </section>

            {secili && (
              <section className="ui-kart ts-detay">
                <div className="ui-kart-baslik">
                  <h2>
                    <Ikon ad="bilgi" /> Teşhis detayı
                  </h2>
                </div>
                <div className="ui-kart-govde ts-detay-govde">
                  <div className="ts-detay-baslik">
                    <span className="ts-kod">{secili.kod}</span>
                    <h3>{secili.ad}</h3>
                    <p>
                      Toplam <strong>{sayiFormat(secili.toplam)}</strong> kez girildi (tüm teşhisler içindeki payı{' '}
                      {yuzde(secili.toplam, rapor.genelToplam)}). En çok <strong>{secili.enCokBrans.ad}</strong> branşında (
                      {yuzde(secili.enCokBrans.sayi, secili.toplam)}).
                    </p>
                  </div>

                  <h4 className="ts-alt-baslik">Branşlara göre dağılım</h4>
                  <SiralamaGrafik satirlar={secili.bransListesi.map((b) => ({ ad: b.ad, deger: b.sayi }))} />

                  <h4 className="ts-alt-baslik">Aylara göre</h4>
                  <YiginGrafik
                    etiketler={rapor.aylar.map((a) => a.kisa)}
                    uzunEtiketler={rapor.aylar.map((a) => a.uzun)}
                    seriler={[{ ad: secili.kod, renk: TEK_SERI_RENGI, degerler: secili.aylik }]}
                    yukseklik={200}
                    ariaEtiket={`${secili.kod} teşhisinin aylara göre sayısı`}
                  />
                </div>
              </section>
            )}
          </div>
        </div>
      )}
    </>
  );
}
