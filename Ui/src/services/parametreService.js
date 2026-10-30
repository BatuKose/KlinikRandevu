import api from './api';

export const parametreService = {
  hepsiniGetir: () => api.get('/SistemParametreleri/hepsinigetir'),
  ekle: (data) => api.post('/SistemParametreleri/parametreekle', data),
  guncelle: (id, data) => api.patch(`/SistemParametreleri/parametreguncelle/${id}`, data),
  cacheTemizle: () => api.post('/SistemParametreleri/cache-temizle'),
  kullaniciEkle: (data) => api.post('/SistemParametreleri/kullaniciekle', data),
  loglariGetir: (baslangic, bitis, aksiyonTipi) =>
    api.get('/SistemParametreleri/loglar', { params: { baslangic, bitis, aksiyonTipi: aksiyonTipi || undefined } }),
  logTipleriniGetir: () => api.get('/SistemParametreleri/log-tipleri'),
  smsAyarlariGetir: () => api.get('/SistemParametreleri/smsayarlari'),
  smsAyariEkle: (data) => api.post('/SistemParametreleri/smsayarikaydet', data),
  smsAyariGuncelle: (data) => api.put(`/SistemParametreleri/${encodeURIComponent('smsayarigüncelle')}`, data),
  smsAyariAktifPasif: (id) => api.patch(`/SistemParametreleri/sms/${id}/aktif-pasif`),
  emailAyarlariGetir: () => api.get('/SistemParametreleri/emailayarlari'),
  emailAyariEkle: (data) => api.post('/SistemParametreleri/emailayarikaydet', data),
  emailAyariGuncelle: (data) => api.put('/SistemParametreleri/emailayariguncelle', data),
  emailAyariAktifPasif: (id) => api.patch(`/SistemParametreleri/email/${id}/aktif-pasif`),
};
