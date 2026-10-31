# 🏪 STORE PROJESİ — 6 ŞAPKALI KAPSAMLI ANALİZ RAPORU
> **Hazırlayan:** Kıdemli PM + Kıdemli Analist + Kıdemli Yazılım Mimarı + Kıdemli DBA + Kıdemli Siber Güvenlik Mühendisi + Kıdemli Test Mühendisi  
> **Tarih:** Temmuz 2026  
> **Durum:** Yeni bir ekip üyesi veya paydaş için hazırlanmış "Projeye Giriş" kılavuzu
Dosyanın Adı : ProjeAnalizi_2026.07.30.md
---

## 📋 İÇİNDEKİLER

1. [Projeye Genel Bakış — Yönetici Özeti](#1-projeye-genel-bakış--yönetici-özeti)
2. [🎩 ŞAPKA 1 — Kıdemli Project Manager](#2--şapka-1--kıdemli-project-manager)
3. [🎩 ŞAPKA 2 — Kıdemli İş Analisti](#3--şapka-2--kıdemli-iş-analisti)
4. [🎩 ŞAPKA 3 — Kıdemli Yazılım Mimarı](#4--şapka-3--kıdemli-yazılım-mimarı)
5. [🎩 ŞAPKA 4 — Kıdemli DBA (Veritabanı Yöneticisi)](#5--şapka-4--kıdemli-dba-veritabanı-yöneticisi)
6. [🎩 ŞAPKA 5 — Kıdemli Siber Güvenlik Mühendisi](#6--şapka-5--kıdemli-siber-güvenlik-mühendisi)
7. [🎩 ŞAPKA 6 — Kıdemli Test Mühendisi](#7--şapka-6--kıdemli-test-mühendisi)
8. [Birleşik Öncelik Tablosu ve Yol Haritası](#8-birleşik-öncelik-tablosu-ve-yol-haritası)

---

## 1. Projeye Genel Bakış — Yönetici Özeti

**Store.sln**, organizasyona ait fiziksel ürünlerin hem **satışını** hem de **kiralanmasını** aynı platform üzerinden yöneten, mikroservis mimarisiyle tasarlanmış bir **E-Ticaret & Kiralama Platformudur.**

Projeyi yeni tanıyacak biri için en temel tabloyla başlayalım:

### Platform Ne İş Yapıyor?

| İş Modeli | Açıklama | Örnek Senaryo |
|:----------|:---------|:--------------|
| **B2C Satış** | Bireysel müşteri ürünü satın alır | Müşteri koltuk takımı satın alıyor |
| **B2C Kiralama** | Bireysel müşteri ürünü dönemlik kiralar | Düğün için masa-sandalye kiralanıyor |
| **B2B Toptan Satış** | Bayi toplu alım yapıyor | Organizasyon firması koltuk alıyor |
| **B2B Toptan Kiralama** | Bayi toplu kiralama yapıyor | Event şirketi sahne malzemesi kiralıyor |

### Teknoloji Yığını (Tech Stack) Özeti

| Katman | Teknoloji |
|:-------|:----------|
| **Backend** | ASP.NET Core 8.0, C# |
| **Mimari** | Mikroservis + Clean Architecture + CQRS |
| **Kimlik Doğrulama** | IdentityServer4, OAuth 2.0, OpenID Connect |
| **Katalog DB** | MongoDB 7.x (NoSQL) |
| **Konum DB** | PostgreSQL 16.x |
| **Sipariş / İndirim DB** | MS SQL Server LocalDB |
| **ORM** | Entity Framework Core + Dapper |
| **Loglama** | Serilog |
| **Validasyon** | FluentValidation |
| **Mesajlaşma** | ❌ Henüz yok (Gelecek Hedef: RabbitMQ) |
| **API Gateway** | ❌ Henüz yok |
| **Konteynerizasyon** | Docker (CI planlanmış) |

---

## 2. 🎩 ŞAPKA 1 — Kıdemli Project Manager

> *"Ben projenin nerede olduğuna, nereye gideceğine ve yolda ne tür risklerle karşılaşabileceğimize bakıyorum."*

### 2.1 Projenin Olgunluk Seviyesi

Proje şu an **gelişim (development) aşamasındadır** ve canlıya (production) taşınmamıştır. Elde ettiğimiz verilere göre tamamlanan işler:

| Tamamlanan | Durumu |
|:-----------|:------:|
| Mikroservis iskelet yapıları (Catalog, Location, Discount, Order) | ✅ |
| Veritabanı şeması ve temel CRUD operasyonları | ✅ |
| IdentityServer4 entegrasyonu ve yetkilendirme rolleri | ✅ |
| CQRS + MediatR yapısının uygulanması (Location, Order) | ✅ |
| Serilog loglama entegrasyonu | ✅ |
| FluentValidation entegrasyonu | ✅ |
| WebUI ön yüzü (Store.WebUI) | ✅ |

### 2.2 Kalan Kritik İşler (Backlog)

| Yapılacak İş | Öncelik | Tahmini Etki |
|:-------------|:-------:|:-------------|
| SSL Bypass güvenlik açığının kapatılması | 🔴 KRİTİK | Canlıya geçiş bloker |
| appsettings.json'daki şifrelerin güvenli yere taşınması | 🔴 KRİTİK | Canlıya geçiş bloker |
| Catalog → Location Domain bağımlılığının koparılması | 🔴 KRİTİK | Mimari borç |
| ContactInfo / ContactInfo2 kod tekrarının temizlenmesi | 🟠 YÜKSEK | Kod kalitesi |
| Order servisindeki `Entitties` yazım hatasının düzeltilmesi | 🟠 YÜKSEK | Bakım kolaylığı |
| Docker-Compose ile altyapı standardizasyonu | 🟡 ORTA | DevOps olgunluğu |
| RabbitMQ/Kafka ile asenkron mesajlaşma | 🟡 ORTA | Ölçeklenebilirlik |
| API Gateway kurulumu | 🟡 ORTA | Güvenlik + yönlendirme |
| Testcontainers ile entegrasyon testleri | 🟡 ORTA | Test kalitesi |
| Elasticsearch entegrasyonu (gelişmiş arama) | 🟢 DÜŞÜK | Özellik geliştirme |
| Redis caching katmanı | 🟢 DÜŞÜK | Performans |

### 2.3 Takım Yapısı ve Paralel Geliştirme Potansiyeli

Projenin **mikroservis yapısı**, takımların bağımsız çalışmasına olanak tanır:

```
┌─────────────────────────────────────────────────────────┐
│              ÖNERİLEN TAKIM YAPISI                      │
│                                                         │
│  🏷️ KATALOG TAKIMI        🛒 SİPARİŞ TAKIMI             │
│  Store.Catalog (MongoDB)  Store.Order (MSSQL)           │
│  Ürün/Kategori/Fiyat      Sipariş Akışı/Ödeme           │
│                                                         │
│  📍 LOKASYON TAKIMI       🖥️ ARAYÜZ TAKIMI              │
│  Store.Location (PgSQL)   Store.WebUI                   │
│  Adres/Şube/Bayi          Frontend/UX                   │
│                                                         │
│  🔐 PLATFORM TAKIMI (Paylaşımlı)                        │
│  IdentityServer + Store.DtoLayer + Shared Core          │
└─────────────────────────────────────────────────────────┘
```

> [!WARNING]
> **Mevcut Sorun:** `Store.DtoLayer` ve `Store.Location.Domain` gibi paylaşılan kütüphaneler üzerindeki değişiklikler birden fazla servisi etkileyebiliyor. Ekipler arası bağımlılık hâlâ yüksek.

### 2.4 Polyglot Persistence Maliyet Analizi

3 farklı veritabanı teknolojisi kullanılması operasyonel maliyet yaratmaktadır:

```
  [İnsan Kaynağı Riski]              [DevOps / Altyapı Maliyeti]
  ─────────────────────              ─────────────────────────────
  • Geliştiricinin MongoDB,          • 3 ayrı veritabanı yedeği
    PostgreSQL, MSSQL ve               (Mongo, PgSQL, MSSQL)
    EF Core + Dapper bilmesi         • 3 ayrı monitoring kurulumu
    gerekiyor                        • Farklı lisanslama gereklilikleri
  • İşe alım ve oryantasyon         • Docker'da 3 ayrı container
    süreleri uzuyor                    yönetimi
```

---

## 3. 🎩 ŞAPKA 2 — Kıdemli İş Analisti

> *"Ben iş süreçlerini, gereksinimlerin doğruluğunu ve sistemin gerçek iş değerini analiz ediyorum."*

### 3.1 Temel İş Süreçleri (Business Flows)

#### Süreç 1: Ürün Satış Akışı

```
Müşteri Girişi   →  Ürün Listesi   →  Sepete Ekle   →  Kupon Uygula
(IdentityServer)    (Catalog API)      (WebUI)           (Discount API)
       ↓
  Adres Seç      →  Sipariş Ver    →  Sipariş Kaydı
(Location API)      (Order API)       (StoreOrderDb)
```

#### Süreç 2: Kiralama Akışı (Gelecek Geliştirme)

```
Ürün Seçimi  →  Kiralama Dönemi Belirle  →  Kira Fiyatı Hesapla
(PriceType: Rental = 3)                     (ProductPrices koleksiyonu)
      ↓
 Sözleşme Oluştur  →  Teslim Adresi  →  Kiralama Başlar
 (RentalContract -      (Location)       (Takip Sistemi
  henüz yok)                              - henüz yok)
```

> [!IMPORTANT]
> **Kritik İş Analizi Bulgusu:** Kiralama iş modeli (RentalContract, kiralama süresi takibi, iade akışı, geç iade cezalandırması) **henüz kodlanmamıştır.** PriceType enum'unda `Rental(3)` ve `Wholerental(5)` değerleri tanımlı, ancak bu değerleri kullanan bir iş akışı (use case) mevcut değil. Bu en büyük eksik iş gereksinimdir.

### 3.2 Fiyatlandırma Motoru Analizi

Proje 5 farklı fiyat türü destekliyor. Bu oldukça olgun bir fiyatlandırma modelidir:

| PriceType Değeri | İş Anlamı | Kullanım Senaryosu |
|:----------------|:----------|:-----------------|
| `Cost (1)` | Maliyet fiyatı | İç raporlama, kâr marjı hesabı |
| `Retail (2)` | Perakende fiyat | B2C son kullanıcı satışı |
| `Rental (3)` | Kiralama fiyatı | Bireysel kiralama |
| `Wholesale (4)` | Toptan satış | B2B bayi satışı |
| `Wholerental (5)` | Toptan kiralama | B2B bayi kiralama |

Ek olarak **dönemsel kampanya** desteği var (`StartDate`, `EndDate`, `DiscountedAmount`) ve **bayi kademesi** (`PriceListTierId`) üzerinden VIP bayi fiyatı oluşturulabiliyor. Bu çok güçlü bir yapı.

### 3.3 Bayi (B2B) Yönetim Analizi

`Dealers` tablosundaki finansal alanlar gerçek bir bayi yönetim sistemi için doğru tasarlanmış:

| Alan | Anlam |
|:-----|:------|
| `DiscountRate` | Bayiye özel genel iskonto oranı |
| `CreditLimit` | Açık hesap çalışabileceği max tutar |
| `CurrentBalance` | Anlık borç/alacak bakiyesi |

> [!NOTE]
> **Analist Tespiti:** `Dealer` entity'si şu an `Location` servisinde yaşıyor. Ancak bu entity'nin işlevi (finansal limit, bakiye, iskonto) coğrafi konum ile ilgisizdir. Bir `Customer` veya `B2B` servisi oluşturulduğunda buraya taşınmalıdır. Bu mimari bir borç olmakla birlikte, **veri modeli olarak doğru tasarlanmıştır.**

### 3.4 KVKK Uyumluluk Analizi

`UserAddress.AnonymizeData()` metodunun varlığı, KVKK'nın bilinçli olarak ele alındığını gösteriyor:

| Anonimleştirilen Alan | Yöntem |
|:---------------------|:-------|
| `ContactFullName` | "silinmiştir" ile değiştiriliyor |
| `PhoneNumber` | Maskeleniyor |
| `AddressDetail` | "silinmiştir" ile değiştiriliyor |
| `IdentityNumber` (TC No) | `null` yapılıyor |
| `IsAnonymized` flag'i | `true` yapılıyor |

> [!CAUTION]
> **Kritik Eksik:** `IdentityNumber` (T.C. Kimlik No) veritabanında **düz metin (plain text)** olarak tutulmaktadır. Bu, KVKK'nın "kişisel verilerin güvenliği" maddesine ve PCI-DSS prensiplerine aykırıdır. Bu alan **mutlaka şifreli (AES-256 vb.) saklanmalıdır.**

---

## 4. 🎩 ŞAPKA 3 — Kıdemli Yazılım Mimarı

> *"Ben sistemin kemiklerini, eklem yerlerini ve taşıma kapasitesini inceliyorum."*

### 4.1 Mimari Harita

```
                          ┌─────────────────────┐
                          │    Store.WebUI       │
                          │  (MVC / Razor)       │
                          └────────┬────────────┘
                                   │ HTTP/REST (Doğrudan)
               ┌───────────────────┼───────────────────────┐
               ↓                   ↓                        ↓
    ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────────┐
    │  Store.Catalog   │  │  Store.Location  │  │  Store.Order         │
    │  (Monolitik)     │  │  (Clean Arch.)   │  │  (Clean Arch.)       │
    │  MongoDB: StoreMD│  │  PgSQL:LocDB     │  │  MSSQL: OrderDB      │
    └─────────────────┘  └─────────────────┘  └─────────────────────┘
                                                         │
                          ┌─────────────────────────────-┘
                          ↓
               ┌─────────────────┐      ┌─────────────────────┐
               │ Store.Discount   │      │  Store.IdentityServer│
               │  (Monolitik)     │      │  (IdentityServer4)   │
               │  MSSQL:DiscountDB│      │  PgSQL: IdentityDB   │
               └─────────────────┘      └─────────────────────┘
```

### 4.2 Clean Architecture Uyumu

Projedeki servisler iki farklı olgunluk seviyesinde tasarlanmış:

| Servis | Mimari Model | Değerlendirme |
|:-------|:------------|:--------------|
| **Store.Location** | Clean Architecture (4 katman) | ✅ Doğru |
| **Store.Order** | Clean Architecture (4 katman) | ✅ Doğru |
| **Store.Catalog** | Monolitik (tek proje) | ⚠️ Kabul edilebilir (basit domain) |
| **Store.Discount** | Monolitik (tek proje) | ✅ Uygun (sadece Coupon tablosu) |

### 4.3 Kritik Mimari Kusurlar

#### KUSUR #1: Catalog → Location Domain Bağımlılığı (Acil)

```
❌ MEVCUT DURUM (YANLIŞ):
Store.Catalog.csproj
    └── ProjectReference → Store.Location.Domain.csproj
                              └── BaseEntity (Guid Id)
                                      ↑
                              Product.cs → "public new string Id" (SHADOWING!)
                              
✅ OLMASI GEREKEN:
Store.Catalog.csproj
    └── Catalog'a özel CatalogBaseEntity (string Id, MongoDB uyumlu)
    
Store.Shared.csproj (Yeni oluşturulacak)
    └── Tüm servislerin kullanabileceği ortak yapılar
```

**Sorunun Etkileri:**
1. **ID Shadowing:** `Product` sınıfı `string Id` tanımlarken, base class `Guid Id` tanımlamış → Serileştirme hataları
2. **Bağımlı Deployment:** Location.Domain değişirse Catalog da yeniden deploy edilmek zorunda
3. **Mimari Prensip İhlali:** Mikroservisler birbirinin domain kütüphanelerine bağımlı olmamalı

#### KUSUR #2: Servisler Arası Senkron HTTP (Uzun Vadeli Risk)

```
Mevcut Durum:
WebUI → HTTP → CatalogService
WebUI → HTTP → OrderService (Sipariş oluşturulurken)
OrderService → HTTP → CatalogService (Stok kontrolü için)

Risk: CatalogService çökerse OrderService de çalışamaz → Cascading Failure

Çözüm: RabbitMQ / Azure Service Bus + Outbox Pattern
```

#### KUSUR #3: ContactInfo / ContactInfo2 Tekrarı

```
Store.Location.Application
    ├── Features.CQRS.Handlers.ContactInfoHandlers   ← Eski yapı
    │       ├── CreateContactInfoCommandHandler
    │       ├── GetContactInfosQueryHandler
    │       └── ContactInfoController
    │
    └── Features.Mediator.Handlers.ContactInfo2Handlers  ← Yeni yapı
            ├── CreateContactInfo2CommandHandler
            ├── GetContactInfo2QueryHandler
            └── ContactInfo2Controller
            
→ Aynı tabloya 2 farklı API endpoint yazıyor/okuyor!
→ Çözüm: CQRS klasik handlers ve ContactInfoController silinmeli,
          ContactInfo2 → ContactInfo olarak yeniden adlandırılmalı.
```

#### KUSUR #4: Order Servisinde Yazım Hatası

```
Store.Order.Domain
    ├── Entities/           ← Doğru klasör (kullanılmıyor)
    └── Entitties/          ← YANLIŞ (çift 't') - asıl sınıflar burada!
            ├── Ordering.cs (namespace: Store.Order.Domain.Entitties)
            └── OrderDetail.cs (namespace: Store.Order.Domain.Entitties)

OrderContext.cs:
    using Store.Order.Domain.Entities;   ← Boş klasör
    using Store.Order.Domain.Entitties;  ← Asıl sınıflar
```

### 4.4 CQRS/MediatR Uygulaması

Clean Architecture uygulanan servislerde MediatR ile CQRS doğru uygulanmış:

```csharp
// Handler örneği (Örnek yapı):
// Application/Features/Orders/Commands/CreateOrder/
//     CreateOrderCommand.cs      ← İstek nesnesi
//     CreateOrderCommandHandler  ← İş mantığı
//     CreateOrderCommandValidator ← FluentValidation
// Controller sadece mediator.Send(command) yapıyor → İnce controller ✅
```

### 4.5 Eksik Mimari Bileşenler

| Bileşen | Önemi | Öncelik |
|:--------|:------|:-------:|
| API Gateway (Ocelot/YARP) | Güvenlik, yönlendirme, rate-limiting | 🔴 |
| Message Broker (RabbitMQ) | Asenkron işlem, loose coupling | 🟠 |
| Shared Core kütüphanesi | BaseEntity, common DTOs | 🔴 |
| Distributed Cache (Redis) | Performans | 🟡 |
| Centralized Logging (ELK) | Gözlemlenebilirlik | 🟡 |
| Health Check Endpoints | Operasyonel görünürlük | 🟡 |

---

## 5. 🎩 ŞAPKA 4 — Kıdemli DBA (Veritabanı Yöneticisi)

> *"Ben verilerin nasıl tutulduğuna, ne kadar güvenli olduğuna ve performansın nerede tıkandığına bakıyorum."*

### 5.1 Veritabanı Mimarisi Özeti

| # | Servis | DBMS | DB Adı | Tablo/Koleksiyon Sayısı | ORM |
|:-:|:-------|:-----|:-------|:----------------------:|:----|
| 1 | Catalog | MongoDB 7.x | `StoreMD` | 8 koleksiyon | MongoDB .NET Driver |
| 2 | Location | PostgreSQL 16 | `StoreLocationDb` | 11 tablo | EF Core + Npgsql |
| 3 | Discount | MSSQL LocalDB | `StoreDiscountDb` | 1 tablo | Dapper + EF Core |
| 4 | Order | MSSQL LocalDB | `StoreOrderDb` | 3 tablo | EF Core |
| 5 | Identity | PostgreSQL | `StoreIdentityDB` | ASP.NET Identity (~12 tablo) | EF Core |

### 5.2 BaseEntity Yapısının DBA Değerlendirmesi

Her tabloda bulunan ortak audit alanları **çok iyi tasarlanmış:**

```
Tüm tablolarda (Discount hariç):
  ✅ Soft Delete (IsDeleted + DeleteDate + DeleteUserId)
  ✅ Audit Trail (InsertDate, InsertUserId, UpdateDate, UpdateUserId)
  ✅ Global Query Filter (IsDeleted == false → otomatik filtrelenir)
  ✅ UTC-Safe zaman damgası (DateTimeOffset, TimeSpan.Zero ile)
  ⚠️ Discount.Coupons tablosu BaseEntity'den türememiş → Audit alanları yok
```

### 5.3 Servis Bazlı Veritabanı Detayları

#### MongoDB — StoreMD (Catalog)

**8 Koleksiyon:**

| Koleksiyon | Amaç | Kimlik Tipi |
|:-----------|:-----|:-----------:|
| `Brands` | Marka tanımları | Guid (String BSON) |
| `Categories` | Sonsuz derinlikte ağaç yapısı (ParentId) | Guid |
| `Currencies` | ISO 4217 para birimleri (TRY, USD, EUR) | Guid |
| `Measurements` | Ölçü birimleri (Adet, Kg, Metre) | Guid |
| `Products` | Ana ürün kartı (SKU, Barcode, SEO, KDV, Durum) | Guid |
| `ProductDetails` | Teknik özellikler, boyutlar, arama etiketleri | Guid |
| `ProductImages` | CDN/S3 görsel URL'leri, galeri sırası | Guid |
| `ProductPrices` | 5 tip fiyat (Cost/Retail/Rental/Wholesale/Wholerental) | Guid |

> [!NOTE]
> **DBA Notu:** MongoDB şemasız çalışsa da, ProductPrices koleksiyonundaki `Amount` ve `DiscountedAmount` alanları `Decimal128` tipiyle saklanıyor. Bu, para birimi hesaplamalarında hassasiyet kaybını önleyen doğru bir seçim.

#### PostgreSQL — StoreLocationDb (Location)

**11 Tablo + Hiyerarşik Yapı:**

```
Countries (Ülke)
    └── Cities (Şehir)
            └── Districts (İlçe)
                    └── Neighborhoods (Mahalle)
                            └── [AddressBase'den türeyen tablolar]
                                    ├── BranchAddresses (Şube Adresleri)
                                    └── UserAddresses (Müşteri Adresleri)

Companies (Şirket)
    └── Branches (Şube)
            ├── BranchAddresses → Neighborhoods
            └── Warehouses (Depo)

ContactInfos ←── Companies.MainContactId
              ←── Branches.ContactInfoId
              ←── Dealers.ContactInfoId

Dealers (Bayi) → ContactInfos
UserAddresses  → Companies (isteğe bağlı)
```

> [!NOTE]
> **DBA Notu [05.08.2026 GÜNCELLEMESİ]:** Önceden `Warehouses` tablosundaki `Address` ve `City` alanları düz metindi. Bu sorun çözüldü! Artık `WarehouseAddress` adında `AddressBase`'den türeyen normalize edilmiş bir fiziksel adres tablosu var ve `WarehouseId` ile bağlanıyor.

#### MSSQL — StoreDiscountDb (Discount)

**1 Tablo (Coupons):**
```
Coupons:
  Id (INT IDENTITY), Code (NVARCHAR MAX), Rate (INT),
  IsActive (BIT), ValidDate (DATETIME2)
  
⚠️ DBA Sorunu: Code alanı NVARCHAR(MAX) tanımlanmış.
   Bir kupon kodu genellikle 20-50 karakter. MAX kullanımı
   gereksiz depolama ve index verimsizliği yaratır.
   Öneri: NVARCHAR(50) veya NVARCHAR(100) yeterli.
   
⚠️ DBA Sorunu: IsActive alanı üzerine INDEX yok.
   "Aktif kuponları getir" sorgusu tam tablo taraması yapıyor.
```

#### MSSQL — StoreOrderDb (Order)

**3 Tablo + Adres Snapshot Tasarımı:**

```
Orderings (Sipariş Master)
    └── OrdersDetails (Sipariş Kalemleri) [1:N]
    
Addresses (Sipariş Anındaki Adres Anlık Görüntüsü)
    → Location DB'ye FK YOK → Kasıtlı tasarım ✅
    → Kullanıcı adresini değiştirse bile sipariş adresi korunur
```

> [!NOTE]
> **DBA Onayı:** `Addresses` tablosunun Location servisine foreign key içermemesi mükemmel bir mikroservis tasarım kararıdır. Sipariş geçmişinin bütünlüğü (immutable snapshot) korunuyor.

### 5.4 Servisler Arası ID Yönetimi

Mikroservis mimarisinde cross-service foreign key yoktur; bunun yerine ID değerleri tutulur:

| Kaynak | Alan | Hedef | Çözüm Yöntemi |
|:-------|:-----|:------|:--------------|
| `OrdersDetails.ProductId` (string) | → | `Products._id` (MongoDB) | HTTP API çağrısı |
| `Ordering.UserId` (string) | → | IdentityServer User | JWT Claim |
| `UserAddress.CustomerId` (Guid) | → | IdentityServer User | Uygulama katmanı |

### 5.5 DBA Öncelikleri

| Öncelik | Sorun | Öneri |
|:-------:|:------|:------|
| 🔴 | LocalDB'den gerçek SQL Server'a geçiş yapılmamış | Prod için SQL Server / Azure SQL |
| 🔴 | UserAddress.IdentityNumber düz metin | AES-256 şifreleme zorunlu |
| 🟠 | Coupons.Code → NVARCHAR(MAX) | NVARCHAR(100)'e düşür |
| 🟠 | Coupons'ta IsActive üzerinde index yok | Filtered index ekle |
| 🟠 | Discount.Coupons BaseEntity'den türemiyor | Audit alanları eksik |
| ✅ | [ÇÖZÜLDÜ - 05.08.2026] Warehouse adres yapısı | WarehouseAddress tablosu eklendi |
| 🟡 | Migration otomasyonu yok | EF Migration runner ekle |

---

## 6. 🎩 ŞAPKA 5 — Kıdemli Siber Güvenlik Mühendisi

> *"Ben sistemin her kapı ve penceresini kontrol ediyorum. Bir saldırgan nereye girebilir diye düşünüyorum."*

### 6.1 Kimlik Doğrulama Mimarisi

**IdentityServer4** kullanımı sağlam bir temel sağlıyor:

```
Client              IdentityServer4          API
  │                 (localhost:5001)          │
  │─ Login ────────────────────────────────▶ │
  │◀─ Access Token (JWT) ──────────────────  │
  │─ API Request + Bearer Token ───────────▶ │
  │                                [Yetki Doğrulama]
```

**3 Kullanıcı Rolü:**

| Rol | İzinler | Kullanım Amacı |
|:----|:--------|:--------------|
| `StoreVisitorId` | `LocationFullPermission` | Giriş yapmamış kullanıcı — Sadece konum/şube görebilir |
| `StoreManagerId` | Location + DiscountFullPermission | Yönetici — İndirim ve konum yönetimi |
| `StoreAdminId` | Tüm API izinleri + OpenID Claims | Süper admin |

### 6.2 Tespit Edilen Güvenlik Açıkları

#### 🔴 KRİTİK AÇIK #1: SSL Sertifika Doğrulama Bypass

**Etkilenen Dosyalar:**
- `Services/Catalog/Store.Catalog/Program.cs` (L38-41)
- `FrontEnds/Store.WebUI/Program.cs` (L18-22)

```csharp
// MEVCUT KOD (YANLIŞ VE TEHLİKELİ):
.ServerCertificateCustomValidationCallback = 
    (msg, cert, chain, errors) => true  // SSL doğrulamasını tamamen kapatıyor!

// OLMASI GEREKEN:
#if DEBUG
    .ServerCertificateCustomValidationCallback = 
        (msg, cert, chain, errors) => true
#endif
```

**Saldırı Senaryosu:** Bu kod canlıya geçerse, bir saldırgan sahte SSL sertifikasıyla araya girerek (Man-in-the-Middle) tüm kullanıcı tokenlarını, ürün verilerini ve sipariş bilgilerini çalabilir.

#### 🔴 KRİTİK AÇIK #2: Şifre ve Bağlantı Dizeleri Düz Metin

**Etkilenen Dosyalar:** Tüm servislerin `appsettings.json` dosyaları

```json
// MEVCUT (YANLIŞ):
{
  "ConnectionStrings": {
    "PostgreSQLConnection": "Server=localhost;Port=5432;Database=StoreLocationDb;Username=postgres;Password=1234"
  }
}

// OLMASI GEREKEN:
// Geliştirme: dotnet user-secrets
// Staging/Prod: Azure Key Vault / HashiCorp Vault / AWS Secrets Manager
```

#### 🔴 KRİTİK AÇIK #3: T.C. Kimlik No Şifresiz Saklanıyor

`UserAddresses.IdentityNumber` alanı `varchar(11)` olarak düz metin saklanıyor. KVKK ve kişisel veri güvenliği açısından bu mutlaka şifreli tutulmalı.

#### 🟠 ORTA RİSK #4: API Gateway Eksikliği

Mikroservislere doğrudan WebUI üzerinden bağlanılıyor. API Gateway olmadan:
- Rate limiting yok → DDoS saldırısına açık
- Merkezi JWT doğrulama yok → Her servis kendi doğrulamasını yapıyor
- İstek loglama merkezi değil → Anomali tespiti zor

#### 🟠 ORTA RİSK #5: IdentityServer4 End-of-Life

IdentityServer4, **Aralık 2022'de End-of-Life (destek sonu)** ilan edilmiştir. Güvenlik yamaları artık gelmemektedir. Duende IdentityServer (ücretli) veya Keycloak/Auth0 gibi alternatiflere geçiş planlanmalıdır.

### 6.3 Mevcut Güvenlik Güçlü Yönler

| Güçlü Yön | Açıklama |
|:----------|:---------|
| ✅ RBAC | 3 kademeli yetkilendirme (Visitor/Manager/Admin) |
| ✅ JWT Token | Stateless, güvenli token tabanlı auth |
| ✅ Blast Radius Küçüklüğü | Her servis ayrı DB → SQL Injection etkisi izole |
| ✅ Soft Delete | Veriler kaybolmuyor, silinmiş kayıt korunuyor |
| ✅ KVKK Anonimleştirme | `AnonymizeData()` metodu mevcut |
| ✅ FluentValidation | Kötü niyetli/hatalı veri girişi filtreleniyor |

### 6.4 Güvenlik Yol Haritası

```
KISA VADE (≤1 ay)           ORTA VADE (1-3 ay)        UZUN VADE (3-6 ay)
─────────────────           ──────────────────         ─────────────────
• SSL bypass → #if DEBUG    • API Gateway kurulumu     • IdentityServer4 →
• appsettings → Secrets     • Rate Limiting            Duende/Keycloak
• IdentityNumber şifreleme  • Centralized Auth log     • WAF (Web App Firewall)
• Prod ortam güvenliği      • Pen-test                 • Encryption at rest
```

---

## 7. 🎩 ŞAPKA 6 — Kıdemli Test Mühendisi

> *"Ben 'çalışıyor' demeden önce 'neden çalışmasın ki?' sorusunu soruyorum."*

### 7.1 Test Edilebilirlik Değerlendirmesi

Projenin test edilebilirlik potansiyeli **Yüksek (8/10)**:

| Avantaj | Açıklama |
|:--------|:---------|
| ✅ CQRS Handler İzolasyonu | Controller'a gerek kalmadan sadece Handler test edilebilir |
| ✅ Clean Architecture | Domain katmanı DB bağımlılığından bağımsız → Mock gerektirmez |
| ✅ FluentValidation | Validasyon kuralları ayrı test edilebilir |
| ✅ Interface'ler | Repository pattern → Mock implementasyonu kolayca yazılabilir |
| ⚠️ Dış Bağımlılık Yoğunluğu | MongoDB + PostgreSQL + MSSQL + IdentityServer → Testcontainers şart |

### 7.2 Mevcut Test Durumu

> [!WARNING]
> Proje dosyaları incelendiğinde **hiçbir test projesi tespit edilmemiştir.** Ne unit test, ne integration test, ne de E2E test mevcut. Bu ciddi bir kalite riskidir.

### 7.3 Önerilen Test Stratejisi

#### Katman 1: Unit Testler

```csharp
// Hedef: CQRS Handler'larını test et
// Örnek: CreateOrderCommandHandlerTests
[Test]
public async Task Handle_ValidOrder_ShouldReturnSuccess()
{
    // Arrange
    var mockRepo = new Mock<IOrderRepository>();
    var handler = new CreateOrderCommandHandler(mockRepo.Object, ...);
    var command = new CreateOrderCommand { UserId = "user1", ... };
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.That(result.IsSuccess, Is.True);
}
```

**Öncelikli Unit Test Hedefleri:**

| Test | Açıklama | Öncelik |
|:-----|:---------|:-------:|
| Coupon doğrulama mantığı | ValidDate ve IsActive kontrolü | 🔴 |
| PriceType hesaplama | Doğru fiyat tipinin seçilmesi | 🔴 |
| AnonymizeData() | KVKK anonimleştirme doğruluğu | 🔴 |
| Order toplam fiyat hesabı | ProductPrice × Amount | 🔴 |
| FluentValidation kuralları | Her servis için | 🟠 |

#### Katman 2: Integration Testler (Testcontainers)

```csharp
// Testcontainers ile gerçek DB'ye karşı test:
public class OrderIntegrationTests : IAsyncLifetime
{
    private MsSqlContainer _sqlContainer;
    
    public async Task InitializeAsync()
    {
        _sqlContainer = new MsSqlBuilder()
            .WithPassword("Test@1234!")
            .Build();
        await _sqlContainer.StartAsync();
    }
    
    // Test sonrası container otomatik yok edilir
    public async Task DisposeAsync() => 
        await _sqlContainer.DisposeAsync();
}
```

#### Katman 3: Kritik E2E Test Senaryoları

**Senaryo 1: Token Yetkilendirme Testleri**

```
StoreVisitorId token ile Order API çağrısı → 403 Forbidden beklenir
StoreManagerId token ile Catalog Admin API → 403 Forbidden beklenir
StoreAdminId token ile tüm API'ler → 200 OK beklenir
Geçersiz token ile herhangi bir API → 401 Unauthorized beklenir
```

**Senaryo 2: Sipariş Bütünlüğü E2E**

```
1. WebUI üzerinden sipariş oluştur
2. StoreOrderDb.Orderings tablosuna yazıldı mı? → Doğrula
3. OrdersDetails'deki ProductId, StoreMD.Products'ta var mı? → Doğrula
4. OrdersDetails'deki ProductPrice, o anki Catalog fiyatıyla uyumlu mu? → Doğrula
5. Ordering.TotalPrice = Σ(OrderDetail.TotalPrice) doğru mu? → Doğrula
```

**Senaryo 3: KVKK Anonimleştirme**

```
1. Kullanıcı adresi oluştur (TC No, Telefon, Ad Soyad ile)
2. AnonymizeData() metodunu çağır
3. ContactFullName = "silinmiştir" mi? → Doğrula
4. IdentityNumber = null mı? → Doğrula
5. IsAnonymized = true mı? → Doğrula
6. Anonimleştirilmiş kayıt API üzerinden hâlâ dönüyor mu? → 404 beklenir
```

**Senaryo 4: Kupon İş Kuralları**

```
Geçerlilik tarihi geçmiş kupon → Geçersiz kabul edilmeli
IsActive = false kupon → Geçersiz kabul edilmeli
Geçerli kupon ile sipariş → Fiyata doğru indirim uygulanmalı
Aynı kupon iki kez kullanılmaya çalışılırsa → İş kuralı belirlenmeli (eksik)
```

### 7.4 Test Altyapısı Kurulum Önerisi

```xml
<!-- Test projelerinde kullanılacak paketler -->
<PackageReference Include="xunit" Version="2.9.*" />
<PackageReference Include="Moq" Version="4.20.*" />
<PackageReference Include="FluentAssertions" Version="6.12.*" />
<PackageReference Include="Testcontainers.MsSql" Version="3.9.*" />
<PackageReference Include="Testcontainers.PostgreSql" Version="3.9.*" />
<PackageReference Include="Testcontainers.MongoDb" Version="3.9.*" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.*" />
```

---

## 8. Birleşik Öncelik Tablosu ve Yol Haritası

### 8.1 Tüm Şapkalardan Gelen Acil Aksiyonlar

| # | Aksiyon | Şapka | Öncelik | Tahmini Süre |
|:-:|:--------|:------|:-------:|:-------------|
| 1 | SSL bypass → `#if DEBUG` blokuna al | 🔐 Güvenlik | 🔴 ACİL | 30 dakika |
| 2 | appsettings şifreleri → User Secrets / Vault | 🔐 Güvenlik | 🔴 ACİL | 2-4 saat |
| 3 | IdentityNumber → Şifreli saklama | 🔐 Güvenlik + 💾 DBA | 🔴 ACİL | 1-2 gün |
| 4 | Store.Catalog → Location.Domain bağımlılığını kopar | 🏗️ Mimari | 🔴 ACİL | 2-4 saat |
| 5 | ContactInfo / ContactInfo2 tekrarını temizle | 🏗️ Mimari | 🟠 YÜKSEK | 3-4 saat |
| 6 | `Entitties` yazım hatasını düzelt | 🏗️ Mimari | 🟠 YÜKSEK | 1 saat |
| 7 | NVARCHAR(MAX) → NVARCHAR(100) (Coupons.Code) | 💾 DBA | 🟠 YÜKSEK | 30 dakika |
| 8 | Unit test projelerini oluştur | 🧪 Test | 🟠 YÜKSEK | 1-2 gün |

### 8.2 Yol Haritası

```
KISA VADE (1-3 Ay)            ORTA VADE (3-6 Ay)           UZUN VADE (6+ Ay)
─────────────────────          ─────────────────────         ─────────────────────
✅ SSL Bypass düzelt           🐳 Docker-Compose             📨 RabbitMQ / Kafka
✅ Secrets Management          🧪 Testcontainers Int. Tests  🔍 Elasticsearch
✅ Catalog domain bağımsızlığı 🌐 API Gateway (Ocelot/YARP) 🗄️ Redis Cache
✅ ContactInfo tekrar temizliği 📊 Merkezi Loglama (ELK/Seq) 🔑 Duende/Keycloak geçiş
✅ Entitties yazım düzelt      ⚡ gRPC iç servis iletişimi  📦 Kiralama iş akışı
✅ Unit testler yaz             📋 OpenAPI / Swagger belgeleme 🏢 Customer Servisi (Dealer/UserAddress taşıma)
```

### 8.3 Teknik Borç Özet Tablosu

| Kategori | Borç Miktarı | Risk Seviyesi |
|:---------|:------------:|:-------------:|
| Güvenlik açıkları | 3 kritik | 🔴 YÜKSEK |
| Mimari kusurlar | 4 sorun | 🟠 ORTA-YÜKSEK |
| DBA sorunları | 5 sorun | 🟠 ORTA |
| Test eksikliği | Sıfır test | 🟠 ORTA |
| Eksik iş akışları | Kiralama flow | 🟡 ORTA-DÜŞÜK |

---

> **Not:** Bu rapor, `Store.sln` projesindeki tüm kaynak dosyalar, entity sınıfları, konfigürasyonlar ve proje referansları doğrudan incelenerek hazırlanmıştır. Herhangi bir madde hakkında daha derin analiz veya kod düzeltme desteği almak için ilgili şapkayı belirtmeniz yeterlidir.


---

## 9. Proje Dosya Agaci (Kaynak Kod)

> **Not:** in/, obj/, .vs/, publish/ klasorleri haric tutulmustur.

```
Store/
|-- 20260619_SonDurum.md
|-- BirEntityIcinYapilacaklar.md
|-- dart_test.log
|-- FixCsproj.ps1
|-- FixMissingViews.ps1
|-- GeriAlmaProceduresiHayatDongusu.md
|-- hatalar ve cozumleri.md
|-- IdentityServerVersionErrorSolution.txt
|-- PortNumbers.txt
|-- ProjeDokumai.md
|-- ProjeYapisi_02.md
|-- ProjeAnalizi_2026.07.30.md   <- Bu dosya
|-- project_context.md
|-- RegenerateAllWebUI.ps1
|-- Store.slnLaunch.user
|-- Store.slnx
|-- VeriTabani_01.md
|-- VeriTabani_02.md
|-- YapilanIslemler.md
|-- YapilanIslemler_001.md
|-- YapilanIslemler_002.md
|-- Yaptiklari_001.md
|
|-- Documents/
|
|-- FixEncoding/
|
|-- FrontEnds/
|   +-- Store.WebUI/
|       |-- appsettings.json
|       |-- appsettings.Development.json
|       |-- Program.cs
|       |-- Store.WebUI.csproj
|       |-- Controllers/
|       |   |-- BrandController.cs
|       |   |-- CartController.cs
|       |   |-- CategoryController.cs
|       |   |-- ContactInfoController.cs
|       |   |-- CurrencyController.cs
|       |   |-- DefaultController.cs
|       |   |-- DiscountController.cs
|       |   |-- IdentityController.cs
|       |   |-- LocationController.cs
|       |   |-- MeasurementController.cs
|       |   |-- OrderController.cs
|       |   |-- ProductController.cs
|       |   |-- ProductDetailController.cs
|       |   |-- ProductImageController.cs
|       |   +-- ProductPriceController.cs
|       |-- Models/
|       |-- Services/
|       +-- Views/
|
|-- IdentityServer/
|   +-- Store.IdentityServer/
|       |-- appsettings.json
|       |-- Config.cs              <- Client/Scope/Role tanimlari
|       |-- Program.cs
|       |-- Store.IdentityServer.csproj
|       +-- Properties/
|           +-- launchSettings.json
|
|-- Requirements/
|
|-- Services/
|   |
|   |-- Catalog/
|   |   +-- Store.Catalog/                  [MongoDB - Monolitik Servis]
|   |       |-- appsettings.json
|   |       |-- Program.cs
|   |       |-- Store.Catalog.csproj        [!! Location.Domain bagimliligi var]
|   |       |-- Context/
|   |       |   +-- MongoDbService.cs
|   |       |-- Controllers/
|   |       |   |-- BrandController.cs
|   |       |   |-- CategoryController.cs
|   |       |   |-- CurrencyController.cs
|   |       |   |-- MeasurementController.cs
|   |       |   |-- ProductController.cs
|   |       |   |-- ProductDetailController.cs
|   |       |   |-- ProductImageController.cs
|   |       |   +-- ProductPriceController.cs
|   |       |-- Entities/
|   |       |   |-- Brand.cs
|   |       |   |-- Category.cs
|   |       |   |-- Currency.cs
|   |       |   |-- Measurement.cs
|   |       |   |-- Product.cs              [!! ID Shadowing sorunu]
|   |       |   |-- ProductDetail.cs
|   |       |   |-- ProductImage.cs
|   |       |   +-- ProductPrice.cs
|   |       +-- Settings/
|   |
|   |-- Discount/
|   |   +-- Store.Discount/                 [MSSQL + Dapper - Monolitik Servis]
|   |       |-- appsettings.json
|   |       |-- Program.cs
|   |       |-- Store.Discount.csproj
|   |       |-- Context/
|   |       |   +-- DapperContext.cs
|   |       |-- Controllers/
|   |       |   +-- DiscountController.cs
|   |       +-- Entities/
|   |           +-- Coupon.cs               [!! BaseEntity'siz - audit alanlari yok]
|   |
|   |-- Location/
|   |   |-- Core/
|   |   |   |-- Store.Location.Application/
|   |   |   |   |-- Features/
|   |   |   |   |   |-- CQRS/              [!! Eski pattern - temizlenecek]
|   |   |   |   |   |   |-- Handlers/
|   |   |   |   |   |   |   +-- ContactInfoHandlers/ [CreateContactInfoCommandHandler, GetContactInfosQueryHandler, ...]
|   |   |   |   |   |   |-- Queries/
|   |   |   |   |   |   +-- Results/
|   |   |   |   |   +-- Mediator/          [Yeni pattern - kalacak]
|   |   |   |   |       |-- Commands/
|   |   |   |   |       |   |-- BranchAddressCommands/
|   |   |   |   |       |   |-- BranchCommands/
|   |   |   |   |       |   |-- CityCommands/
|   |   |   |   |       |   |-- CompanyCommands/
|   |   |   |   |       |   |-- ContactInfo2Commands/
|   |   |   |   |       |   |-- CountryCommands/
|   |   |   |   |       |   |-- DealerCommands/
|   |   |   |   |       |   |-- DistrictCommands/
|   |   |   |   |       |   |-- NeighborhoodCommands/
|   |   |   |   |       |   |-- UserAddressCommands/
|   |   |   |   |       |   +-- WarehouseCommands/
|   |   |   |   |       |-- Handlers/
|   |   |   |   |       |   |-- BranchAddressHandlers/
|   |   |   |   |       |   |-- BranchHandlers/
|   |   |   |   |       |   |-- CityHandlers/
|   |   |   |   |       |   |-- CompanyHandlers/
|   |   |   |   |       |   |-- ContactInfo2Handlers/ [ContactInfo2 = ContactInfo olarak degistirilecek]
|   |   |   |   |       |   |-- CountryHandlers/
|   |   |   |   |       |   |-- DealerHandlers/
|   |   |   |   |       |   |-- DistrictHandlers/
|   |   |   |   |       |   |-- NeighborhoodHandlers/
|   |   |   |   |       |   |-- UserAddressHandlers/
|   |   |   |   |       |   +-- WarehouseHandlers/
|   |   |   |   |       |-- Queries/ ...
|   |   |   |   |       +-- Results/ ...
|   |   |   |   |-- Interfaces/
|   |   |   |   |   +-- IUserContext.cs
|   |   |   |   +-- Services/
|   |   |   |       +-- ServiceRegistration.cs
|   |   |   |
|   |   |   +-- Store.Location.Domain/
|   |   |       |-- Entities/
|   |   |       |   |-- AddressBase.cs
|   |   |       |   |-- BaseEntity.cs          [!! Catalog servisi bu sinifi kullaniyor - HATALI]
|   |   |       |   |-- Branch.cs
|   |   |       |   |-- BranchAddress.cs
|   |   |       |   |-- City.cs
|   |   |       |   |-- Company.cs
|   |   |       |   |-- ContactInfo.cs
|   |   |       |   |-- Country.cs
|   |   |       |   |-- Dealer.cs              [!! Customer servisine tasimasi gerekir]
|   |   |       |   |-- District.cs
|   |   |       |   |-- InvoiceType.cs         (Enum: Individual=1, Corporate=2)
|   |   |       |   |-- Neighborhood.cs
|   |   |       |   |-- PriceType.cs           [!! Catalog Domain'e tasimasi gerekir]
|   |   |       |   |-- UserAddress.cs         (AnonymizeData() KVKK metodu)
|   |   |       |   +-- Warehouse.cs           [!! Inventory servisine tasimasi gerekir]
|   |   |       +-- Store.Location.Domain.csproj
|   |   |
|   |   +-- Infrastructure/
|   |       +-- Store.Location.Persistence/
|   |           |-- Context/
|   |           |   +-- LocationContext.cs      (EF Core DbContext + Global Query Filter)
|   |           |-- Migrations/
|   |           |   +-- [EF Core migration dosyalari]
|   |           |-- Repositories/
|   |           |   +-- [Repository implementasyonlari]
|   |           |-- Controllers/
|   |           |   |-- BranchAddressController.cs
|   |           |   |-- BranchController.cs
|   |           |   |-- CityController.cs
|   |           |   |-- CompanyController.cs
|   |           |   |-- ContactInfo2Controller.cs [Kalacak]
|   |           |   |-- ContactInfoController.cs  [!! Silinecek - kod tekrari]
|   |           |   |-- CountryController.cs
|   |           |   |-- DealerController.cs
|   |           |   |-- DistrictController.cs
|   |           |   |-- NeighborhoodController.cs
|   |           |   |-- UserAddressController.cs
|   |           |   +-- WarehouseController.cs
|   |           +-- ServiceRegistration.cs
|   |
|   +-- Order/
|       |-- Core/
|       |   |-- Store.Order.Application/
|       |   |   |-- Features/
|       |   |   |   |-- CQRS/
|       |   |   |   |   |-- Commands/ (Address, OrderDetail)
|       |   |   |   |   |-- Handlers/
|       |   |   |   |   |-- Queries/
|       |   |   |   |   +-- Results/
|       |   |   |   +-- Mediator/
|       |   |   |       |-- Commands/ (Ordering)
|       |   |   |       |-- Handlers/
|       |   |   |       |-- Queries/
|       |   |   |       +-- Results/
|       |   |   |-- Interfaces/
|       |   |   |   |-- IRepository.cs
|       |   |   |   +-- IUserContext.cs
|       |   |   +-- Services/
|       |   |       +-- ServiceRegistration.cs
|       |   |
|       |   +-- Store.Order.Domain/
|       |       |-- Entities/              [!! Bos klasor - kullanilmiyor]
|       |       |   +-- BaseEntity.cs
|       |       +-- Entitties/             [!! YAZIM HATASI (cift t) - asil siniflar burada!]
|       |           |-- Address.cs
|       |           |-- OrderDetail.cs
|       |           +-- Ordering.cs
|       |
|       +-- Presentation/
|           +-- Store.Order.WebApi/
|               |-- appsettings.json
|               |-- Program.cs
|               |-- Store.Order.WebApi.csproj
|               |-- Controllers/
|               |   |-- AdressesController.cs   [!! "Adresses" yazim hatasi]
|               |   |-- DeleteOrderingCommand.cs
|               |   |-- OrderDetailsController.cs
|               |   +-- OrderingsController.cs
|               +-- Services/
|                   +-- UserContext.cs
|
+-- Store.DtoLayer/                             [Paylasimli DTO Kutuphanesi]
    |-- Store.DtoLayer.csproj
    |-- CatalogDtos/
    |   |-- BrandDtos/         [Create, Delete, GetById, Result, Update]
    |   |-- CategoryDtos/      [Create, Result, Update]
    |   |-- CurrencyDtos/      [Create, Result, Update]
    |   |-- InventoryDtos/     [Create, Result, Update] <- Gelecekteki Envanter servisi
    |   |-- MeasurementDtos/   [Create, Result, Update]
    |   |-- ProductDetailDtos/ [Create, Result, Update]
    |   |-- ProductDtos/       [CreateCommand, Create, GetAllQuery, Result, Update]
    |   |-- ProductImageDtos/  [Create, Result, Update]
    |   +-- ProductPriceDtos/  [Create, Result, Update]
    +-- LocationDtos/
        |-- BranchAddressDtos/ [Create, Result, Update]
        |-- BranchDtos/        [Create, Result, Update]
        |-- CityDtos/          [Create, Result, Update]
        |-- CompanyDtos/       [Create, Result, Update]
        |-- ContactInfoDtos/   [Create, Result, Update]
        |-- CountryDtos/       [Create, Result, Update]
        |-- DealerDtos/        [Create, Result, Update]
        |-- DistrictDtos/      [Create, Result, Update]
        |-- InvoiceTypeDtos/   [bos - henuz DTO yok]
        |-- NeighborhoodDtos/  [Create, Result, Update]
        |-- PriceTypeDtos/     [bos - henuz DTO yok]
        |-- UserAddressDtos/   [Create, Result, Update]
        +-- WarehouseDtos/     [Create, Result, Update]
```

---

> **Not:** Bu rapor, Store.sln projesindeki tum kaynak dosyalar, entity siniflari, konfigurasyonlar ve proje referanslari dogrudan incelenerek 30 Temmuz 2026 tarihinde hazirlanmistir.


### G�ncelleme (2026.08.05): Dealer B2B Servisi Ay�rma
1. Dealer entitysindeki (Store.Location) finansal alanlar (TaxNumber, TaxOffice, DiscountRate, CreditLimit, CurrentBalance) DDD kurallar� gere�i temizlendi.
2. Yeni Store.B2B mikroservisi olu�turuldu.
3. DealerFinancial entity'si ile StoreB2BDb aya�a kald�r�larak bu alanlar ilgili servise (Bounded Context) ta��nd�.
