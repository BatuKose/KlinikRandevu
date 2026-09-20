// Backend iki farklı hata şekli dönebiliyor:
// - GlobalExceptionMiddleware (BadRequestException/NotFoundException vb.) kendi
//   JsonSerializer çağrısıyla PascalCase dönüyor: { IsSuccess, Message, Data }
// - ModelState validasyon hataları ASP.NET'in camelCase ProblemDetails'i üzerinden
//   geliyor: { errors: { alan: ["mesaj"] } }
export function getApiErrorMessage(err, fallback = 'Bir hata oluştu') {
  const data = err?.response?.data;
  if (!data) return err?.message || fallback;
  if (data.errors) return Object.values(data.errors).flat().join(', ');
  return data.Message || data.message || fallback;
}
