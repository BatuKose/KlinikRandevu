import { useState, useEffect, useCallback } from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import './Layout.css';

export default function Layout() {
  const [acik, setAcik] = useState(() => window.innerWidth >= 1024);

  const handleResize = useCallback(() => {
    if (window.innerWidth < 1024) setAcik(false);
  }, []);

  useEffect(() => {
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [handleResize]);

  const toggle = () => setAcik((p) => !p);

  return (
    <div className={`layout ${acik ? 'sidebar-acik' : 'sidebar-kapali'}`}>
      <Sidebar acik={acik} onToggle={toggle} />

      {/* Mobilde sidebar açıkken arkayı karartan backdrop */}
      {acik && <div className="sidebar-backdrop" onClick={() => setAcik(false)} />}

      <main className="layout-icerik">
        {/* Sadece mobilde görünen üst bar */}
        <div className="mobil-topbar">
          <button className="mobil-menu-btn" onClick={toggle} aria-label="Menüyü aç/kapat">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <line x1="3" y1="6" x2="21" y2="6" />
              <line x1="3" y1="12" x2="21" y2="12" />
              <line x1="3" y1="18" x2="21" y2="18" />
            </svg>
          </button>
          <span className="mobil-site-adi">Klinik Randevu</span>
        </div>

        <Outlet />
      </main>
    </div>
  );
}
