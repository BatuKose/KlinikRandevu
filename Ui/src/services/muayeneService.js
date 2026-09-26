import api from './api';

export const muayeneService = {
  hastaninRandevulariniGetir: (protokol) =>
    api.get('/api/Poliklinik/hastaninrandevusunugetir', { params: { protokol } }),
  hastaninPoliklinikKayitlariniGetir: (protokol) =>
    api.get('/api/Poliklinik/hastaninPoliklinikKayitlariniGetir', { params: { protokol } }),
  randevulariGetir: (baslangic, bitis) =>
    api.get('/api/Poliklinik/randevularigetir', { params: { baslangic, bitis } }),
  poliklinikHastaListesiGetir: (polNo, baslangic, bitis) =>
    api.get('/api/Poliklinik/poliklinikHastaListesiGetir', { params: { polNo, baslangic, bitis } }),
  randevuOlustur: (dto) => api.post('/api/Poliklinik/randevuolustur', dto),
  muayeneOlustur: (dto) => api.post('/api/Poliklinik/muayeneolustur', dto),
  aktifDoktorlariGetir: () => api.get('/api/Poliklinik/doktorListesiGetir'),
  aktifServisleriGetir: () => api.get('/api/Poliklinik/ServisListesiGetir'),
  uzmanlikBranslariniGetir: () => api.get('/api/Poliklinik/uzmanlikBranslariniGetir'),
  doktorEkle: (dto) => api.post('/api/Poliklinik/doktorEkle', dto),
  servisEkle: (dto) => api.post('/api/Poliklinik/servisEkle', dto),
  randevuIptalEt: (id) => api.patch('/api/Poliklinik/randevuiptalet', null, { params: { id } }),

  calismaPlanlariGetir: (params) => api.get('/api/Poliklinik/calismaPlanlariGetir', { params }),
  calismaPlaniOlustur: (dto) => api.post('/api/Poliklinik/calismaplaniolustur', dto),
  calismaPlaniKopyala: (calismaPlaniId, yeniGun) =>
    api.post('/api/Poliklinik/CalismaPlaniKopyala', { calismaPlaniId, yeniGün: yeniGun }),
  calismaPlaniPasifeAl: (id) => api.patch('/api/Poliklinik/calismaPlaniPasifeAl', null, { params: { id } }),
  musaitSlotlariGetir: (doktorNo, polNo, tarih) =>
    api.get('/api/Poliklinik/musaitSlotlariGetir', { params: { doktorNo, polNo, tarih } }),

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

  hastaninTaahutnameleriniGetir: (protokol) =>
    api.get('/api/Poliklinik/hastaninTaahutnameleriniGetir', { params: { protokol } }),
  // Backend route'u "taahütnameEKle" (ü'lü); yol elle kodlanıyor.
  taahutnameEkle: (dto) => api.post('/api/Poliklinik/taah%C3%BCtnameEKle', dto),
  taahutnameGuncelle: (dto) => api.put('/api/Poliklinik/taahutnameGuncelle', dto),
  taahutnameIptalEt: (id) => api.patch('/api/Poliklinik/taahutnameIptalEt', null, { params: { id } }),
};
