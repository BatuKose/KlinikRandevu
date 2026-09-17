import { NavLink } from 'react-router-dom';
import './Sidebar.css';

const menuItems = [
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

export default function Sidebar({ acik, onToggle }) {
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
        {acik ? <div className="versiyon">v1.0.0</div> : <div className="versiyon-kisa">v1</div>}
      </div>
    </aside>
  );
}
