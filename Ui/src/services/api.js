import axios from 'axios';
import { tokenStorage } from './tokenStorage';

const baseURL = import.meta.env.VITE_API_URL || 'https://localhost:1000';

const api = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Refresh isteği için ayrı, interceptor'suz bir instance (döngüye girmesin diye)
const refreshApi = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// AuthContext, token yenileme tamamen başarısız olduğunda (refresh token da
// geçersizse) burayı kullanıcıyı login'e atmak için dolduruyor.
let onAuthFailure = null;
export const setOnAuthFailure = (handler) => {
  onAuthFailure = handler;
};

api.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

let isRefreshing = false;
let refreshQueue = [];

const AUTH_ENDPOINTS = ['/Authentication/login', '/Authentication/refresh'];

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    const status = error.response?.status;
    const isAuthEndpoint = AUTH_ENDPOINTS.some((url) => originalRequest?.url?.includes(url));

    if (status !== 401 || !originalRequest || originalRequest._retry || isAuthEndpoint) {
      return Promise.reject(error);
    }

    const refreshToken = tokenStorage.getRefreshToken();
    if (!refreshToken) {
      onAuthFailure?.();
      return Promise.reject(error);
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        refreshQueue.push({ resolve, reject, originalRequest });
      });
    }

    originalRequest._retry = true;
    isRefreshing = true;

    try {
      const { data } = await refreshApi.post('/Authentication/refresh', { refreshToken });
      tokenStorage.setTokens(data.accessToken, data.refreshToken);

      refreshQueue.forEach(({ resolve, originalRequest: queuedRequest }) => {
        queuedRequest.headers.Authorization = `Bearer ${data.accessToken}`;
        resolve(api(queuedRequest));
      });
      refreshQueue = [];

      originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
      return api(originalRequest);
    } catch (refreshError) {
      refreshQueue.forEach(({ reject }) => reject(refreshError));
      refreshQueue = [];
      tokenStorage.clearTokens();
      onAuthFailure?.();
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  }
);

export default api;
