import { useEffect, useState } from 'react';
import { muayeneService } from '../../services/muayeneService';
import { getApiErrorMessage } from '../../utils/apiError';

export function useUzmanlikBranslari() {
  const [branslar, setBranslar] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(true);
  const [hata, setHata] = useState('');

  useEffect(() => {
    let iptal = false;
    muayeneService
      .uzmanlikBranslariniGetir()
      .then(({ data }) => {
        if (!iptal) setBranslar(data?.data || []);
      })
      .catch((err) => {
        if (!iptal) setHata(getApiErrorMessage(err, 'Uzmanlık branşları alınamadı'));
      })
      .finally(() => {
        if (!iptal) setYukleniyor(false);
      });
    return () => {
      iptal = true;
    };
  }, []);

  return { branslar, yukleniyor, hata };
}
