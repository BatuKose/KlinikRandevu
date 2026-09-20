// Backend enum'ları (Entities.Enums.PatientEnum) int olarak serileşiyor,
// JsonStringEnumConverter kayıtlı değil — bu yüzden değerler burada sabit.
export const CINSIYET_SECENEKLERI = [
  { deger: 1, etiket: 'Kadın' },
  { deger: 2, etiket: 'Erkek' },
];

export const KAN_GRUBU_SECENEKLERI = [
  { deger: 0, etiket: 'A Rh+' },
  { deger: 1, etiket: 'A Rh-' },
  { deger: 2, etiket: 'B Rh+' },
  { deger: 3, etiket: 'B Rh-' },
  { deger: 4, etiket: 'AB Rh+' },
  { deger: 5, etiket: 'AB Rh-' },
  { deger: 6, etiket: '0 Rh+' },
  { deger: 7, etiket: '0 Rh-' },
];

export function cinsiyetEtiket(deger) {
  return CINSIYET_SECENEKLERI.find((c) => c.deger === deger)?.etiket ?? 'Belirtilmemiş';
}

export function kanGrubuEtiket(deger) {
  return KAN_GRUBU_SECENEKLERI.find((k) => k.deger === deger)?.etiket ?? '-';
}
