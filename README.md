# 🏥 KlinikRandevu

ASP.NET Core 8 Web API ile geliştirilmiş klinik randevu yönetim sistemi.

## 🚀 Teknolojiler

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
-javascript
- SQL Server
- Git

## 📁 Proje Yapısı

```
KlinikRandevu.Api/
│
├── KlinikRandevu/         # Ana API projesi (Presentation)
│   ├── Controllers/
│   ├── Extensions/
│   ├── Migrations/
│   └── Program.cs
│
├── Entities/              # Entity'ler ve modeller
│   ├── Models/
│   ├── Enums/
│   ├── Exceptions/
│   ├── View/
│   └── Data Transfer Objects/
│
├── Services/              # İş mantığı katmanı
│   └── Contracts/
│
├── Repositories/          # Veritabanı işlemleri
│   ├── Config/
│   ├── Contracts/
│   └── EFCore/
│
└── Presentation/          # Controller'lar
    └── Controllers/
```

## ⚙️ Kurulum

### Gereksinimler
- .NET 8 SDK
- SQL Server
- Visual Studio 2022

### Adımlar

1. Repoyu klonla:
```bash
git clone https://github.com/kullaniciadi/KlinikRandevu.git
cd KlinikRandevu
```

2. `appsettings.json` dosyasında veritabanı bağlantısını ayarla:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=KlinikRandevuDb;Trusted_Connection=True;"
  }
}
```

3. Migration'ları uygula:
```bash
dotnet ef database update
```

4. Projeyi çalıştır:
```bash
dotnet run
```

## 🌿 Branch Yapısı

| Branch | Açıklama |
|--------|----------|
| `main` | Stabil, production kodu |
| `develop` | Geliştirme branch'i |

## 📌 API Endpointleri

> Geliştirme aşamasında...

## 👤 Geliştirici

**Batuhan Köse**
