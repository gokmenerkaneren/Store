# 📁 STORE PROJESİ - DETAYLI DOSYA AĞACI VE KAYNAK KOD ANALİZİ
> **Hazırlayan:** Kıdemli PM + Kıdemli İş Analisti + Kıdemli Yazılım Mimarı + Kıdemli DBA + Kıdemli Siber Güvenlik Mühendisi + Kıdemli Test Mühendisi  
> **Tarih:** 05 Ağustos 2026  
> **Kaynak:** `ProjeAnalizRaporu_2026.08.05.md` + Canlı Kaynak Kod İncelemesi  
> **Kapsam:** `bin/`, `obj/`, `.vs/`, `.git/`, `publish/`, `Migrations/` klasörleri özetlenmiş/hariç tutulmuştur.

---

## 📋 İÇİNDEKİLER

1. [Kök Dizin (Store/) - Belge ve Yardımcı Dosyalar](#1-kök-dizin-store---belge-ve-yardımcı-dosyalar)
2. [Store.slnx - Çözüm Yapısı](#2-storeslnx---çözüm-yapısı)
3. [FrontEnds/ - Kullanıcı Arayüzü Katmanı](#3-frontends---kullanıcı-arayüzü-katmanı)
4. [IdentityServer/ - Kimlik ve Yetkilendirme Servisi](#4-identityserver---kimlik-ve-yetkilendirme-servisi)
5. [Services/ - Mikroservisler](#5-services---mikroservisler)
   - 5.1 [Services/B2B/ - Toptan Ticaret & Bayi Finans Servisi (YENİ 05.08.2026)](#51-servicesb2b---toptan-ticaret--bayi-finans-servisi-yeni-05082026)
   - 5.2 [Services/Catalog/ - Ürün Katalog Servisi](#52-servicescatalog---ürün-katalog-servisi)
   - 5.3 [Services/Discount/ - İskonto ve Kupon Servisi](#53-servicesdiscount---iskonto-ve-kupon-servisi)
   - 5.4 [Services/Location/ - Konum ve Adres Servisi & Testler](#54-serviceslocation---konum-ve-adres-servisi--testler)
   - 5.5 [Services/Order/ - Sipariş ve Teslimat Servisi](#55-servicesorder---sipariş-ve-teslimat-servisi)
6. [Store.DtoLayer/ - Paylaşımlı DTO Kütüphanesi](#6-storedtolayer---paylaşımlı-dto-kütüphanesi)
7. [docker-compose.yml & Altyapı Tanımları](#7-docker-composeyml--altyapı-tanımları)
8. [Mimari Bağımlılık Haritası](#8-mimari-bağımlılık-haritası)
9. [Dosya Sağlığı ve Teknik Borç Özeti (05.08.2026)](#9-dosya-sağlığı-ve-teknik-borç-özeti-05082026)

---

## 1. Kök Dizin (Store/) - Belge ve Yardımcı Dosyalar

Kök dizin proje dokümantasyonu, PowerShell otomasyon scriptleri ve geliştirici kılavuzlarından oluşur.

| Dosya | Rol | Açıklama |
|:---|:---|:---|
| `Store.slnx` | Çözüm Dosyası | Tüm projeleri bir arada tutan Visual Studio çözüm dosyası (Yeni .slnx XML formatı). |
| `Store.slnLaunch.user` | IDE Ayarları | Visual Studio çoklu proje başlatma profili. |
| `agent.md` | Geliştirici Bağlamı | AI asistanının projeyi anlayabilmesi için hazırlanan bağlam dosyası (Tech stack, 6-şapka rolleri, bilinen borçlar). |
| `project_context.md` | Proje Özeti | İş gereksinimleri ve teknik mimari özeti. |
| `ProjeAnalizRaporu_2026.08.05.md` | 6-Şapka Analizi | 05.08.2026 tarihli güncel 6 uzman perspektifinden kapsamlı analiz raporu. |
| `ProjeDosyaAgaci_2026.08.05.md` | Bu Dosya | Projedeki tüm dosya ve klasörlerin detaylı mimari ağacı. |
| `Yapilanlar_2026.08.05.md` | Günlük Değişiklik | 05.08.2026 tarihli B2B servisi ayrımı ve Location refactoring özeti. |
| `Islemler_2026.08.03.md` | İşlem Günlüğü | 03.08.2026 tarihli sıralama, DataTables Server-Side ve Soft-Delete soru-cevap raporu. |
| `YapilanIslemler_20260801.md` | İşlem Günlüğü | 01.08.2026 tarihli Location Domain refactoring ve TPC AddressBase raporu. |
| `GelistirilebilirYanlar_2026.07.30.md` | İyileştirme Listesi | Mimari, güvenlik ve performans iyileştirme alanları. |
| `VeriTabani_01.md` / `02.md` | VT Dokümantasyonu | MongoDB, PgSQL ve MSSQL veritabanı şema ve view detayları. |
| `FixCsproj.ps1` | PowerShell Script | Proje referanslarını ve eksik kütüphaneleri otomatik onaran script. |
| `FixMissingViews.ps1` | PowerShell Script | Eksik Razor View (.cshtml) dosyalarını tespit ve onarım script'i. |
| `RegenerateAllWebUI.ps1` | PowerShell Script | Admin panel Controller ve View yapılarını yeniden oluşturan otomasyon. |
| `PortNumbers.txt` | Port Rehberi | Mikroservislerin çalıştığı HTTP/HTTPS port listesi. |

---

## 2. Store.slnx - Çözüm Yapısı

`Store.slnx` visual studio çözüm dosyasındaki mantıksal klasör (solution folder) yapısı:

```
Store.slnx
├── 📁 Solution Items (Dokümanlar ve Scriptler)
├── 📁 FrontEnds
│   └── 🌐 Store.WebUI
├── 📁 IdentityServer
│   └── 🔐 Store.IdentityServer
├── 📁 Services
│   ├── 📁 B2B (YENİ 05.08.2026)
│   │   ├── Store.B2B.Domain
│   │   ├── Store.B2B.Application
│   │   ├── Store.B2B.Persistence
│   │   └── Store.B2B.WebApi
│   ├── 📁 Catalog
│   │   └── Store.Catalog
│   ├── 📁 Discount
│   │   └── Store.Discount
│   ├── 📁 Location
│   │   ├── Store.Location.Domain
│   │   ├── Store.Location.Application
│   │   ├── Store.Location.Persistence
│   │   ├── Store.Location.WebApi
│   │   └── 🧪 Store.Location.Tests
│   └── 📁 Order
│       ├── Store.Order.Domain
│       ├── Store.Order.Application
│       ├── Store.Order.Persistance
│       └── Store.Order.WebApi
└── 📁 Libraries
    └── 📦 Store.DtoLayer
```

---

## 3. FrontEnds/ - Kullanıcı Arayüzü Katmanı

```
FrontEnds/Store.WebUI/
├── 📄 Program.cs                   # IoC, HttpClient, Authentication ve Middleware konfigürasyonu
├── 📄 appsettings.json              # Servis URL'leri ve API endpoint tanımları
├── 📁 Areas/
│   └── 📁 Admin/                   # Yönetim Paneli
│       ├── 📁 Controllers/         # Country, City, District, Neighborhood, Dealer, Warehouse vb.
│       └── 📁 Views/               # Sekmeli (Tabbed) UI, Modal formlar, Razor bileşenleri
├── 📁 Services/                    # Microservice HTTP Client servisleri (LocationService, CatalogService vb.)
├── 📁 Models/                      # ViewModel ve View DTO eşleşmeleri
└── 📁 wwwroot/                     # Statik dosyalar (Bootstrap, DataTables, Custom JS & CSS)
```

---

## 4. IdentityServer/ - Kimlik ve Yetkilendirme Servisi

```
IdentityServer/Store.IdentityServer/
├── 📄 Program.cs                   # OAuth 2.0 / OpenID Connect konfigürasyonu
├── 📄 Config.cs                    # ApiResources, ApiScopes, Clients ve IdentityResources
├── 📄 appsettings.json              # PostgreSQL (StoreIdentityDB) bağlantı cümlesi
├── 📄 tempkey.jwk                  # Geliştirme ortamı JWT imzalama anahtarı (KRİTİK GÜVENLİK RİSKİ)
├── 📁 Configs/                     # EF Core Identity konfigürasyon sınıfları
└── 📁 Quickstart/                  # Login, Logout, Consent ve TestUsers bileşenleri
```

---

## 5. Services/ - Mikroservisler

### 5.1 Services/B2B/ - Toptan Ticaret & Bayi Finans Servisi (YENİ 05.08.2026)

DDD Bounded Context ayrıştırması sonrasında 05.08.2026'da oluşturulmuştur.

```
Services/B2B/
├── 📁 Core/
│   ├── 📁 Store.B2B.Domain/
│   │   ├── 📄 BaseEntity.cs         # Id, CreatedDate, UpdatedDate, IsDeleted
│   │   └── 📁 Entities/
│   │       └── 📄 DealerFinancial.cs# DealerId, TaxNumber (UNIQUE), TaxOffice, DiscountRate, CreditLimit, CurrentBalance
│   └── 📁 Store.B2B.Application/    # CQRS MediatR Command/Query ve DTO interfaces
├── 📁 Infrastructure/
│   └── 📁 Store.B2B.Persistence/
│       ├── 📁 Context/
│       │   └── 📄 B2BContext.cs     # DbContext (PostgreSQL - StoreB2BDb), Unique Index Konfigürasyonu
│       └── 📁 Migrations/
│           └── 📄 20260805152155_InitialB2BDb_20260805.cs
└── 📁 Presentation/
    └── 📁 Store.B2B.WebApi/
        ├── 📄 Program.cs            # PostgreSQL IoC kaydı
        └── 📄 appsettings.json       # Host=localhost;Database=StoreB2BDb;...
```

### 5.2 Services/Catalog/ - Ürün Katalog Servisi

```
Services/Catalog/ Store.Catalog/ (Monolitik Yapıda tek proje)
├── 📄 Program.cs                   # MongoDB Settings, Service Registration, SSL Bypass (KRİTİK)
├── 📄 appsettings.json              # DatabaseName: StoreMD, ConnectionString
├── 📁 Entities/                    # Product, Category, Brand, ProductImage, ProductPrice, Inventory
├── 📁 Services/                    # CategoryService, ProductService vb. (Direct Mongo Driver / Repositories)
└── 📁 Controllers/                 # ProductsController, CategoriesController vb.
```

### 5.3 Services/Discount/ - İskonto ve Kupon Servisi

```
Services/Discount/ Store.Discount/ (EF Core + Dapper Hibrit)
├── 📄 Program.cs                   # MSSQL IoC kaydı, Dapper Context
├── 📄 appsettings.json              # MSSQL LocalDB StoreDiscountDb
├── 📁 Entities/                    # Coupon (Code, Rate, IsActive, ValidDate)
├── 📁 Services/                    # DiscountService (Dapper ile yüksek hızlı SQL sorguları)
└── 📁 Controllers/                 # DiscountsController
```

### 5.4 Services/Location/ - Konum ve Adres Servisi & Testler

```
Services/Location/
├── 📁 Core/
│   ├── 📁 Store.Location.Domain/
│   │   ├── 📄 AddressBase.cs        # TPC Base Sınıfı (CountryId, NeighborhoodId?)
│   │   └── 📁 Entities/            # Country, City, District, Neighborhood, Branch, Warehouse, Dealer, DealerAddress, ContactInfo
│   └── 📁 Store.Location.Application/
│       ├── 📁 Interfaces/          # IRepository (GetAllByFilterAsync desteği)
│       └── 📁 Features/CQRS/       # Commands, Queries, Handlers (MediatR)
├── 📁 Infrastructure/
│   └── 📁 Store.Location.Persistence/
│       ├── 📄 LocationContext.cs    # TPC Mapping, Soft Delete Global Filters, PostgreSQL Enum Conversions
│       └── 📁 Migrations/          # InitialLocationDb_20260801, RemoveDealerFinancialFields_20260805
├── 📁 Presentation/
│   └── 📁 Store.Location.WebApi/   # REST Controllers (CountriesController, DealersController vb.)
└── 📁 Tests/
    └── 📁 Store.Location.Tests/     # xUnit Test Projesi (DealerAddressControllerTests, WarehouseControllerTests)
```

### 5.5 Services/Order/ - Sipariş ve Teslimat Servisi

```
Services/Order/
├── 📁 Core/
│   ├── 📁 Store.Order.Domain/
│   │   └── 📁 Entitties/ (YAZIM HATASI)# Address, OrderDetail, Ordering
│   └── 📁 Store.Order.Application/ # CQRS Handlers, IRepository, IUserContext
├── 📁 Infrastructure/
│   └── 📁 Store.Order.Persistance/ (YAZIM HATASI) # OrderContext (MSSQL StoreOrderDb)
└── 📁 Presentation/
    └── 📁 Store.Order.WebApi/       # OrderingsController, OrderDetailsController
```

---

## 6. Store.DtoLayer/ - Paylaşımlı DTO Kütüphanesi

```
Store.DtoLayer/
├── 📁 CatalogDtos/                 # BrandDtos, CategoryDtos, ProductDtos, ProductPriceDtos vb.
├── 📁 CommonDtos/                  # DataTablesRequest, DataTablesResponse (Server-Side Pagination & Order)
└── 📁 LocationDtos/                # CountryDtos, CityDtos, DistrictDtos, NeighborhoodDtos, DealerDtos, DealerAddressDtos, WarehouseDtos
```

---

## 7. docker-compose.yml & Altyapı Tanımları

```yaml
version: '3.8'
services:
  catalogdb:
    image: mongo:latest
    ports: ["27017:27017"]
  locationdb:
    image: postgres:16
    ports: ["5432:5432"]
  orderdb:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports: ["1433:1433"]
```
*(Not: Uygulama servisleri için Dockerfile tanımları henüz eklenmemiştir.)*

---

## 8. Mimari Bağımlılık Haritası

```
MEVCUT BAĞIMLILIK İHLALİ:
Store.Catalog ──(ProjectReference)──> Store.Location.Domain  [İHLAL! Düzeltilmeli]

HEDEF MİMARİ BAĞIMLILIK (Store.Shared):
Store.Catalog ──> Store.Shared
Store.Location ──> Store.Shared
Store.B2B ──> Store.Shared
Store.Order ──> Store.Shared
Store.Discount ──> Store.Shared
```

---

## 9. Dosya Sağlığı ve Teknik Borç Özeti (05.08.2026)

| # | Dosya / Konum | Sorun / Durum | Eylem Planı | Durum |
|:-:|:---|:---|:---|:---:|
| 1 | `Store.Catalog/Program.cs` | SSL Bypass (`ServerCertificate...`) | `#if DEBUG` şartına al | 🔴 Acil |
| 2 | `Store.WebUI/Program.cs` | SSL Bypass (`ServerCertificate...`) | `#if DEBUG` şartına al | 🔴 Acil |
| 3 | `IdentityServer/tempkey.jwk` | Güvenlik riski | `.gitignore`'a ekle, X.509 kullan | 🔴 Acil |
| 4 | `Store.Catalog.csproj` | Catalog -> Location.Domain bağımlılığı | `Store.Shared` oluşturup ayır | 🔴 Acil |
| 5 | `Store.Location/Domain/Dealer.cs` | Bounded Context karmaşası | Finansal alanlar `Store.B2B`'ye taşındı | 🟢 TAMAMLANDI |
| 6 | `Store.B2B` Mikroservisi | Yeni servis ihtiyacı | 4 katmanlı B2B servisi ve DB kuruldu | 🟢 TAMAMLANDI |
| 7 | `Store.Location/Entities/Warehouse.cs` | Serbest metin adres karmaşası | `WarehouseAddress` & `ContactInfo` entegre | 🟢 TAMAMLANDI |
| 8 | `Store.Order.Domain/Entitties/` | Klasör yazım hatası | `Entities` olarak yeniden adlandır | 🟡 Orta |
| 9 | `Store.Order.Persistance` | Proje yazım hatası | `Persistence` olarak yeniden adlandır | 🟡 Orta |
| 10| `Store.Location.Tests` | Eksik test coverage | Test projesi açıldı, 7/7 yeşil test | 🟢 TAMAMLANDI |

---

> **Hazırlayan:** 6 Şapkalı Uzman Ekibi (Antigravity)  
> **Tarih:** 05 Ağustos 2026  
> **Dosya Adı:** `ProjeDosyaAgaci_2026.08.05.md`  
> **Durum:** ✅ Tamamlandı & Güncellendi
