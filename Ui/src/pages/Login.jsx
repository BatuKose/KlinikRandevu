import { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { getApiErrorMessage } from '../utils/apiError';
import './Login.css';

export default function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [hata, setHata] = useState('');
  const [yukleniyor, setYukleniyor] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const hedefYol = location.state?.from?.pathname || '/';

  const submit = async (e) => {
    e.preventDefault();
    if (!username.trim() || !password.trim()) {
      setHata('Kullanıcı adı ve şifre zorunludur');
      return;
    }

    setHata('');
    setYukleniyor(true);
    try {
      await login(username.trim(), password);
      navigate(hedefYol, { replace: true });
    } catch (err) {
      setHata(getApiErrorMessage(err, 'Giriş başarısız'));
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="login-sayfa">
      <form className="login-kart" onSubmit={submit}>
        <div className="login-logo">
          <div className="login-logo-ikon">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M22 12h-4l-3 9L9 3l-3 9H2" />
            </svg>
          </div>
          <div>
            <div className="login-baslik">Klinik Randevu</div>
            <div className="login-alt">Yönetim Paneli</div>
          </div>
        </div>

        <label className="login-alan">
          <span>Kullanıcı Adı</span>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            autoFocus
            autoComplete="username"
            disabled={yukleniyor}
          />
        </label>

        <label className="login-alan">
          <span>Şifre</span>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
            disabled={yukleniyor}
          />
        </label>

        {hata && <div className="login-hata">{hata}</div>}

        <button type="submit" className="login-buton" disabled={yukleniyor}>
          {yukleniyor ? 'Giriş yapılıyor...' : 'Giriş Yap'}
        </button>
      </form>
    </div>
  );
}
