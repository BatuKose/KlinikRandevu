import api from './api';

export const hastaService = {
  ara: (arama) => api.get('/api/Patient/hastakayithastagetir', { params: { arama } }),
  ekle: (dto) => api.post('/api/Patient/hastakayit', dto),
  guncelle: (protokol, dto) =>
    api.put('/api/Patient/hastakayithastagüncelle', dto, { params: { protokol } }),
};
