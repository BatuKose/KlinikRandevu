# KlinikRandevu - Klinik Randevu Yönetim Sistemi

Klinik ortamlarında hasta kaydı, randevu planlaması, doktor çalışma programı ve muayene kaydı yönetimini sağlayan tam kapsamlı (full-stack) sistem. **Backend**: .NET 8 RESTful Web API. **Frontend**: React (Vite) ile geliştirilen tek sayfa uygulama (SPA).

```
KlinikRandevu/
├── Api/    ← Backend (.NET 8 Web API, 5 katmanlı mimari)
└── Ui/     ← Frontend (React + Vite SPA)
```

---

## Backend Teknolojileri

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

## Frontend Teknolojileri

| Teknoloji | Versiyon | Ne İşe Yarar? |
|---|---|---|
| **React** | 19.2.8 | UI kütüphanesi. Sayfa ve bileşenler fonksiyonel component + hook'larla yazılmıştır. |
| **Vite** | 8.3.0 | Geliştirme sunucusu ve build aracı. Varsayılan port: `2000`. |
| **React Router DOM** | 7.18.4 | Sayfa yönlendirme (`/login`, `/hasta-kayit`, `/randevu`, `/poliklinik`, `/sistem-yonetimi`). |
| **Axios** | 1.20.0 | API istekleri. JWT access token'ı otomatik ekleyen ve 401 durumunda refresh token ile yenileyen interceptor içerir. |
| **oxlint** | 1.81.0 | Linting. |

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
Bu controller randevu, muayene, teşhis/tedavi/ödeme, doktor çalışma planı ve yeşil liste akışlarının tamamını karşılar.

| Method | Endpoint | Açıklama |
|---|---|---|
| POST | `/calismaplaniolustur` | Doktor çalışma planı oluşturur. Slot validasyonları uygulanır. |
| POST | `/CalismaPlaniKopyala` | Bir doktorun çalışma planını başka bir tarihe kopyalar. |
| POST | `/bransBazliCalismaPlaniKopyala` | Aynı uzmanlık branşındaki doktorlara branş bazlı plan kopyalar. |
| POST | `/randevuolustur` | Randevu açar. Çakışma, slot, aynı gün kontrolü yapılır. |
| GET | `/randevularigetir?baslangic=&bitis=` | Tarih aralığındaki tüm randevuları listeler (Randevu modülü liste/takvim görünümü bunu kullanır). |
| GET | `/hastaninrandevusunugetir?protokol=` | Hastanın tüm randevularını listeler. |
| PATCH | `/randevuiptalet?id=` | Randevuyu iptal eder. |
| POST | `/muayeneolustur` | Muayene kaydı açar. |
| GET | `/muayeneGetir?id=` | Muayene kaydını ID ile getirir. |
| GET | `/muayeneRandevuIleGetir?randevuId=` | Randevuya bağlı muayene kaydını getirir. |
| PATCH | `/muayenebitis?id=` | Muayeneyi kapatır. |
| POST | `/{muayeneid}/teshisekle` | Muayeneye teşhis ekler. |
| GET | `/teshisleriGetir?muayeneId=` | Muayenenin teşhislerini listeler. |
| POST | `/tedaviEkle` | Muayeneye tedavi ekler. |
| GET | `/tedavileriGetir?muayeneId=` | Muayenenin tedavilerini listeler. |
| POST | `/odenemeYap` | Ödeme kaydı oluşturur. |
| PATCH | `/OdemeIptal` | Ödemeyi iade eder. |
| GET | `/odemeleriGetir?muayeneId=` | Muayenenin ödemelerini listeler. |
| GET | `/muayeneBorcGetir?muayeneId=` | Muayenenin kalan borcunu hesaplar. |
| GET | `/muayeneOdemeToplamGetir?muayeneId=` | Muayeneye yapılan toplam ödemeyi getirir. |
| GET | `/poliklinikHastaListesiGetir?polNo=&baslangic=&bitis=` | Poliklinik + tarih aralığına göre hasta/muayene listesi. |
| GET | `/doktorListesiGetir` | Aktif doktorları listeler. |
| GET | `/ServisListesiGetir` | Aktif poliklinikleri listeler. |
| PATCH | `/{doktorId}/docpasif` | Doktoru aktif/pasif yapar (ileri randevu varsa engeller). |
| PATCH | `/{polId}/polpasif` | Polikliniği aktif/pasif yapar. |
| POST | `/doktor/{doktorNo}/randevu-hatirlatma-mail` | Doktora günlük randevu programını e-posta ile gönderir. |
| POST | `/DoktorIzınEkle` | Doktora izin günü tanımlar. |
| POST | `/icdara` | ICD tanı kodu arama (harici ICD API'si üzerinden). |
| POST | `/taahütnameEKle` | Hastaya taahütname ekler. |
| POST | `/PoliklinikYesilAlanEKle` | Poliklinik bazlı yeşil liste kuralı tanımlar. |
| POST | `/HastaYesilAlanEKle` | Hastayı yeşil listeye ekler. |
| POST | `/randevuluHastalarinBilgileri` | Tarih aralığı + muayene durumuna göre randevulu hasta bilgilerini getirir. |

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

### Backend

#### Gereksinimler
- .NET 8 SDK
- SQL Server / LocalDB

#### 1. Veritabanı Bağlantısı

`Api/KlinikRandevu.Api/KlinikRandevu/appsettings.json` dosyasını düzenleyin:

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

#### 2. Migration Uygulama

```bash
cd Api/KlinikRandevu.Api/KlinikRandevu
dotnet ef database update
```

#### 3. Çalıştırma

```bash
dotnet run --project Api/KlinikRandevu.Api/KlinikRandevu
```

Swagger arayüzü: `https://localhost:{port}/swagger` (varsayılan: `https://localhost:1000/swagger`)

### Frontend

#### Gereksinimler
- Node.js 18+

#### 1. Bağımlılıkları Kurma

```bash
cd Ui
npm install
```

#### 2. API Adresi (opsiyonel)

Frontend varsayılan olarak `https://localhost:1000` adresine istek atar (`src/services/api.js`). Farklı bir adres kullanmak için `Ui/.env` dosyasına ekleyin:

```
VITE_API_URL=https://localhost:1000
```

#### 3. Çalıştırma

```bash
npm run dev
```

Uygulama `http://localhost:2000` üzerinde açılır. Backend çalışmıyorsa login ve veri çeken sayfalar hata verir.

---

## Proje Yapısı

```
KlinikRandevu/
├── Api/
│   └── KlinikRandevu.Api/
│       ├── KlinikRandevu.Api.sln
│       ├── KlinikRandevu/        # API Host (Program.cs, appsettings.json, extensions, migrations)
│       ├── Entities/             # Domain modeller, DTO'lar, enum'lar, exception'lar
│       ├── Repositories/         # EF Core implementasyonları
│       ├── Services/             # İş mantığı
│       └── Presentation/         # Controller'lar, action filter'lar
└── Ui/
    ├── src/
    │   ├── pages/                # Login, AnaSayfa, HastaKayit, Randevu, Poliklinik, SistemYonetimi
    │   ├── components/           # Sayfa bazlı alt bileşenler ve modaller (hastaKayit/, muayene/, randevu/)
    │   ├── services/             # Axios tabanlı API çağrıları (authService, hastaService, muayeneService, parametreService)
    │   ├── context/              # AuthContext (JWT + refresh token yönetimi)
    │   └── utils/                # Hata mesajı çözümleme, sabit seçenek listeleri
    └── package.json
```

### Frontend Sayfaları

| Sayfa | Yol | Açıklama |
|---|---|---|
| Login | `/login` | JWT ile giriş. |
| Ana Sayfa | `/` | Modül seçim ekranı. |
| Hasta Kayıt | `/hasta-kayit` | Hasta arama/oluşturma, randevu geçmişi, poliklinik kayıtları. |
| Randevu | `/randevu` | Randevu listesi (tarih/doktor/poliklinik/durum filtreli) ve haftalık takvim görünümü, yeni randevu oluşturma. |
| Poliklinik | `/poliklinik` | Poliklinik bazlı hasta listesi, muayene açma/kapama, teşhis, tedavi, ödeme işlemleri. |
| Sistem Yönetimi | `/sistem-yonetimi` | Sistem parametreleri, kullanıcı ve yetki yönetimi. |

---

## Notlar

- Loglama: uygulama başladığında `logs/` klasörüne günlük dönen dosyalar yazılır (`log-YYYYMMDD.txt`).
- E-posta servisi şu an DI'a kayıtlı değildir (`ServicesExtensions.cs`'de yorum satırı). Aktif etmek için `IEmailService` kaydını açmak gerekir.
- Frontend build çıktısı `Ui/dist` altında oluşur (`npm run build`); linting için `npm run lint` (oxlint) kullanılır.

---

**Geliştirici:** Batuhan Köse
