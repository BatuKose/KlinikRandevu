import { createContext, useCallback, useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';
import { setOnAuthFailure } from '../services/api';
import { tokenStorage } from '../services/tokenStorage';
import { decodeToken, getUsernameFromToken, isTokenExpired } from '../utils/jwt';

const AuthContext = createContext(null);

function userFromAccessToken(accessToken) {
  const decoded = decodeToken(accessToken);
  if (!decoded || isTokenExpired(decoded)) return null;
  return { username: getUsernameFromToken(decoded) };
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => userFromAccessToken(tokenStorage.getAccessToken()));
  const navigate = useNavigate();

  const logout = useCallback(() => {
    tokenStorage.clearTokens();
    setUser(null);
    navigate('/login', { replace: true });
  }, [navigate]);

  // api.js, refresh de dahil her yol başarısız olduğunda kullanıcıyı login'e
  // atabilmek için bu callback'e ihtiyaç duyuyor (döngüsel import olmasın diye).
  useEffect(() => {
    setOnAuthFailure(logout);
    return () => setOnAuthFailure(null);
  }, [logout]);

  const login = useCallback(async (username, password) => {
    const { data } = await authService.login(username, password);
    tokenStorage.setTokens(data.accessToken, data.refreshToken);
    setUser(userFromAccessToken(data.accessToken));
  }, []);

  const value = {
    user,
    isAuthenticated: !!user,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth, AuthProvider içinde kullanılmalıdır');
  return ctx;
}
