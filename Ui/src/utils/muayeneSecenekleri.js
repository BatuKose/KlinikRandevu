// Entities.Enums.odemeTipiEnum.OdemeEnum — JsonStringEnumConverter yok, int olarak dönüyor.
export const ODEME_TIPI = {
  TEDAVI_BAZLI: 1,
  TOPLAM_MUAYENE: 2,
};

export function muayeneDurumEtiket(bitisSaati) {
  return bitisSaati ? 'Kapalı' : 'Açık';
}
