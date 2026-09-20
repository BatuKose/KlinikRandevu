import { useEffect, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';

// Doktor/servis listesi boşsa backend NotFoundException (404) fırlatıyor;
// bu bir hata değil, sadece "aktif kayıt yok" demek.
export function useDoktorVeServisListesi() {
  const [doktorlar, setDoktorlar] = useState([]);
  const [servisler, setServisler] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(true);
  const [hata, setHata] = useState('');

  useEffect(() => {
    let iptal = false;

    (async () => {
      setYukleniyor(true);
      setHata('');
      const [doktorSonuc, servisSonuc] = await Promise.allSettled([
        muayeneService.aktifDoktorlariGetir(),
        muayeneService.aktifServisleriGetir(),
      ]);
      if (iptal) return;

      setDoktorlar(doktorSonuc.status === 'fulfilled' ? doktorSonuc.value.data?.data || [] : []);
      setServisler(servisSonuc.status === 'fulfilled' ? servisSonuc.value.data?.data || [] : []);

      const gercekHata = [doktorSonuc, servisSonuc].find(
        (sonuc) => sonuc.status === 'rejected' && sonuc.reason?.response?.status !== 404
      );
      if (gercekHata) {
        setHata(getApiErrorMessage(gercekHata.reason, 'Doktor/poliklinik listesi alınamadı'));
      }
      setYukleniyor(false);
    })();

    return () => {
      iptal = true;
    };
  }, []);

  return { doktorlar, servisler, yukleniyor, hata };
}
