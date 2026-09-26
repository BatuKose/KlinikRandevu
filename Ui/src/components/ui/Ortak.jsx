import Ikon from './Ikon';

export function SayfaBaslik({ ikon, baslik, aciklama, children }) {
  return (
    <header className="ui-sayfa-baslik">
      <div className="ui-sayfa-baslik-sol">
        {ikon && (
          <div className="ui-sayfa-ikon">
            <Ikon ad={ikon} />
          </div>
        )}
        <div>
          <h1>{baslik}</h1>
          {aciklama && <p>{aciklama}</p>}
        </div>
      </div>
      {children && <div className="ui-sayfa-aksiyonlar">{children}</div>}
    </header>
  );
}

export function BosDurum({ ikon = 'bilgi', baslik, aciklama, kompakt, children }) {
  return (
    <div className={`ui-bos${kompakt ? ' ui-bos-kompakt' : ''}`}>
      <div className="ui-bos-ikon">
        <Ikon ad={ikon} />
      </div>
      {baslik && <h3>{baslik}</h3>}
      {aciklama && <p>{aciklama}</p>}
      {children && <div className="ui-bos-aksiyon">{children}</div>}
    </div>
  );
}

// Liste/tablo yüklenirken "Yükleniyor..." yazısı yerine satır iskeleti.
export function IskeletListe({ satir = 4, avatar = true }) {
  return (
    <div className="ui-iskelet-satirlar" aria-busy="true" aria-label="Yükleniyor">
      {Array.from({ length: satir }, (_, i) => (
        <div className="ui-iskelet-satir" key={i}>
          {avatar && <span className="ui-iskelet" style={{ width: 36, height: 36, borderRadius: '50%' }} />}
          <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: 7 }}>
            <span className="ui-iskelet" style={{ width: `${55 - (i % 3) * 10}%` }} />
            <span className="ui-iskelet" style={{ width: `${35 - (i % 2) * 8}%`, height: 10 }} />
          </div>
        </div>
      ))}
    </div>
  );
}

export function IstatistikKart({ ikon, ton, deger, etiket }) {
  return (
    <div className="ui-istatistik">
      <div className={`ui-istatistik-ikon${ton ? ` ui-istatistik-${ton}` : ''}`}>
        <Ikon ad={ikon} />
      </div>
      <div>
        <div className="ui-istatistik-deger">{deger}</div>
        <div className="ui-istatistik-etiket">{etiket}</div>
      </div>
    </div>
  );
}

// İsme göre sabit bir renk seçiyoruz ki aynı hasta her ekranda aynı renkte görünsün.
const AVATAR_PALETI = [
  ['#dbeafe', '#1d4ed8'],
  ['#dcfce7', '#15803d'],
  ['#fef3c7', '#b45309'],
  ['#fce7f3', '#be185d'],
  ['#ede9fe', '#6d28d9'],
  ['#cffafe', '#0e7490'],
  ['#ffedd5', '#c2410c'],
];

export function Avatar({ ad = '', soyad = '', boyut }) {
  const basHarfler = `${ad.trim()[0] || ''}${soyad.trim()[0] || ''}`.toLocaleUpperCase('tr-TR') || '?';
  const tohum = [...`${ad}${soyad}`].reduce((t, c) => t + c.charCodeAt(0), 0);
  const [zemin, metin] = AVATAR_PALETI[tohum % AVATAR_PALETI.length];
  return (
    <div
      className={`ui-avatar${boyut ? ` ui-avatar-${boyut}` : ''}`}
      style={{ '--avatar-zemin': zemin, '--avatar-metin': metin }}
      aria-hidden="true"
    >
      {basHarfler}
    </div>
  );
}

const AY_KISA = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'];

export function TarihKutu({ tarih, pasif }) {
  const d = new Date(tarih);
  return (
    <div className={`ui-tarih-kutu${pasif ? ' ui-tarih-kutu-pasif' : ''}`}>
      <span className="ui-tarih-kutu-ay">{AY_KISA[d.getMonth()]}</span>
      <span className="ui-tarih-kutu-gun">{d.getDate()}</span>
    </div>
  );
}
