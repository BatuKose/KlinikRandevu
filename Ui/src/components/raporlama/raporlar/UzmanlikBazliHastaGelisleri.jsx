import { useMemo, useState } from 'react';
import { raporlamaService } from '../../../services/raporlamaService';
import Ikon from '../../ui/Ikon';
import { IstatistikKart } from '../../ui/Ortak';
import CizgiGrafik from '../grafik/CizgiGrafik';
import YiginGrafik from '../grafik/YiginGrafik';
import SiralamaGrafik from '../grafik/SiralamaGrafik';
import SeriLejanti from '../grafik/SeriLejanti';
import { DIGER_RENGI, sayiFormat } from '../grafik/grafikYardimci';
import { aylariUret, ayAnahtari, csvIndir, serileriKatla, useTarihliRapor } from '../raporYardimci';
import TarihAraligiFiltresi, { RaporDurumu } from '../TarihAraligiFiltresi';

const GORUNUMLER = [
  { id: 'egilim', etiket: 'Eğilim', ikon: 'trend' },
  { id: 'dagilim', etiket: 'Aylık Dağılım', ikon: 'grafik' },
  { id: 'siralama', etiket: 'Branş Sıralaması', ikon: 'liste' },
];

export default function UzmanlikBazliHastaGelisleri() {
  const veri = useTarihliRapor(raporlamaService.uzmanlikBazliHastaGelisleri);
  const { baslangic, bitis, kayitlar, yukleniyor, aralikHatasi } = veri;
  const [gorunum, setGorunum] = useState('egilim');
  const [gizliSeriler, setGizliSeriler] = useState(() => new Set());

  // Ham satırları (branş, ay, sayı) grafik ve tablonun ortak kullandığı pivot yapıya çevirir.
  const rapor = useMemo(() => {
    if (aralikHatasi) return null;
    const aylar = aylariUret(baslangic, bitis);
    const ayIndeks = new Map(aylar.map((a, i) => [a.anahtar, i]));
    const branslar = new Map();
    for (const k of kayitlar) {
      const i = ayIndeks.get(ayAnahtari(k.tarih));
      if (i === undefined) continue;
      const ad = k.uzmanlikAdi || 'Belirtilmemiş';
      if (!branslar.has(ad)) branslar.set(ad, new Array(aylar.length).fill(0));
      branslar.get(ad)[i] += k.muayeneSayisi ?? 0;
    }
    const tumBranslar = [...branslar.entries()]
      .map(([ad, degerler]) => ({ ad, degerler, toplam: degerler.reduce((t, v) => t + v, 0) }))
      .sort((a, b) => b.toplam - a.toplam || a.ad.localeCompare(b.ad, 'tr'));
    const { seriler, renkHaritasi } = serileriKatla(tumBranslar, aylar.length);

    const aylikToplam = aylar.map((_, i) => tumBranslar.reduce((t, b) => t + b.degerler[i], 0));
    const genelToplam = aylikToplam.reduce((t, v) => t + v, 0);
    const enYogunAyI = aylikToplam.indexOf(Math.max(...aylikToplam));

    return { aylar, tumBranslar, seriler, aylikToplam, genelToplam, enYogunAyI, renkHaritasi };
  }, [kayitlar, baslangic, bitis, aralikHatasi]);

  const gorunurSeriler = rapor?.seriler.filter((s) => !gizliSeriler.has(s.ad)) ?? [];

  const csv = () =>
    csvIndir(`uzmanlik-bazli-hasta-gelisleri_${baslangic}_${bitis}.csv`, [
      ['Uzmanlık Dalı', ...rapor.aylar.map((a) => a.uzun), 'Toplam'],
      ...rapor.tumBranslar.map((b) => [b.ad, ...b.degerler, b.toplam]),
      ['Toplam', ...rapor.aylikToplam, rapor.genelToplam],
    ]);

  const veriVar = rapor && rapor.genelToplam > 0;
  const enYogunBrans = rapor?.tumBranslar[0];

  return (
    <>
      <TarihAraligiFiltresi rapor={veri} onCsv={csv} csvAktif={veriVar} />
      <RaporDurumu rapor={veri} veriVar={veriVar} bosBaslik="Bu aralıkta muayene kaydı yok" />

      {veriVar && (
        <div className={`rp-sonuc${yukleniyor ? ' rp-sonuc-yenileniyor' : ''}`}>
          <div className="ui-istatistik-grid rp-ozet">
            <IstatistikKart ikon="stetoskop" ton="bilgi" deger={sayiFormat(rapor.genelToplam)} etiket="Toplam muayene" />
            <IstatistikKart ikon="pano" deger={sayiFormat(rapor.tumBranslar.length)} etiket="Muayene yapılan branş" />
            <IstatistikKart
              ikon="trend"
              ton="basari"
              deger={enYogunBrans.ad}
              etiket={`En yoğun branş · ${sayiFormat(enYogunBrans.toplam)} muayene`}
            />
            <IstatistikKart
              ikon="takvim"
              ton="uyari"
              deger={rapor.aylar[rapor.enYogunAyI].uzun}
              etiket={`En yoğun ay · ${sayiFormat(rapor.aylikToplam[rapor.enYogunAyI])} muayene`}
            />
          </div>

          <section className="ui-kart">
            <div className="ui-kart-baslik rp-grafik-baslik">
              <h2>
                <Ikon ad={GORUNUMLER.find((g) => g.id === gorunum).ikon} />
                {gorunum === 'siralama' ? 'Branşlara göre toplam muayene' : 'Aylara göre branş bazlı muayene sayısı'}
              </h2>
              <div className="ui-segment" role="tablist" aria-label="Grafik görünümü">
                {GORUNUMLER.map((g) => (
                  <button
                    type="button"
                    role="tab"
                    aria-selected={gorunum === g.id}
                    key={g.id}
                    className={gorunum === g.id ? 'ui-segment-aktif' : ''}
                    onClick={() => setGorunum(g.id)}
                  >
                    {g.etiket}
                  </button>
                ))}
              </div>
            </div>
            <div className="ui-kart-govde">
              {gorunum !== 'siralama' && (
                <SeriLejanti seriler={rapor.seriler} gizliSeriler={gizliSeriler} onDegis={setGizliSeriler} />
              )}
              {gorunum === 'egilim' && (
                <CizgiGrafik
                  etiketler={rapor.aylar.map((a) => a.kisa)}
                  uzunEtiketler={rapor.aylar.map((a) => a.uzun)}
                  seriler={gorunurSeriler}
                />
              )}
              {gorunum === 'dagilim' && (
                <YiginGrafik
                  etiketler={rapor.aylar.map((a) => a.kisa)}
                  uzunEtiketler={rapor.aylar.map((a) => a.uzun)}
                  seriler={gorunurSeriler}
                  ariaEtiket="Aylara göre branş bazlı muayene sayısı"
                />
              )}
              {gorunum === 'siralama' && (
                <SiralamaGrafik satirlar={rapor.tumBranslar.map((b) => ({ ad: b.ad, deger: b.toplam }))} />
              )}
            </div>
          </section>

          <section className="ui-kart">
            <div className="ui-kart-baslik">
              <h2>
                <Ikon ad="liste" /> Detay tablo
              </h2>
              <span className="ui-sayac">{rapor.tumBranslar.length}</span>
            </div>
            <div className="ui-tablo-kapsayici">
              <table className="ui-tablo rp-tablo">
                <thead>
                  <tr>
                    <th>Uzmanlık Dalı</th>
                    {rapor.aylar.map((a) => (
                      <th key={a.anahtar} className="ui-tablo-sag" title={a.uzun}>{a.kisa}</th>
                    ))}
                    <th className="ui-tablo-sag">Toplam</th>
                  </tr>
                </thead>
                <tbody>
                  {rapor.tumBranslar.map((b) => (
                    <tr key={b.ad}>
                      <td className="rp-tablo-brans">
                        <span className="grf-lejant-isaret" style={{ background: rapor.renkHaritasi.get(b.ad) ?? DIGER_RENGI }} />
                        {b.ad}
                      </td>
                      {b.degerler.map((v, i) => (
                        <td key={i} className={`ui-tablo-sag ui-sayi${v === 0 ? ' ui-soluk' : ''}`}>{sayiFormat(v)}</td>
                      ))}
                      <td className="ui-tablo-sag ui-sayi rp-tablo-toplam">{sayiFormat(b.toplam)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <td>Toplam</td>
                    {rapor.aylikToplam.map((v, i) => (
                      <td key={i} className="ui-tablo-sag ui-sayi">{sayiFormat(v)}</td>
                    ))}
                    <td className="ui-tablo-sag ui-sayi">{sayiFormat(rapor.genelToplam)}</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </section>
        </div>
      )}
    </>
  );
}
