import { useState } from 'react';
import { createPortal } from 'react-dom';
import './Bildirim.css';

// Tüm bildirimler body altındaki tek bir sabit alana portal'lanır; böylece modal
// içinden de, sayfadan da açılsa aynı köşede üst üste yığılırlar.
function bildirimAlani() {
  let alan = document.getElementById('bildirim-alani');
  if (!alan) {
    alan = document.createElement('div');
    alan.id = 'bildirim-alani';
    alan.setAttribute('aria-live', 'assertive');
    document.body.appendChild(alan);
  }
  return alan;
}

const BASLIKLAR = { hata: 'Hata', basari: 'Başarılı', uyari: 'Uyarı' };

const IKONLAR = {
  hata: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">
      <circle cx="12" cy="12" r="10" />
      <line x1="12" y1="7.5" x2="12" y2="13" />
      <line x1="12" y1="16.5" x2="12.01" y2="16.5" />
    </svg>
  ),
  basari: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">
      <circle cx="12" cy="12" r="10" />
      <polyline points="7.5 12.5 10.5 15.5 16.5 9" />
    </svg>
  ),
  uyari: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M10.3 3.9 1.8 18a2 2 0 0 0 1.7 3h17a2 2 0 0 0 1.7-3L13.7 3.9a2 2 0 0 0-3.4 0z" />
      <line x1="12" y1="9" x2="12" y2="13" />
      <line x1="12" y1="17" x2="12.01" y2="17" />
    </svg>
  ),
};

function BildirimKutusu({ mesaj, tip, sure, onKapat }) {
  const [cikiyor, setCikiyor] = useState(false);
  const [kapandi, setKapandi] = useState(false);
  const [duraklatildi, setDuraklatildi] = useState(false);

  const kapat = () => setCikiyor(true);

  // Çıkış animasyonu bitince gerçekten kapat; parent state'i de temizlensin ki
  // aynı mesaj tekrar oluştuğunda popup yeniden açılabilsin.
  const animasyonBitti = () => {
    if (!cikiyor) return;
    setKapandi(true);
    onKapat?.();
  };

  if (kapandi) return null;

  return createPortal(
    <div
      className={`bildirim-kutu bildirim-kutu-${tip}${cikiyor ? ' bildirim-kutu-cikis' : ''}`}
      role={tip === 'hata' ? 'alert' : 'status'}
      // Portal'dan gelen tıklama React ağacında modal overlay'ine kadar yükselip
      // modalı kapatmasın.
      onClick={(e) => e.stopPropagation()}
      onMouseEnter={() => setDuraklatildi(true)}
      onMouseLeave={() => setDuraklatildi(false)}
      onAnimationEnd={animasyonBitti}
    >
      <div className="bildirim-ikon">{IKONLAR[tip]}</div>
      <div className="bildirim-icerik">
        <div className="bildirim-baslik">{BASLIKLAR[tip]}</div>
        <div className="bildirim-metin">{mesaj}</div>
      </div>
      <button type="button" className="bildirim-kapat" onClick={kapat} aria-label="Kapat">
        ×
      </button>
      {sure > 0 && (
        <div
          className="bildirim-sure"
          style={{ animationDuration: `${sure}ms`, animationPlayState: duraklatildi ? 'paused' : 'running' }}
          onAnimationEnd={(e) => {
            e.stopPropagation();
            kapat();
          }}
        />
      )}
    </div>,
    bildirimAlani()
  );
}

// Kullanım: {hata && <div className="...">{hata}</div>} yerine
//   <Bildirim mesaj={hata} onKapat={() => setHata('')} />
// mesaj boşsa hiçbir şey çizmez; mesaj değiştiğinde popup yeniden açılır.
// onKapat verilmezse popup kendi içinde kapanır, parent state'i olduğu gibi kalır
// (hata state'ine bağlı "liste boş" gibi koşullu render'lar bozulmasın diye).
export default function Bildirim({ mesaj, tip = 'hata', sure, onKapat }) {
  if (!mesaj) return null;
  const varsayilanSure = tip === 'basari' ? 4000 : 7000;
  return (
    <BildirimKutusu
      key={mesaj}
      mesaj={mesaj}
      tip={tip}
      sure={sure ?? varsayilanSure}
      onKapat={onKapat}
    />
  );
}
