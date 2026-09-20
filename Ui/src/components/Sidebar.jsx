import { NavLink } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Sidebar.css';

const menuItems = [
  {
    baslik: 'Genel',
    icerik: [
      {
        yol: '/',
        bitis: true,
        etiket: 'Ana Sayfa',
        ikon: (
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M3 9.5 12 3l9 6.5" />
            <path d="M5 9.5V21h14V9.5" />
            <path d="M9 21v-6h6v6" />
          </svg>
        ),
      },
    ],
  },
  {
    baslik: 'Hasta',
    icerik: [
      {
        yol: '/hasta-kayit',
        etiket: 'Hasta Kayıt',
        ikon: (
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <line x1="19" y1="8" x2="19" y2="14" />
            <line x1="16" y1="11" x2="22" y2="11" />
          </svg>
        ),
      },
      {
        yol: '/poliklinik',
        etiket: 'Poliklinik',
        ikon: (
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M4.8 2.3A.3.3 0 1 0 5 2H4a2 2 0 0 0-2 2v5a6 6 0 0 0 6 6v0a6 6 0 0 0 6-6V4a2 2 0 0 0-2-2h-1a.2.2 0 1 0 .3.3" />
            <path d="M8 15v1a6 6 0 0 0 6 6v0a6 6 0 0 0 6-6v-4" />
            <circle cx="20" cy="10" r="2" />
          </svg>
        ),
      },
    ],
  },
  {
    baslik: 'Sistem',
    icerik: [
      {
        yol: '/sistem-yonetimi',
        etiket: 'Sistem Yönetimi',
        ikon: (
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z" />
            <circle cx="12" cy="12" r="3" />
          </svg>
        ),
      },
    ],
  },
];

const IkonSol = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
    <polyline points="15 18 9 12 15 6" />
  </svg>
);

const IkonSag = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
    <polyline points="9 18 15 12 9 6" />
  </svg>
);

const IkonCikis = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
    <polyline points="16 17 21 12 16 7" />
    <line x1="21" y1="12" x2="9" y2="12" />
  </svg>
);

export default function Sidebar({ acik, onToggle }) {
  const { user, logout } = useAuth();

  return (
    <aside className={`sidebar ${acik ? 'sidebar-acik' : 'sidebar-kapali'}`}>
      {/* Logo + Toggle */}
      <div className="sidebar-logo">
        <div className="logo-ikon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M22 12h-4l-3 9L9 3l-3 9H2" />
          </svg>
        </div>
        {acik && (
          <div className="logo-yazi">
            <span className="logo-baslik">Klinik</span>
            <span className="logo-alt">Randevu Sistemi</span>
          </div>
        )}
        <button className="toggle-btn" onClick={onToggle} title={acik ? 'Küçült' : 'Genişlet'}>
          {acik ? <IkonSol /> : <IkonSag />}
        </button>
      </div>

      {/* Navigasyon */}
      <nav className="sidebar-nav">
        {menuItems.map((grup) => (
          <div className="nav-grup" key={grup.baslik}>
            {acik && <span className="nav-grup-baslik">{grup.baslik}</span>}
            {!acik && <div className="nav-grup-ayrac" />}
            {grup.icerik.map((item) => (
              <NavLink
                key={item.yol}
                to={item.yol}
                end={item.bitis}
                title={!acik ? item.etiket : undefined}
                className={({ isActive }) => `nav-item ${isActive ? 'nav-item-aktif' : ''}`}
              >
                <span className="nav-ikon">{item.ikon}</span>
                {acik && <span className="nav-etiket">{item.etiket}</span>}
              </NavLink>
            ))}
          </div>
        ))}
      </nav>

      {/* Alt */}
      <div className="sidebar-alt">
        {acik ? (
          <div className="sidebar-kullanici">
            <div className="kullanici-bilgi">
              <span className="kullanici-adi" title={user?.username || ''}>
                {user?.username || 'Kullanıcı'}
              </span>
              <span className="versiyon">v1.0.0</span>
            </div>
            <button className="cikis-btn" onClick={logout} title="Çıkış yap">
              <IkonCikis />
            </button>
          </div>
        ) : (
          <button className="cikis-btn" onClick={logout} title="Çıkış yap">
            <IkonCikis />
          </button>
        )}
      </div>
    </aside>
  );
}
