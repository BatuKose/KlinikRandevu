import api from './api';

export const parametreService = {
  hepsiniGetir: () => api.get('/SistemParametreleri/hepsinigetir'),
  ekle: (data) => api.post('/SistemParametreleri/parametreekle', data),
  guncelle: (id, data) => api.patch(`/SistemParametreleri/parametreguncelle/${id}`, data),
  cacheTemizle: () => api.post('/SistemParametreleri/cache-temizle'),
  kullaniciEkle: (data) => api.post('/SistemParametreleri/kullaniciekle', data),
};
