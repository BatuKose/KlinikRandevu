// Backend token'ı ClaimTypes.Name ile üretiyor; JWT'ye yazılırken tam URI olarak
// serileşiyor, bu yüzden hem kısa hem uzun claim adını kontrol ediyoruz.
const NAME_CLAIM_URI = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

export function decodeToken(token) {
  if (!token) return null;
  try {
    const payload = token.split('.')[1];
    const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(normalized)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join('')
    );
    return JSON.parse(json);
  } catch {
    return null;
  }
}

export function getUsernameFromToken(decoded) {
  if (!decoded) return null;
  return decoded[NAME_CLAIM_URI] || decoded.unique_name || decoded.name || null;
}

export function getUserIdFromToken(decoded) {
  if (!decoded) return null;
  return decoded.UserID ?? null;
}

export function isTokenExpired(decoded) {
  if (!decoded?.exp) return true;
  return decoded.exp * 1000 <= Date.now();
}
