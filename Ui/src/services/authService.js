import api from './api';

export const authService = {
  login: (username, password) => api.post('/Authentication/login', { username, password }),
  refresh: (refreshToken) => api.post('/Authentication/refresh', { refreshToken }),
};
