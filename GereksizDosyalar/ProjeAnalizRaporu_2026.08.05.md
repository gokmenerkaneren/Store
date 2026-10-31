# 🚀 STORE PROJESİ - 6 ŞAPKALI KAPSAMLI VE PROFESYONEL ANALİZ RAPORU
> **Hazırlayan:** Kıdemli PM + Kıdemli İş Analisti + Kıdemli Yazılım Mimarı + Kıdemli DBA + Kıdemli Siber Güvenlik Mühendisi + Kıdemli Test Mühendisi  
> **Tarih:** 05 Ağustos 2026  
> **Rapor Sürümü:** v2.0 (05.08.2026 B2B Servisi ve DDD Refactoring Sonrası Güncel Durum)  
> **Kapsam:** `Store.sln` (.slnx) Tüm Mikroservisler, Veritabanları, Güvenlik ve Arayüz Katmanı  
> **Hedef Kitle:** Proje Sahibi, Ortaklar, Takım Liderleri ve Projeye Yeni Katılan Yazılım Geliştiriciler  

---

## 📋 İÇİNDEKİLER

1. [Projeye Genel Bakış - Yönetici Özeti](#1-projeye-genel-bakış---yönetici-özeti)
2. [05.08.2026 Tarihli Büyük Dönüşüm ve B2B Servisi Ayrımı](#2-05082026-tarihli-büyük-dönüşüm-ve-b2b-servisi-ayrımı)
3. [🧢 ŞAPKA 1 - Kıdemli Project Manager Perspective](#3--şapka-1---kıdemli-project-manager-perspective)
4. [🧢 ŞAPKA 2 - Kıdemli İş Analisti Perspective](#4--şapka-2---kıdemli-iş-analisti-perspective)
5. [🧢 ŞAPKA 3 - Kıdemli Yazılım Mimarı Perspective](#5--şapka-3---kıdemli-yazılım-mimarı-perspective)
6. [🧢 ŞAPKA 4 - Kıdemli DBA (Veritabanı Yöneticisi) Perspective](#6--şapka-4---kıdemli-dba-veritabanı-yöneticisi-perspective)
7. [🧢 ŞAPKA 5 - Kıdemli Siber Güvenlik Mühendisi Perspective](#7--şapka-5---kıdemli-siber-güvenlik-mühendisi-perspective)
8. [🧢 ŞAPKA 6 - Kıdemli Test Mühendisi Perspective](#8--şapka-6---kıdemli-test-mühendisi-perspective)
9. [Birleşik Öncelik Tablosu ve Stratejik Yol Haritası](#9-birleşik-öncelik-tablosu-ve-stratejik-yol-haritası)

---

## 1. Projeye Genel Bakış - Yönetici Özeti

**Store.sln**, organizasyona ait fiziksel ürünlerin hem **satışını** hem de **kiralanmasını** aynı mimari çatı altında yöneten, enterprise ölçekte, yüksek performanslı ve mikroservis mimarisine dayalı bir **E-Ticaret, B2B ve MRP (Malzeme İhtiyaç Planlaması / Lojistik) Platformudur.**

Projeyi yeni tanıyacak bir paydaş veya geliştirici için platformun temel yetenekleri şu şekildedir:

### Platformun Ticari Modelleri

| İş Modeli | Açıklama | Örnek Senaryo | Durum |
|:---|:---|:---|:---:|
| **B2C Satış** | Bireysel müşterinin ürünü sepete ekleyip satın alması. | Müşteri bir çalışma koltuğu satın alır. | 🟢 Aktif |
| **B2C Kiralama** | Bireysel müşterinin ürünü belirli tarih aralığında kiralaması. | Organizasyon için 3 günlüğüne ses sistemi kiralanır. | 🟡 Kodlanıyor |
| **B2B Toptan Satış** | Anlaşmalı bayi/firmaların özel iskonto ve kredi limitleriyle toplu alımı. | Bayi 100 adet mobilya siparişi verir. | 🟢 Aktif (DDD Refactored) |
| **B2B Toptan Kiralama** | Bayilerin dönemsel kurumsal kiralama yapması. | Etkinlik firması 1 ay için sahne ekipmanı kiralar. | 🟡 Kodlanıyor |

### Teknoloji Yığını (Tech Stack) Özeti

```
[ Frontend: Store.WebUI (ASP.NET Core 8 MVC + DataTables Server-Side) ]
                                |
                                v
               [ IdentityServer4 (OAuth 2.0 / OpenID) ]
                                |
        +-----------------------+-----------------------+
        |                       |                       |
        v                       v                       v
[ Store.Catalog ]       [ Store.Location ]        [ Store.B2B ]
(MongoDB 7.x)         (PostgreSQL 16.x)       (PostgreSQL 16.x)
        |                       |                       |
        +-----------------------+-----------------------+
                                |
                                v
                [ Store.Order ]     [ Store.Discount ]
                (MSSQL LocalDB)       (MSSQL LocalDB)
```

| Katman | Kullanılan Teknolojiler & Kütüphaneler |
|:---|:---|
| **Framework & Çalışma Zamanı** | .NET 8.0 (C# 12) |
| **Mimari Yaklaşım** | Microservices Architecture, Clean Architecture, Onion Architecture |
| **Tasarım Desenleri** | CQRS (Command Query Responsibility Segregation), MediatR, Repository Pattern, TPC (Table-Per-Concrete) |
| **Kimlik Doğrulama & Yetki** | IdentityServer4, OAuth 2.0, OpenID Connect, JWT Bearer Tokens |
| **Veritabanları (Polyglot)** | MongoDB 7.x (Catalog), PostgreSQL 16.x (Location & B2B), MSSQL LocalDB (Order & Discount) |
| **ORM & Veri Erişim** | Entity Framework Core 8.0, Dapper (Discount servisinde yüksek performans için) |
| **Loglama & İzleme** | Serilog (Structured Logging), File Sink |
| **Doğrulama (Validation)** | FluentValidation |
| **Gelişmiş Arayüz (UI)** | ASP.NET Core MVC, Bootstrap, Dynamic DataTables (Server-Side & Custom Multi-Column Order) |

---

## 2. 05.08.2026 Tarihli Büyük Dönüşüm ve B2B Servisi Ayrımı

05 Ağustos 2026 tarihi projenin mimari olgunluğunda bir **milat** olmuştur. Bu tarihte Domain Driven Design (DDD) ve Bounded Context (Sınırlı Bağlam) ilkeleri gereğince büyük bir mimari ayrıştırma (refactoring) gerçekleştirilmiştir:

### 1. `Store.Location` Servisinden Finansal Verilerin Temizlenmesi
Daha önce coğrafi ve fiziki lokasyon yönetimi yapan `Store.Location` servisi içindeki `Dealer` (Bayi) entity'sinde ticari/finansal alanlar karmaşaya yol açmaktaydı.
- `TaxNumber`, `TaxOffice`, `DiscountRate`, `CreditLimit` ve `CurrentBalance` alanları `Dealer` sınıfından tamamen çıkarılmıştır.
- `Store.Location` sadece coğrafi varlıklara (Ülke, Şehir, İlçe, Mahalle, Şube, Depo ve Bayi Fiziki İletişim/Adres) odaklanmıştır.
- `CreateDealerCommandHandler`, `UpdateDealerCommandHandler`, `GetDealerByIdQueryHandler` ve `GetDealersQueryHandler` CQRS sınıfları bu alanlardan temizlenmiştir.
- `DuplicateCheckerHelper` vergi numarası kontrolü yerine şirket adı tabanlı sadeleştirilmiştir.

### 2. `Store.B2B` Mikroservisinin Doğuşu
Bayilerin ticari sınırlarını, limitlerini ve finansal anlaşmalarını bağımsız bir Bounded Context altında yönetmek için **yepyeni bir mikroservis** kurulmuştur:
- **Proje Yapısı:** `Services/B2B` dizini altında `Store.B2B.Domain`, `Store.B2B.Application`, `Store.B2B.Persistence` ve `Store.B2B.WebApi` projeleri oluşturulup `Store.slnx` çözüm dosyasına eklenmiştir.
- **Veritabanı:** PostgreSQL üzerinde `StoreB2BDb` adıyla müstakil bir veritabanı oluşturulmuştur.
- **Domain Entity:** `DealerFinancial` sınıfı tasarlanmış; `TaxNumber` alanı PostgreSQL seviyesinde `IsUnique()` indeksi ile benzersiz kılınmıştır.
- **Migration:** `InitialB2BDb_20260805` migration'ı uygulanmış ve veritabanı canlıya hazır hale getirilmiştir.

### 3. Depo ve Adres Normalizasyonu (Warehouse Normalization)
- `Warehouse` (Depo) sınıfındaki serbest metin `Address`, `City`, `Phone` alanları silinmiş; `ContactInfoId` ve `WarehouseAddress` ilişkisiyle veritabanı 3NF normalizasyonuna ulaştırılmıştır.
- Veritabanında silinen alanları referans alan `vw_dealer_full_details` PostgreSQL view'ı başarıyla güncellenmiş ve `RemoveDealerFinancialFields` migration'ı sorunsuz uygulanmıştır.

---

## 3. 🧢 ŞAPKA 1 - Kıdemli Project Manager Perspective

> *"Bir Project Manager olarak projenin teslim tarihlerine, bütçe/efor dengesine, takım çalışma modeline ve projenin canlıya çıkış engelleyicilerine (blockers) odaklanıyorum."*

```
                       PROJE OLGUNLUK SEVİYESİ (%68)
[█████████████████████████████████████████░░░░░░░░░░░░░░░░░]
  Temel Servisler: %90 | Mimari Olgunluk: %80 | Güvenlik: %45 | Test: %25
```

### 3.1 Projenin Anlık Durumu ve Tamamlanma Oranı

Projemiz şu an **Geliştirme ve Entegrasyon (Development & Integration)** aşamasındadır.
- **Çözümdeki Toplam Proje Sayısı:** 15 Proje (Servisler, IdentityServer, WebUI, DtoLayer, Test)
- **Derleme Durumu (Build Status):** `Store.slnx` çözümü **0 Hata (0 Errors)** ve **0 Uyarı (0 Warnings)** ile derlenmektedir.
- **Veritabanı Durumu:** 4 Farklı Veritabanı Sunucusu/İstisnai DB aktif çalışmaktadır (MongoDB, PgSQL Location, PgSQL B2B, MSSQL Order & Discount).

### 3.2 Proje Açık İş Listesi (Backlog) & Risk Matrisi

| İş / Görev Adı | Öncelik | Kategori | Bloker mı? | Tahmini Efor |
|:---|:---:|:---:|:---:|:---:|
| **SSL Bypass Açıklarının Kapatılması** | 🔴 KRİTİK | Siber Güvenlik | **EVET** | 0.5 Gün |
| **`appsettings.json` Bağlantı Cümlelerinin Şifrelenmesi** | 🔴 KRİTİK | Siber Güvenlik | **EVET** | 1 Gün |
| **Catalog -> Location.Domain Bağımlılığının Koparılması** | 🔴 KRİTİK | Mimari Borç | **EVET** | 1 Gün |
| **Rental (Kiralama) İş Akışlarının Kodlanması** | 🟡 YÜKSEK | İş Mantığı | Hayır | 4 Gün |
| **Server-Side DataTables Mimarisinin Tüm Ekranlara Yayılması** | 🟡 YÜKSEK | Performans | Hayır | 3 Gün |
| **`Store.Order.Domain` Klasör/Namespace Yazım Hatası (`Entitties`)** | 🔵 ORTA | Kod Kalitesi | Hayır | 0.5 Gün |
| **Docker-Compose ve Container Environment Ayarları** | 🔵 ORTA | DevOps | Hayır | 2 Gün |
| **RabbitMQ Asenkron Mesajlaşma Altyapısının Kurulması** | 🔵 ORTA | Mimari | Hayır | 5 Gün |
| **Unit & Integration Test Kapsamının Artırılması** | 🔵 ORTA | Kalite / Test | Hayır | Sürekli |

### 3.3 Takım Organizasyonu ve Paralel Çalışma Modeli

Mikroservis mimarisi sayesinde farklı yazılımcı ekipleri birbirini engellemeden çalışabilir:

```
+-----------------------------------------------------------------------+
|                         EKİP BÖLÜMLENMESİ                             |
+------------------------------------+----------------------------------+
| 📦 KATALOG & B2B TAKIMI            | 🛒 SİPARİŞ & İSKONTO TAKIMI      |
| Servisler: Catalog API, B2B API    | Servisler: Order API, Discount   |
| Odak: Ürün, Kategori, Fiyat, Bayi  | Odak: Sepet, Ödeme, İskonto,     |
| Finansal Limitleri                 | Sipariş Geçmişi, Kiralama        |
+------------------------------------+----------------------------------+
| 📍 LOKASYON & ARAYÜZ TAKIMI        | 🛡️ PLATFORM & DEVOPS TAKIMI     |
| Servisler: Location API, WebUI     | Servisler: IdentityServer, Shared|
| Odak: Coğrafi Veri, Adres, Şube,   | Odak: OAuth2, SSL, Gateway, CI/CD|
| Admin Panel & DataTables           | Docker, RabbitMQ                 |
+------------------------------------+----------------------------------+
```

---

## 4. 🧢 ŞAPKA 2 - Kıdemli İş Analisti Perspective

> *"Bir İş Analisti olarak sistemin iş gereksinimlerine, B2C/B2B iş akışlarına, kiralama mekaniğine ve veri bütünlüğüne odaklanıyorum."*

### 4.1 Temel İş Akışları (Business Workflows)

#### 1. B2B Toptan Ticaret & İskonto/Limit Akışı (05.08.2026 Güncel)

```
[ Bayi Girişi (WebUI) ] ──> [ IdentityServer4 (Token Alımı) ]
                                      │
                                      ▼
                         [ Store.B2B API Kontrolü ]
                                      │
          ┌───────────────────────────┴───────────────────────────┐
          ▼                                                       ▼
[ Kredi Limiti / Risk Kontrolü ]                      [ Bayi İskonto Oranı (%X) ]
(DealerFinancial.CreditLimit >= Order)                (DealerFinancial.DiscountRate)
          │                                                       │
          └───────────────────────────┬───────────────────────────┘
                                      ▼
                         [ Sipariş Onayı (Order API) ]
```

#### 2. Kiralama (Rental) İş Modeli Mantığı (Geliştirme Aşamasında)

Sistemde `PriceType` enum yapısında **Rental (3)** ve **WholeRental (5)** tanımlıdır.
- **Kiralama Süreç Kuralları:**
  - Ürünün kiralanabilir stok miktarı (Inventory) ayrı takip edilir.
  - Fiyatlandırma gün/hafta/ay bazlı katsayılar ile `ProductPrices` koleksiyonundan çekilir.
  - Kiralama başlangıç ve bitiş tarihleri arasında takvim çakışması (Overlapping Check) yapılmalıdır.
  - Ürün iade edildiğinde "Hasar / Kontrol" süreci işletilip depoya (Warehouse) kabul verilir.

#### 3. Silinen Kayıtların Yönetimi (Soft-Delete & Restore Akışı)

Projede veriler fiziksel olarak silinmez (`IsDeleted = true`).
- **Sistem Mantığı:** 
  - Standart kullanıcılara (B2C Müşteri, Standart Bayi) sadece `IsDeleted = false` olan veriler sunulur.
  - Admin kullanıcılarına yönetim panelinde "Silinen Kayıtları Göster" filtresi sunulur.
  - API katmanında `showDeleted=true` parametresi geldiğinde EF Core `IgnoreQueryFilters()` çağrılarak silinmiş veriler de listelenir ve gerektiğinde "Geri Yükle (Restore)" işlemi yapılır.

---

## 5. 🧢 ŞAPKA 3 - Kıdemli Yazılım Mimarı Perspective

> *"Bir Yazılım Mimarı olarak Clean Architecture ilkelerine, servis sınırlarına (Bounded Context), CQRS/MediatR desenlerine ve bileşenler arası loose-coupling (gevşek bağlılık) disiplinine odaklanıyorum."*

### 5.1 Mimari Tasarım Tablosu ve Katmanlar

Sistem **Clean Architecture** (Onion Architecture) standartlarına tam uyumludur:

```
[ Presentation Layer ]  --> WebApi / Controllers / DataTables
        │
        ▼
[ Application Layer ]   --> CQRS Commands/Queries, MediatR Handlers, DTOs, FluentValidation
        │
        ▼
[ Domain Layer ]        --> BaseEntity, Entities, Enums, Value Objects (Bağımsız Çekirdek)
        ▲
        │
[ Persistence Layer ]   --> DbContext, EF Core Configurations, Migrations, Repositories
```

### 5.2 Mimari Borç ve Bağımlılık İncelemesi

> [!CAUTION]
> **Kritik Mimari İhlal (Catalog -> Location Bağımlılığı):**
> `Store.Catalog.csproj` dosyası, `Store.Location.Domain` projesine doğrudan referans vermektedir (`ProjectReference`). Mikroservislerin bağımsız dağıtılabilirliği (independent deployability) ilkesine göre bir mikroservis başka bir mikroservisin domain katmanına referans veremez!
> 
> **Çözüm Yolu:** `Store.Shared` adlı ortak bir class library oluşturulup `BaseEntity`, `IAuditable`, ortak enum'lar buraya taşınmalı; `Store.Catalog`'un `Location.Domain` bağımlılığı derhal kesilmelidir.

### 5.3 Mevcut Yazım ve İsimlendirme Hataları (Naming Debt)

1. `Store.Order.Domain` projesinde `Entities` klasörü yanlışlıkla `Entitties` (çift 't' ile) olarak adlandırılmıştır.
2. `Store.Order.Persistance` projesinde `Persistence` kelimesi `Persistance` şeklinde yanlış yazılmıştır.
3. `Order.WebApi` içindeki `AdressesController.cs` dosyasında `AddressesController` yazım hatası mevcuttur.

---

## 6. 🧢 ŞAPKA 4 - Kıdemli DBA Perspective

> *"Bir DBA olarak veri bütünlüğüne, veritabanı seçimlerinin doğruluğuna, indeksleme stratejilerine, PostgreSQL View yapılarına ve Polyglot Persistence maliyetine odaklanıyorum."*

### 6.1 Polyglot Persistence (Çoklu Veritabanı) Mimarisi

Proje 3 farklı veritabanı teknolojisini hibrit olarak kullanmaktadır:

| Veritabanı | Servis / Katman | Veri Tipi / Yapı | Neden Seçildi? |
|:---|:---|:---|:---|
| **MongoDB 7.x** | `Store.Catalog` | NoSQL / Document Store | Esnek ürün nitelikleri (Attributes, Colors, Dimensions) ve yüksek okuma hızı. |
| **PostgreSQL 16.x** | `Store.Location` | Relational / Relational DB | Hiyerarşik coğrafi veriler (Ülke->Şehir->İlçe->Mahalle) ve ilişkisel adres yapısı. |
| **PostgreSQL 16.x** | `Store.B2B` | Relational / Relational DB | Bayi finansal verileri, benzersiz vergi numaraları (`TaxNumber` UNIQUE Index). |
| **MSSQL LocalDB** | `Store.Order` | Relational / Relational DB | ACID garantili sipariş, ödeme ve finansal hareket takibi. |
| **MSSQL LocalDB** | `Store.Discount` | Relational / Relational DB | Dapper destekli yüksek hızlı kupon ve indirim hesaplama motoru. |

### 6.2 Veritabanı İndeksleme ve PostgreSQL View Yapısı

#### `vw_dealer_full_details` PostgreSQL View (05.08.2026 Güncellendi)
Bayi listeleme ve raporlama ekranlarında 5 farklı tabloyu JOIN etmek yerine veritabanı seviyesinde compile edilmiş güncel view script'i:

```sql
CREATE OR REPLACE VIEW vw_dealer_full_details AS
SELECT 
    d."Id" AS "DealerId",
    d."CompanyName",
    c."CompanyEmail",
    c."Phone" AS "CompanyPhone",
    c."AuthorizedPersonName",
    c."AuthorizedPersonPhone",
    da."AddressDetail",
    da."Title" AS "AddressTitle",
    ct."CityName",
    dt."DistrictName",
    cn."CountryName"
FROM "Dealers" d
LEFT JOIN "ContactInfos" c ON d."ContactInfoId" = c."Id" AND c."IsDeleted" = false
LEFT JOIN "DealerAddresses" da ON da."DealerId" = d."Id" AND da."AddressType" = 1 AND da."IsDeleted" = false
LEFT JOIN "Cities" ct ON da."CityId" = ct."Id"
LEFT JOIN "Districts" dt ON da."DistrictId" = dt."Id"
LEFT JOIN "Countries" cn ON da."CountryId" = cn."Id"
WHERE d."IsDeleted" = false;
```

---

## 7. 🧢 ŞAPKA 5 - Kıdemli Siber Güvenlik Mühendisi Perspective

> *"Bir Siber Güvenlik Mühendisi olarak IdentityServer4 entegrasyonuna, SSL sertifika doğrulama açıklarına, veri gizliliğine (KVKK) ve OWASP Top 10 zafiyetlerine odaklanıyorum."*

### 7.1 Kritik Güvenlik Zafiyetleri ve Acil Eylem Planı

> [!WARNING]
> **Zafiyet 1: SSL Sertifika İptali / Bypass (`ServerCertificateCustomValidationCallback => true`)**
> - **Konum:** `Store.Catalog/Program.cs` ve `Store.WebUI/Program.cs`
> - **Tehlike:** Üretim ortamında (Production) Man-in-the-Middle (MitM) saldırılarına kapı açar. Saldırıcı sahte bir SSL sertifikasıyla servisle WebUI arasındaki trafiği dinleyebilir.
> - **Çözüm:** `#if DEBUG` derleme direktifi içine alınmalı veya canlı ortamda geçerli Let's Encrypt / Kurumsal SSL sertifikaları kullanılmalıdır.

> [!WARNING]
> **Zafiyet 2: Hassas Verilerin Açıkta Durması (Plaintext Storage & Secrets)**
> - **Konum:** `appsettings.json` dosyalarındaki veritabanı şifreleri açık metin (plaintext) durmaktadır.
> - **Tehlike:** GitHub/GitLab repository'sine kod atıldığında şifreler sızabilir.
> - **Çözüm:** `dotnet user-secrets` (geliştirme için) ve Environment Variables / Azure Key Vault / HashiCorp Vault (canlı için) kullanılmalıdır.

> [!NOTE]
> **Zafiyet 3: `tempkey.jwk` Dosyası**
> - **Konum:** `IdentityServer/Store.IdentityServer/tempkey.jwk`
> - **Tehlike:** Geliştirme aşamasında token imzalamak için oluşturulan bu anahtar `.gitignore`'a eklenmezse canlıda token sahteciliğine (Token Forgery) yol açar.
> - **Çözüm:** Canlı ortamda gerçek X.509 dijital sertifikası (`AddSigningCredential`) tanımlanmalıdır.

---

## 8. 🧢 ŞAPKA 6 - Kıdemli Test Mühendisi Perspective

> *"Bir Test Mühendisi olarak birim (unit), entegrasyon (integration) ve uçtan uca (E2E) test stratejilerine, test edilebilir kod mimarisine ve regresyon risklerine odaklanıyorum."*

### 8.1 Mevcut Test Kapsamı ve Yeşil Testler

Projemizde test kültürünün temelleri atılmıştır:
- **Test Projesi:** `Services/Location/Tests/Store.Location.Tests` (xUnit + Moq + FluentAssertions)
- **Mevcut Test Sınıfları:** 
  - `DealerAddressControllerTests.cs` (7 Adet Birim Test)
  - `WarehouseControllerTests.cs` (CQRS Handler & Repository Mocking Testleri)
- **Test Sonucu:** Tüm testler **%100 BAŞARI (PASS)** ile çalışmaktadır.

```
  Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7, Duration: 1.2 s
```

### 8.2 Önerilen Test Stratejisi Piramidi

```
          / \
         /   \     E2E Testler (Playwright / Selenium)
        /     \    [ Sipariş & Ödeme Akışı ]
       /-------\
      /         \   Entegrasyon Testleri (Testcontainers)
     /           \  [ PgSQL & MSSQL Gerçek DB Testleri ]
    /-------------\
   /               \  Birim Testler (xUnit + Moq)
  /                 \ [ CQRS Handlers, Domain Logic, Validations ]
 /-------------------\
```

---

## 9. Birleşik Öncelik Tablosu ve Stratejik Yol Haritası

Projeyi canlının eşiğine getirecek ve tüm uzman şapkalarının onayından geçecek 3 aşamalı eylem planı:

### 🔴 Faz 1: Acil Güvenlik ve Mimari Temizlik (Hedef: 07 Ağustos 2026)
1. `Catalog` ve `WebUI` servislerindeki SSL Bypass kodlarının `#if DEBUG` ile sınırlandırılması.
2. `tempkey.jwk` dosyasının `.gitignore` kapsamına alınması.
3. `Store.Shared` projesinin oluşturulup `Catalog` -> `Location.Domain` bağımlılığının tamamen kaldırılması.
4. `Store.Order.Domain` altındaki `Entitties` klasör ve namespace isminin `Entities` olarak düzeltilmesi.

### 🟡 Faz 2: İş Mantığı Tamamlama & B2B/Kiralama Entegrasyonu (Hedef: 15 Ağustos 2026)
1. `Store.B2B` API için WebUI Admin panelinde "Bayi Finans Yönetimi" ekranlarının ve DTO entegrasyonunun tamamlanması.
2. `PriceType.Rental` kiralama süresi, stok takvimi çakışma kontrolü ve kira tutarı hesaplama motorunun kodlanması.
3. Server-Side DataTables mimarisinin Ürün, Sipariş ve Bayi gibi yüksek hacimli ekranlara uygulanması.

### 🔵 Faz 3: Altyapı, Mesajlaşma ve DevOps Olgunlaşması (Hedef: 31 Ağustos 2026)
1. Microservices arası asenkron iletişim için **RabbitMQ** (veya MassTransit) entegrasyonu (Örn: Sipariş oluşturulduğunda stok düşme olayı).
2. Servis yönlendirmeleri, yetkilendirme ve Rate Limiting için **YARP / Ocelot API Gateway** kurulumu.
3. Sık erişilen katalog ve referans verileri için **Redis Distributed Cache** katmanının aktif edilmesi.
4. Tüm servislerin Dockerfile'larının yazılıp `docker-compose.yml` ile tek komutta kaldırılabilir hale getirilmesi.

---

> **Hazırlayan:** 6 Şapkalı Uzman Ekibi (Antigravity)  
> **Tarih:** 05 Ağustos 2026  
> **Dosya Adı:** `ProjeAnalizRaporu_2026.08.05.md`  
> **Durum:** ✅ Tamamlandı & İncelemeye Hazır
