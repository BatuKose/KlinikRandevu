import api from './api';

export const raporlamaService = {
  uzmanlikBazliHastaGelisleri: (baslangic, bitis) =>
    api.get('/api/Raporlama/uzmanlikbazlihastagelisleri', { params: { baslangic, bitis } }),
  teshisRaporu: (baslangic, bitis) =>
    api.get('/api/Raporlama/teshisraporu', { params: { baslangic, bitis } }),
};
