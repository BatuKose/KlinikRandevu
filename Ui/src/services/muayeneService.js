import api from './api';

export const muayeneService = {
  hastaninRandevulariniGetir: (protokol) =>
    api.get('/api/Poliklinik/hastaninrandevusunugetir', { params: { protokol } }),
  poliklinikHastaListesiGetir: (polNo, baslangic, bitis) =>
    api.get('/api/Poliklinik/poliklinikHastaListesiGetir', { params: { polNo, baslangic, bitis } }),
  randevuOlustur: (dto) => api.post('/api/Poliklinik/randevuolustur', dto),
  muayeneOlustur: (dto) => api.post('/api/Poliklinik/muayeneolustur', dto),
  aktifDoktorlariGetir: () => api.get('/api/Poliklinik/doktorListesiGetir'),
  aktifServisleriGetir: () => api.get('/api/Poliklinik/ServisListesiGetir'),
  randevuIptalEt: (id) => api.patch('/api/Poliklinik/randevuiptalet', null, { params: { id } }),

  muayeneKaydiGetir: (id) => api.get('/api/Poliklinik/muayeneGetir', { params: { id } }),
  muayeneRandevuIleGetir: (randevuId) =>
    api.get('/api/Poliklinik/muayeneRandevuIleGetir', { params: { randevuId } }),
  teshisleriGetir: (muayeneId) => api.get('/api/Poliklinik/teshisleriGetir', { params: { muayeneId } }),
  tedavileriGetir: (muayeneId) => api.get('/api/Poliklinik/tedavileriGetir', { params: { muayeneId } }),
  odemeleriGetir: (muayeneId) => api.get('/api/Poliklinik/odemeleriGetir', { params: { muayeneId } }),
  muayeneBorcGetir: (muayeneId) => api.get('/api/Poliklinik/muayeneBorcGetir', { params: { muayeneId } }),
  muayeneOdemeToplamGetir: (muayeneId) =>
    api.get('/api/Poliklinik/muayeneOdemeToplamGetir', { params: { muayeneId } }),
  teshisEkle: (muayeneId, teshisMetni) =>
    api.post(`/api/Poliklinik/${muayeneId}/teshisekle`, JSON.stringify(teshisMetni), {
      headers: { 'Content-Type': 'application/json' },
    }),
  tedaviEkle: (dto) => api.post('/api/Poliklinik/tedaviEkle', dto),
  odemeYap: (dto) => api.post('/api/Poliklinik/odenemeYap', dto),
  odemeIptalEt: (dto) => api.patch('/api/Poliklinik/OdemeIptal', dto),
  muayeneKapat: (id) => api.patch('/api/Poliklinik/muayenebitis', null, { params: { id } }),
};
