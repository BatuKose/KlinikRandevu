import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './AnaSayfa.css';

const IkonSistem = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z" />
    <circle cx="12" cy="12" r="3" />
  </svg>
);

const IkonHasta = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
    <circle cx="9" cy="7" r="4" />
    <line x1="19" y1="8" x2="19" y2="14" />
    <line x1="16" y1="11" x2="22" y2="11" />
  </svg>
);

const IkonRandevu = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <rect x="3" y="4" width="18" height="18" rx="2" />
    <line x1="16" y1="2" x2="16" y2="6" />
    <line x1="8" y1="2" x2="8" y2="6" />
    <line x1="3" y1="10" x2="21" y2="10" />
  </svg>
);

const IkonMuayene = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M4.8 2.3A.3.3 0 1 0 5 2H4a2 2 0 0 0-2 2v5a6 6 0 0 0 6 6v0a6 6 0 0 0 6-6V4a2 2 0 0 0-2-2h-1a.2.2 0 1 0 .3.3" />
    <path d="M8 15v1a6 6 0 0 0 6 6v0a6 6 0 0 0 6-6v-4" />
    <circle cx="20" cy="10" r="2" />
  </svg>
);

const moduller = [
  {
    baslik: 'Sistem Yönetimi',
    aciklama: 'Sistem parametreleri, kullanıcı ve yetki yönetimi',
    yol: '/sistem-yonetimi',
    aktif: true,
    ikon: <IkonSistem />,
  },
  {
    baslik: 'Hasta Kayıt',
    aciklama: 'Hasta kaydı oluşturma, TC no / protokol ile arama',
    yol: '/hasta-kayit',
    aktif: true,
    ikon: <IkonHasta />,
  },
  {
    baslik: 'Randevu',
    aciklama: 'Randevu oluşturma ve doktor çalışma planları',
    yol: '/randevu',
    aktif: false,
    ikon: <IkonRandevu />,
  },
  {
    baslik: 'Poliklinik',
    aciklama: 'Poliklinik bazlı hasta listesi, muayene, teşhis ve tedavi işlemleri',
    yol: '/poliklinik',
    aktif: true,
    ikon: <IkonMuayene />,
  },
];

export default function AnaSayfa() {
  const { user } = useAuth();

  return (
    <div className="sayfa">
      <div className="sayfa-baslik">
        <h1>Hoş geldin{user?.username ? `, ${user.username}` : ''}</h1>
        <p className="ana-sayfa-alt">Devam etmek için bir modül seç</p>
      </div>

      <div className="modul-grid">
        {moduller.map((modul) =>
          modul.aktif ? (
            <Link to={modul.yol} className="modul-kart" key={modul.baslik}>
              <div className="modul-ikon">{modul.ikon}</div>
              <div className="modul-metin">
                <h2>{modul.baslik}</h2>
                <p>{modul.aciklama}</p>
              </div>
            </Link>
          ) : (
            <div className="modul-kart modul-kart-pasif" key={modul.baslik} title="Yakında eklenecek">
              <div className="modul-ikon">{modul.ikon}</div>
              <div className="modul-metin">
                <h2>{modul.baslik}</h2>
                <p>{modul.aciklama}</p>
              </div>
              <span className="modul-rozet">Yakında</span>
            </div>
          )
        )}
      </div>
    </div>
  );
}
