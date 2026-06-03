# KlinikRandevu - Klinik Randevu Yönetim Sistemi

Klinik ortamlarında hasta kaydı, randevu planlaması, doktor çalışma programı ve muayene kaydı yönetimini sağlayan RESTful Web API projesi.

---

## Teknolojiler

| Teknoloji | Versiyon | Ne İşe Yarar? |
|---|---|---|
| **.NET 8 / ASP.NET Core** | 8.0 | Web API'nin çalıştığı framework. HTTP isteklerini karşılar, middleware pipeline'ı yönetir. |
| **Entity Framework Core** | 8.0.26 | ORM katmanı. C# nesneleri ile SQL Server arasında köprü kurar, migration desteği sağlar. |
| **SQL Server (LocalDB)** | - | Veritabanı. Tüm hasta, doktor, randevu ve kullanıcı verileri burada tutulur. |
| **JWT Bearer Authentication** | 8.0.0 | Kimlik doğrulama. Kullanıcı giriş yaptıktan sonra imzalı token alır, her istekte bu token ile kendini doğrular. |
| **Serilog** | 10.0.0 | Yapılandırılmış loglama. Uygulama loglarını hem konsola hem de günlük dönen dosyalara (`logs/log-YYYYMMDD.txt`) yazar. |
| **MailKit / MimeKit** | 4.16.0 | E-posta gönderme kütüphanesi. Randevu onay maili ve doktora günlük program bildirimi gönderir. |
| **Swashbuckle (Swagger)** | 6.6.2 | API dokümantasyonu ve test arayüzü. JWT Bearer token girişi destekler. |
| **Rate Limiter** | .NET 8 built-in | IP bazlı istek sınırlama. Her IP için 30 saniyede maksimum 100 istek, 3 isteklik kuyruk. |
| **Memory Cache** | .NET built-in | Sistem parametrelerini 3 saat boyunca bellekte tutar, her istekte DB'ye gitmez. |
| **ASP.NET Core Identity (EF)** | 8.0.26 | Kullanıcı ve yetki modelleri için temel sağlar. |

---

## Mimari

Proje 5 katmanlı (N-Layer) bir mimariyle tasarlanmıştır. Her katmanın tek bir sorumluluğu vardır ve yalnızca bir alt katmanı referans alır.

```
┌──────────────────────────────────────┐
│         KlinikRandevu (API Host)     │  ← Program.cs, DI kayıtları, Middleware, Migrations
│          ↓ referans alır             │
├──────────────────────────────────────┤
│            Presentation              │  ← Controller'lar, Action Filter'lar
│          ↓ referans alır             │
├──────────────────────────────────────┤
│              Services                │  ← İş kuralları (Manager sınıfları), e-posta, cache
│          ↓ referans alır             │
├──────────────────────────────────────┤
│            Repositories              │  ← EF Core implementasyonları, RepositoryContext
│          ↓ referans alır             │
├──────────────────────────────────────┤
│              Entities                │  ← Domain modeller, DTO'lar, Enum'lar, Exception'lar
└──────────────────────────────────────┘
```

### Katman Detayları

#### `KlinikRandevu` — API Host
- **Program.cs**: Tüm servislerin DI'a kaydedildiği ve middleware sırasının belirlendiği giriş noktası.
- **Extensions/ServicesExtensions.cs**: DI kayıtlarını extension method'lara böler (JWT, Swagger, CORS, Rate Limiter, Serilog, EF Core).
- **Migrations/**: EF Core migration dosyaları burada tutulur.

#### `Entities` — Domain Katmanı
Hiçbir başka projeye bağımlılığı yoktur. Tüm katmanlarca kullanılabilir.
- **Models/**: Veritabanı tabloları ile eşleşen C# sınıfları.
- **Data Transfer Objects/**: API'ye giren/çıkan veri şemaları (DTO). Entity'leri doğrudan dışa açmaz.
- **Enums/**: `GenderEnum`, `BloodTypeEnum`, `UzmanlikBransi` gibi sabit değer listeleri.
- **Exceptions/**: `BadRequestException`, `NotFoundException`, `ParamException` ve bunları yakalayan `GlobalExceptionMiddleware`.
- **Constants/**: `YetkiKodlari` (yetki sistemi için sabit string kodlar).

#### `Repositories` — Veri Erişim Katmanı
EF Core ile SQL Server arasındaki köprü. Servis katmanı somut DB implementasyonunu bilmez, sadece arayüzleri (`IPatientRepository`, `IMuayeneRepository` vb.) kullanır.
- **EFCore/RepositoryContext.cs**: `DbContext` sınıfı. Tüm `DbSet`'leri tanımlar.
- **EFCore/RepositorManager.cs**: Tüm repository'leri tek noktadan sunan `IRepositoryManager` implementasyonu.

#### `Services` — İş Mantığı Katmanı
Business logic burada yaşar. Her servis kendi `Manager` sınıfıyla implemente edilir.
- **PatientManager**: Hasta oluşturma, güncelleme, silme (soft delete), TC ve protokol bazlı arama.
- **MuayeneManager**: Çalışma planı oluşturma, randevu açma (slot validasyonu, çakışma kontrolü), muayene kaydı açma (parametre bazlı iş kuralları), mail gönderme.
- **AuthenticationManager**: JWT access token + refresh token üretimi, login audit log.
- **SistemParametreServiceManager**: Sistem parametresi ekleme/güncelleme, memory cache ile 3 saatlik okuma.
- **EmailManager**: MailKit üzerinden HTML içerikli mail gönderme.
- **UserLogManager**: Kullanıcı işlem loglarını yönetir.
- **UserYetkiManager**: Kullanıcının yetkilerini sorgular (`YetkiKontrol` attribute tarafından kullanılır).
- **ServiceManager**: Tüm servisleri tek bir nesne üzerinden sunar.

#### `Presentation` — Sunum Katmanı
Controller'lar ve action filter'lar burada bulunur. HTTP ile iş mantığı arasındaki ince köprü rolündedir; controller'lar sadece yönlendirme yapar, iş kuralı içermez.
- **Controllers/**: API endpoint'leri.
- **ActionFilters/YetkiKontrolAttribute.cs**: JWT token'daki `UserID` claim'ini okuyarak kullanıcının ilgili yetkiye sahip olup olmadığını DB'den kontrol eden custom authorization filter.

---

## Domain Modeller

| Model | Açıklama |
|---|---|
| `Patient` | Hasta. TC, protokol no, doğum tarihi, cinsiyet, kan grubu, e-posta. |
| `Doctor` | Doktor. Doktor no, tescil no, uzmanlık branşı, servis no. |
| `Poliklinik` | Poliklinik birimi. Randevu müsaitliği, maks. randevu süresi/sayısı parametreleri. |
| `DoktorCalismaPlani` | Doktorun hangi gün, hangi poliklinik, hangi saatler arası çalıştığını ve randevu süresini tanımlar. |
| `Randevu` | Hasta + Doktor + Poliklinik + Tarih/saat + süre. Slot validasyonuna göre açılır. |
| `MuayeneKaydi` | Açılmış muayene kaydı. Randevuya bağlı ya da randevusuz olabilir (parametre bağımlı). |
| `SistemParametresi` | DB'den yönetilen uygulama kuralları (bkz. Sistem Parametreleri). |
| `User` | Kullanıcı. Refresh token ve expiry alanları dahil. |
| `Yetki` | Yetki tanımı. Kod ve açıklama içerir. |
| `UserYetki` | Kullanıcı-Yetki ilişki tablosu. |
| `UserLog` | Login ve işlem logları. Kullanıcı ID, IP adresi, aksiyon tipi. |

---

## API Endpoint'leri

### Authentication — `/Authentication`
| Method | Endpoint | Açıklama |
|---|---|---|
| POST | `/login` | Kullanıcı girişi. Access token + refresh token döner. |
| POST | `/refresh` | Refresh token ile yeni access token alır. |

### Hasta — `/api/Patient`
| Method | Endpoint | Açıklama |
|---|---|---|
| POST | `/hastakayit` | Yeni hasta kaydı oluşturur. |
| GET | `/hastakayithastagetir?arama=...` | TC veya protokol ile hasta arar. |
| PUT | `/hastakayithastagüncelle?protokol=...` | Hasta bilgilerini günceller. |
| PATCH | `/hastakayithastasil?protkol=...` | Hastayı pasife alır (soft delete). |

### Poliklinik & Randevu — `/api/Poliklinik`
| Method | Endpoint | Açıklama |
|---|---|---|
| POST | `/calismaplaniolustur` | Doktor çalışma planı oluşturur. Slot validasyonları uygulanır. |
| POST | `/randevuolustur` | Randevu açar. Çakışma, slot, aynı gün kontrolü yapılır. |
| POST | `/muayeneolustur` | Muayene kaydı açar. `[YetkiKontrol]` korumalı. |
| GET | `/randevularigetir?baslangic=&bitis=` | Tarih aralığına göre randevuları listeler. |
| GET | `/hastaninrandevusunugetir?protokol=` | Hastanın tüm randevularını listeler. |
| PATCH | `/{doktorId}/docpasif` | Doktoru aktif/pasif yapar (ileri randevu varsa engeller). |
| PATCH | `/{polId}/polpasif` | Polikliniği aktif/pasif yapar. |
| POST | `/doktor/{doktorNo}/randevu-hatirlatma-mail` | Doktora günlük randevu programını e-posta ile gönderir. |

### Sistem Parametreleri — `/SistemParametreleri`
| Method | Endpoint | Açıklama |
|---|---|---|
| POST | `/parametreekle` | Yeni parametre ekler. |
| PATCH | `/parametreguncelle?id=` | Mevcut parametreyi günceller. |

---

## Sistem Parametreleri

Uygulama davranışları kod değişikliği gerektirmeden DB üzerinden yönetilir. Parametreler 3 saat boyunca memory cache'te tutulur.

| Parametre Kodu | Ne Yapar? |
|---|---|
| `RANDEVUSUZ_KAYIT_ACMA` | Belirli bir poliklinik için randevusuz muayene kaydı açılmasına izin verir/engeller. |
| `PEDIATRI_YAS_LIMITI` | Pedodonti polikliniği için minimum ve maksimum hasta yaşı sınırı koyar. |
| `KADIN_DOGUM_ERKEK_YASAKLA` | Kadın Hastalıkları ve Doğum polikliniğine erkek/belirsiz cinsiyet hasta kaydını engeller. |
| `EMAIL_GONDERME` | E-posta gönderimini açar/kapatır. Belirli poliklinik veya doktor için bypass edilebilir. |
| `LOGIN_LOG_TUTULSUN` | Kullanıcı girişlerinin IP adresiyle birlikte loglanmasını açar/kapatır. |

---

## Güvenlik

- **JWT Bearer Token**: Access token süresi **5 saat**, refresh token süresi **7 gün**.
- **Refresh Token**: Kriptografik rastgele 64 byte (Base64) olarak üretilir.
- **YetkiKontrol Attribute**: İstek JWT'den `UserID` claim'ini okur, kullanıcının o yetki koduna sahip olup olmadığını DB'den kontrol eder.
- **Rate Limiting**: IP başına 30 saniyede 100 istek. Aşılırsa `429 Too Many Requests`. Tüm controller'lara uygulanır.
- **Global Exception Middleware**: Tüm yakalanmayan exception'lar burada karşılanır, client'a tip bazlı HTTP durum kodu döner (404, 400, 403 vb.).
- **CORS**: Şu an tüm originlere açık (`AllowAll`). Production'da kısıtlanacak.

---

## Kurulum ve Çalıştırma

### Gereksinimler
- .NET 8 SDK
- SQL Server / LocalDB

### 1. Veritabanı Bağlantısı

`backend/KlinikRandevu.Api/KlinikRandevu/appsettings.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "sqlConnection": "Server=(localdb)\\mssqllocaldb;Database=KlinikRandevuDb;Trusted_Connection=True;"
  },
  "jwt": {
    "Key": "GIZLI_ANAHTARINIZ_EN_AZ_32_KARAKTER",
    "Issuer": "KlinikRandevuApi",
    "Audience": "KlinikRandevuClient"
  }
}
```

### 2. Migration Uygulama

```bash
cd backend/KlinikRandevu.Api/KlinikRandevu
dotnet ef database update
```

### 3. Çalıştırma

```bash
dotnet run --project backend/KlinikRandevu.Api/KlinikRandevu
```

Swagger arayüzü: `https://localhost:{port}/swagger`

---

## Proje Yapısı

```
KlinikRandevu/
└── backend/
    └── KlinikRandevu.Api/
        ├── KlinikRandevu/        # API Host (startup, extensions, migrations)
        ├── Entities/             # Domain modeller, DTO'lar, enum'lar, exception'lar
        ├── Repositories/         # EF Core implementasyonları
        ├── Services/             # İş mantığı
        └── Presentation/         # Controller'lar, action filter'lar
```

---

## Notlar

- UI henüz geliştirilmemiştir. Tüm endpointler Swagger üzerinden test edilebilir.
- Loglama: uygulama başladığında `logs/` klasörüne günlük dönen dosyalar yazılır (`log-YYYYMMDD.txt`).
- E-posta servisi şu an DI'a kayıtlı değildir (`ServicesExtensions.cs`'de yorum satırı). Aktif etmek için `IEmailService` kaydını açmak gerekir.

---

**Geliştirici:** Batuhan Köse
