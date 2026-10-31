# Store Projesi — Veritabanı Mimarisi Teknik Referans Raporu

> **Belge:** `VeriTabani_02.md`
> **Proje:** `Store.sln` — Mikroservis E-Ticaret & Kiralama Platformu
> **Tarih:** Temmuz 2026
> **Kapsam:** DBMS seçimleri, veritabanı bağlantı bilgileri, DbContext yapıları, entity sınıf hiyerarşisi, tüm tablo/koleksiyon şemaları ve sütun kısıtlamaları
> **Kaynak:** Doğrudan `.cs` kaynak dosyaları ve `appsettings.json` konfigürasyonlarından türetilmiştir.

---

## İÇİNDEKİLER

1. [Mimari Özet](#1-mimari-özet)
2. [Ortak Temel Sınıflar (Base Entities)](#2-ortak-temel-sınıflar-base-entities)
3. [Catalog Mikroservisi — MongoDB (StoreMD)](#3-catalog-mikroservisi--mongodb-storemd)
4. [Location Mikroservisi — PostgreSQL (StoreLocationDb)](#4-location-mikroservisi--postgresql-storelocationdb)
5. [Discount Mikroservisi — MS SQL Server (StoreDiscountDb)](#5-discount-mikroservisi--ms-sql-server-storediscountdb)
6. [Order Mikroservisi — MS SQL Server (StoreOrderDb)](#6-order-mikroservisi--ms-sql-server-storeorderdb)
7. [Enum Referansları](#7-enum-referansları)
8. [Servisler Arası İlişki Haritası](#8-servisler-arası-ilişki-haritası)

---

## 1. MİMARİ ÖZET

`Store.sln`, her mikroservisin kendi veritabanı motorunu bağımsız olarak yönettiği **Database-per-Service** (Servis Başına Veritabanı) prensibine dayalı bir mimariye sahiptir. Bu yapı, herhangi bir servisin diğerlerini etkilemeden bağımsız olarak ölçeklenebilmesini ve farklı veri modellerine optimize veritabanı motorları kullanabilmesini sağlar.

### 1.1 DBMS ve Veritabanı Özet Tablosu

| # | Mikroservis | DBMS / Motor | Veritabanı Adı | ORM / Erişim Katmanı | Bağlantı |
|:---:|:---|:---|:---|:---|:---|
| 1 | **Catalog** | MongoDB 7.x (NoSQL) | `StoreMD` | MongoDB Driver (.NET) | `mongodb://localhost:27017` |
| 2 | **Location** | PostgreSQL 16.x | `StoreLocationDb` | EF Core + Npgsql | `Server=localhost;Port=5432` |
| 3 | **Discount** | MS SQL Server (LocalDB) | `StoreDiscountDb` | Dapper + EF Core | `(localdb)\MSSQLLocalDB` |
| 4 | **Order** | MS SQL Server (LocalDB) | `StoreOrderDb` | EF Core | `(localdb)\MSSQLLocalDB` |

### 1.2 Servis Yapısal Karşılaştırması

| Özellik | Catalog | Location | Discount | Order |
|:---|:---|:---|:---|:---|
| **Mimari Katman** | Tek Proje (Monolitik Servis) | Clean Architecture (Domain/Application/Infrastructure) | Tek Proje (Monolitik Servis) | Clean Architecture |
| **Kimlik (PK) Türü** | `Guid` (MongoDB ObjectId ile haritalı) | `Guid` | `int` (Identity) | `int` (Identity) |
| **Soft Delete** | Evet — `IsDeleted` flag | Evet — `IsDeleted` flag + Global Query Filter | Hayır | Evet — `IsDeleted` flag + Global Query Filter |
| **Zaman Damgası** | `DateTimeOffset` (UTC-Safe) | `DateTimeOffset` (UTC-Safe) | `DateTime` | `DateTimeOffset` (UTC-Safe) |
| **Migration** | Hayır — Şemasız (Schema-less) | Evet — EF Core Migrations | Evet — EF Core Migrations | Evet — EF Core Migrations |

---

## 2. ORTAK TEMEL SINIFLAR (BASE ENTITIES)

Projede her mikroservis, kendi katmanında ayrı bir `BaseEntity` soyut sınıfı tanımlar. Bu sınıflar tüm entity'lere ortak audit (denetim) ve soft-delete alanları kazandırır.

### 2.1 Location & Catalog — `BaseEntity` (Guid Kimlikli)

**Dosya:** `Services/Location/Core/Store.Location.Domain/Entities/BaseEntity.cs`

> Catalog mikroservisi, `Store.Location.Domain` projesine proje referansı (`ProjectReference`) ekleyerek bu sınıfı doğrudan kullanmaktadır.

| Özellik Adı | C# Tipi | DB Karşılığı | Kısıtlama | Varsayılan | Açıklama |
|:---|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `UUID` / `uniqueidentifier` | `[Key]` — **Primary Key** | `Guid.NewGuid()` | Tablodaki kaydın eşsiz birincil anahtarı. |
| `Notes` | `string?` | `TEXT` / `NVARCHAR(1000)` | `[StringLength(1000)]`, Nullable | `null` | Yönetici notları, kayıtla ilgili özel açıklamalar. |
| `IsActive` | `bool` | `BOOLEAN` / `BIT` | — | `true` | Kaydın arayüzde görünürlüğünü kontrol eden bayrak. |
| `IsDeleted` | `bool` | `BOOLEAN` / `BIT` | **Global Query Filter** | `false` | Soft-delete bayrağı. `true` olan kayıtlar tüm sorgulardan otomatik filtrelenir. |
| `InsertUserId` | `Guid` | `UUID` | **NOT NULL** | — | Kaydı ilk oluşturan kullanıcının kimliği. |
| `InsertDate` | `DateTimeOffset` | `TIMESTAMPTZ` | **NOT NULL** | `GetLocalTime()` | Kaydın oluşturulma tarihi (yerel saat, sıfır offset ile). |
| `UpdateUserId` | `Guid?` | `UUID` | Nullable | `null` | Son güncelleyen kullanıcının kimliği. |
| `UpdateDate` | `DateTimeOffset?` | `TIMESTAMPTZ` | Nullable | `null` | Son güncelleme tarihi. |
| `DeleteUserId` | `Guid?` | `UUID` | Nullable | `null` | Soft-delete yapan kullanıcının kimliği. |
| `DeleteDate` | `DateTimeOffset?` | `TIMESTAMPTZ` | Nullable | `null` | Soft-delete tarihi. |

> **`GetLocalTime()` Notu:** PostgreSQL ve MongoDB'nin otomatik UTC dönüşümünü engellemek için `DateTime.Now.Ticks` değeri `TimeSpan.Zero` offset ile `DateTimeOffset` olarak kaydedilir. Bu sayede sunucu yerel saati kayıt kaymasına uğramadan veritabanına işlenir.

---

### 2.2 Order — `BaseEntity` (int Kimlikli)

**Dosya:** `Services/Order/Core/Store.Order.Domain/Entities/BaseEntity.cs`

Order servisi, Auto-Increment kimlik stratejisi kullanır. Alanların büyük çoğunluğu `Guid` tabanlı versiyonla aynıdır; tek fark kimlik alanının türü ve `InsertUserId`'nin nullable olmasıdır.

| Özellik Adı | C# Tipi | DB Karşılığı | Kısıtlama | Varsayılan | Açıklama |
|:---|:---|:---|:---|:---|:---|
| `Id` | `int` | `INT IDENTITY(1,1)` | `[Key]` — **Primary Key** | Auto-increment | Tablodaki kaydın otomatik artan birincil anahtarı. |
| `Notes` | `string?` | `NVARCHAR(1000)` | `[StringLength(1000)]`, Nullable | `null` | Kayda ait özel notlar. |
| `IsActive` | `bool` | `BIT` | — | `true` | Aktiflik bayrağı. |
| `IsDeleted` | `bool` | `BIT` | **Global Query Filter** | `false` | Soft-delete bayrağı. |
| `InsertUserId` | `Guid?` | `uniqueidentifier` | Nullable | `null` | Kaydı oluşturan kullanıcı (Order servisinde nullable). |
| `InsertDate` | `DateTimeOffset` | `datetimeoffset` | **NOT NULL** | `GetLocalTime()` | Oluşturma tarihi. |
| `UpdateUserId` | `Guid?` | `uniqueidentifier` | Nullable | `null` | Son güncelleyenin kimliği. |
| `UpdateDate` | `DateTimeOffset?` | `datetimeoffset` | Nullable | `null` | Son güncelleme tarihi. |
| `DeleteUserId` | `Guid?` | `uniqueidentifier` | Nullable | `null` | Silen kullanıcı kimliği. |
| `DeleteDate` | `DateTimeOffset?` | `datetimeoffset` | Nullable | `null` | Silinme tarihi. |

---

### 2.3 Location — `AddressBase` (Soyut Adres Şablonu, TPC Mimarisi)

**Dosya:** `Services/Location/Core/Store.Location.Domain/Entities/AddressBase.cs`

`AddressBase`, `BaseEntity`'den türeyen ve **TPC (Table-Per-Concrete-Type)** mimarisiyle çalışan soyut (`abstract`) bir sınıftır. Veritabanında doğrudan bir tablo olarak oluşmaz; tüm alanları `BranchAddresses` ve `UserAddresses` tablolarına kalıtım yoluyla taşınır.

| Özellik Adı | C# Tipi | DB Karşılığı | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Title` | `string` | `VARCHAR(100)` | `[Required]`, `[StringLength(100)]` | Adresin kullanıcı tarafından verilen başlığı (Örn: "Ev Adresi"). |
| `ContactFullName` | `string` | `VARCHAR(150)` | `[Required]`, `[StringLength(150)]` | Teslimat/iletişim yetkilisinin adı soyadı. |
| `PhoneNumber` | `string` | `VARCHAR(30)` | `[Required]`, `[StringLength(30)]` | Teslimat iletişim telefonu. |
| `AddressDetail` | `string` | `VARCHAR(500)` | `[Required]`, `[StringLength(500)]` | Sokak, cadde, bina no, daire gibi açık adres detayı. |
| `CityId` | `Guid` | `UUID` | **Foreign Key** → `Cities` | Adresin bağlı olduğu şehir. |
| `DistrictId` | `Guid` | `UUID` | **Foreign Key** → `Districts` | Adresin bağlı olduğu ilçe. |
| `NeighborhoodId` | `Guid` | `UUID` | **Foreign Key** → `Neighborhoods` | Adresin bağlı olduğu mahalle. |

---

## 3. CATALOG MİKROSERVİSİ — MongoDB (`StoreMD`)

**Bağlantı:** `mongodb://localhost:27017`
**Veritabanı:** `StoreMD`
**ORM / Sürücü:** MongoDB .NET Driver
**Proje:** `Services/Catalog/Store.Catalog`
**Mimari:** Tek proje (monolitik mikroservis)
**Kimlik Stratejisi:** `Guid` (BaseEntity'den; MongoDB'de `BsonRepresentation(BsonType.String)` ile string olarak saklanır)

> MongoDB şemasız (schema-less) yapıda çalışır. Aşağıdaki alanlar C# entity sınıflarına ve `appsettings.json` içindeki koleksiyon adlarına göre belgelenmiştir.

---

### Koleksiyon: `Brands` — Marka Bilgileri

**Entity:** `Store.Catalog/Entities/Brand.cs`
**Miras:** `BaseEntity` (Guid + 10 audit alanı dahildir)
**Amaç:** Ürünlere atanabilecek marka kimlik ve tanım bilgilerini tutar.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Açıklama |
|:---|:---|:---|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | Primary Key. MongoDB `_id` alanına haritalı. |
| `Code` | `string` | `String` | Hayır | Markanın kısa kodu veya URL-slug'ı (Örn: `sez-dekor`). |
| `Name` | `string` | `String` | Hayır | Markanın tam ticari adı (Örn: `Sezadan Dekoratif`). |
| *(+ BaseEntity alanları)* | — | — | — | `Notes`, `IsActive`, `IsDeleted`, `InsertUserId`, `InsertDate`, `UpdateUserId`, `UpdateDate`, `DeleteUserId`, `DeleteDate` |

---

### Koleksiyon: `Categories` — Kategori Hiyerarşisi

**Entity:** `Store.Catalog/Entities/Category.cs`
**Miras:** `BaseEntity`
**Amaç:** Sonsuz derinlikte ağaç (tree) yapısı oluşturabilen kategori sistemini tutar. `ParentId` null ise kök kategoriyedir.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Açıklama |
|:---|:---|:---|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | Primary Key. |
| `ParentId` | `Guid?` | `String` | Evet | Üst kategorinin kimliği. `null` ise bu bir kök (root) kategoridir. |
| `Code` | `string?` | `String` | Evet | ERP veya entegrasyon sistemleri için kısa kategori kodu. |
| `Name` | `string` | `String` | Hayır | Sitede ve kataloglarda görünecek kategori adı (Örn: `Nişan Konseptleri`). |
| *(+ BaseEntity alanları)* | — | — | — | Ortak audit alanları. |

---

### Koleksiyon: `Currencies` — Para Birimleri

**Entity:** `Store.Catalog/Entities/Currency.cs`
**Miras:** `BaseEntity`
**Amaç:** `ProductPrices` koleksiyonundaki fiyat kayıtlarında başvurulan para birimi tanımlarını tutar.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Açıklama |
|:---|:---|:---|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | Primary Key. |
| `Code` | `string` | `String` | Hayır | ISO 4217 üç haneli kodu (Örn: `TRY`, `USD`, `EUR`). |
| `Name` | `string` | `String` | Hayır | Para biriminin okunuşu (Örn: `Türk Lirası`). |
| `Symbol` | `string` | `String` | Hayır | Sembol (Örn: `₺`, `$`, `€`). |
| *(+ BaseEntity alanları)* | — | — | — | Ortak audit alanları. |

---

### Koleksiyon: `Measurements` — Ölçü Birimleri

**Entity:** `Store.Catalog/Entities/Measurement.cs`
**Miras:** `BaseEntity`
**Amaç:** `Products` koleksiyonundaki ürünlerin hangi birimde işlem göreceğini (Adet, Metre, Kg vb.) tanımlar.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Açıklama |
|:---|:---|:---|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | Primary Key. |
| `Code` | `string` | `String` | Hayır | Sistem kodu (Örn: `pcs`, `kg`, `m`). |
| `Name` | `string` | `String` | Hayır | Birimin tam adı (Örn: `Kilogram`, `Adet`, `Metre`). |
| `Symbol` | `string` | `String` | Hayır | Kısa sembol (Örn: `Kg`, `Ad.`, `m`). |
| *(+ BaseEntity alanları)* | — | — | — | Ortak audit alanları. |

---

### Koleksiyon: `Products` — Ana Ürün Kartı

**Entity:** `Store.Catalog/Entities/Product.cs`
**Miras:** `BaseEntity`
**Amaç:** Platformun satış ve kiralama için sunduğu her ürünün temel, mali, lojistik ve SEO bilgilerini barındıran ana kayıt koleksiyonudur. Fiyat, görsel ve detay bilgileri bağımsız koleksiyonlarda tutulur.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Varsayılan | Açıklama |
|:---|:---|:---|:---:|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | `Guid.NewGuid()` | Primary Key. |
| **— Temel Bilgiler —** | | | | | |
| `ProductCode` | `string` | `String` | Hayır | `""` | Şirketin dahili stok/SKU kodu (Örn: `DAFT017-MT02`). |
| `Barcode` | `string?` | `String` | Evet | `null` | EAN/UPC barkod numarası. Depo el terminalleri için kullanılır. |
| `Name` | `string` | `String` | Hayır | `""` | Ürünün sitedeki görünen adı. |
| `ShortDescription` | `string?` | `String` | Evet | `null` | Kategori listeleme sayfalarındaki kısa özet (1-2 cümle). |
| `Description` | `string?` | `String` | Evet | `null` | Ürün detay sayfasındaki zengin HTML açıklama metni. |
| **— Durum Bayrakları —** | | | | | |
| `IsRentalEnabled` | `bool` | `Boolean` | Hayır | `true` | Ürün kiralanabilir mi? |
| `IsSaleEnabled` | `bool` | `Boolean` | Hayır | `true` | Ürün satın alınabilir mi? |
| `IsPublished` | `bool` | `Boolean` | Hayır | `true` | Ürün yayında mı? `false` ise taslak modunda gizlidir. |
| **— Mali Bilgiler —** | | | | | |
| `TaxRate` | `int` | `Int32` | Hayır | `20` | KDV oranı (Örn: `20` = %20). Fatura hesabı için zorunludur. |
| **— Lojistik Bilgiler —** | | | | | |
| `Weight` | `double?` | `Double` | Evet | `null` | Kg cinsinden ağırlık. Kargo fiyat hesabı için. |
| `Desi` | `double?` | `Double` | Evet | `null` | Hacimsel ağırlık (Desi). Nakliye şirketi fiyatlandırması için. |
| **— SEO Alanları —** | | | | | |
| `Slug` | `string` | `String` | Hayır | `""` | URL-dostu temiz link (Örn: `alice-arka-fon`). Benzersiz olmalıdır. |
| `MetaTitle` | `string?` | `String` | Evet | `null` | Google arama sonuçlarındaki sekme başlığı (SEO). |
| `MetaDescription` | `string?` | `String` | Evet | `null` | Google arama sonuçlarındaki açıklama metni (SEO). |
| **— Dış Anahtarlar —** | | | | | |
| `CategoryId` | `Guid` | `String` | Hayır | — | Ürünün ait olduğu kategori → `Categories._id` |
| `BrandId` | `Guid?` | `String` | Evet | `null` | Bağlı marka kimliği → `Brands._id` (Markalı olmayan ürünler için null) |
| `MeasurementId` | `Guid` | `String` | Hayır | — | Ürünün ölçü birimi → `Measurements._id` |
| *(+ BaseEntity alanları)* | — | — | — | — | Ortak audit alanları. |

---

### Koleksiyon: `ProductDetails` — Ürün Teknik Detayları

**Entity:** `Store.Catalog/Entities/ProductDetail.cs`
**Miras:** `BaseEntity`
**Amaç:** Ürün teknik özelliklerini, fiziksel boyutlarını ve arama anahtar kelimelerini `Products` koleksiyonundan ayrı tutarak ana belgeyi hafif bırakır.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Açıklama |
|:---|:---|:---|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | Primary Key. |
| `ProductId` | `Guid` | `String` | Hayır | Bağlı ürün → `Products._id` |
| `Description` | `string` | `String` | Hayır | Teknik özellik açıklama metni. |
| `Feature` | `string` | `String` | Hayır | Katalogda öne çıkarılacak özellik (Örn: `Paslanmaz Çelik`). |
| `Dimension` | `string` | `String` | Hayır | Uzamsal ebat (Örn: `120 x 80 x 5 cm`). |
| `AdditionalInfo` | `string` | `String` | Hayır | Montaj, kurulum veya paket içeriği ek bilgisi. |
| `Tag` | `string` | `String` | Hayır | Arama motorları için virgülle ayrılmış anahtar kelimeler. |
| *(+ BaseEntity alanları)* | — | — | — | Ortak audit alanları. |

> **Not:** `Product? Product` navigation property'si `[BsonIgnore]` ile işaretlenmiştir; MongoDB'ye circular reference oluşmasını engeller.

---

### Koleksiyon: `ProductImages` — Ürün Görselleri

**Entity:** `Store.Catalog/Entities/ProductImage.cs`
**Miras:** `BaseEntity`
**Amaç:** Ürünlere ait CDN/S3 görsel URL'lerini, galeri sırasını ve erişilebilirlik meta verilerini tutar.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Açıklama |
|:---|:---|:---|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | Primary Key. |
| `ProductId` | `Guid` | `String` (`BsonRepresentation`) | Hayır | Görselin ait olduğu ürün → `Products._id` |
| `ImageUrl` | `string` | `String` | Hayır | AWS S3 veya CDN üzerindeki tam görsel URL'si. |
| `IsMain` | `bool` | `Boolean` | Hayır | `true` ise bu ürünün kapak (vitrin) fotoğrafıdır. |
| `DisplayOrder` | `int` | `Int32` | Hayır | Galeri içindeki gösterim sırası (küçük değer = önce). |
| `AltText` | `string` | `String` | Hayır | Görme engelli kullanıcılar ve SEO için resim alternatif metni. |
| *(+ BaseEntity alanları)* | — | — | — | Ortak audit alanları. |

---

### Koleksiyon: `ProductPrices` — Ürün Fiyat Listeleri

**Entity:** `Store.Catalog/Entities/ProductPrice.cs`
**Miras:** `BaseEntity`
**Amaç:** Bir ürünün farklı satış stratejilerine (maliyet, perakende, kiralama, toptan) ve dönemlere (kampanya başlangıç/bitiş tarihleri) göre çok boyutlu fiyat listesini tutar.

| Alan Adı | C# Tipi | MongoDB BSON Tipi | Nullable | Varsayılan | Açıklama |
|:---|:---|:---|:---:|:---:|:---|
| `_id` / `Id` | `Guid` | `String` | Hayır | `Guid.NewGuid()` | Primary Key. |
| `ProductId` | `Guid` | `String` | Hayır | — | Fiyatın ait olduğu ürün → `Products._id` |
| `CurrencyId` | `Guid` | `String` | Hayır | — | Para birimi → `Currencies._id` |
| `PriceType` | `PriceType` (Enum) | `String` | Hayır | — | Fiyat stratejisi: `Cost(1)`, `Retail(2)`, `Rental(3)`, `Wholesale(4)`, `Wholerental(5)`. |
| `PriceListTierId` | `Guid?` | `String` | Evet | `null` | B2B bayi kademesi kimliği. `null` = genel liste fiyatı. |
| `MinimumQuantity` | `int` | `Int32` | Hayır | `1` | Fiyatın geçerli olması için gereken minimum sepet adedi. |
| `Amount` | `decimal` | `Decimal128` | Hayır | — | Geçerli asıl satış/kiralama tutarı. |
| `DiscountedAmount` | `decimal?` | `Decimal128` | Evet | `null` | Aktif kampanya varsa indirimlı tutar. |
| `IsTaxIncluded` | `bool` | `Boolean` | Hayır | `true` | KDV'nin bu fiyata dahil olup olmadığı. |
| `StartDate` | `DateTimeOffset?` | `DateTime` | Evet | `null` | Kampanya/fiyatın başlangıç tarihi. |
| `EndDate` | `DateTimeOffset?` | `DateTime` | Evet | `null` | Kampanya bitiş tarihi. `null` = kalıcı (süresiz) fiyat. |
| *(+ BaseEntity alanları)* | — | — | — | — | Ortak audit alanları. |

---

## 4. LOCATION MİKROSERVİSİ — PostgreSQL (`StoreLocationDb`)

**Bağlantı:** `Server=localhost;Port=5432;Database=StoreLocationDb;Username=postgres;Password=***`
**ORM:** Entity Framework Core + Npgsql Provider
**DbContext:** `Services/Location/Infrastructure/Store.Location.Persistence/Context/LocationContext.cs`
**Mimari:** Clean Architecture (Domain / Application / Infrastructure/Persistence)
**Kimlik Stratejisi:** `Guid` (PostgreSQL'de `UUID` tipine haritalı)
**Global Query Filter:** Tüm `BaseEntity` türevlerine `IsDeleted == false` filtresi otomatik uygulanır.

---

### Tablo: `Countries` — Ülkeler

**Entity:** `Store.Location.Domain/Entities/Country.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Cities` koleksiyonu ile 1:N (Bir ülkenin birden fazla şehri)

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `CountryName` | `string` | `varchar(100)` | `NOT NULL` | Ülkenin resmi adı (Örn: `Türkiye`). |
| `CountryCode` | `string?` | `varchar(5)` | Nullable | ISO 3166 kodu (Örn: `TR`, `DE`). |
| `UPhoneCode` | `string?` | `varchar(10)` | Nullable | Uluslararası telefon ön kodu (Örn: `+90`). |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `Cities` — Şehirler

**Entity:** `Store.Location.Domain/Entities/City.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Countries` ile N:1, `Districts` ile 1:N

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `CityName` | `string` | `varchar(100)` | `NOT NULL` | Şehrin adı (Örn: `İzmir`, `İstanbul`). |
| `PlateCode` | `string?` | `varchar(10)` | Nullable | Türkiye trafik plaka kodu (Örn: `35`). Uluslararası sistemler için boş geçilebilir. |
| `CountryId` | `Guid` | `uuid` | `FK → Countries.Id`, `NOT NULL` | Bağlı ülke. |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `Districts` — İlçeler

**Entity:** `Store.Location.Domain/Entities/District.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Cities` ile N:1, `Neighborhoods` ile 1:N

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `DistrictName` | `string` | `varchar(100)` | `NOT NULL` | İlçenin adı (Örn: `Karşıyaka`, `Kadıköy`). |
| `CityId` | `Guid` | `uuid` | `FK → Cities.Id`, `NOT NULL` | Bağlı şehir. |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `Neighborhoods` — Mahalleler

**Entity:** `Store.Location.Domain/Entities/Neighborhood.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Districts` ile N:1. `AddressBase`'deki `NeighborhoodId` Foreign Key ile adreslere bağlanır.

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `NeighborhoodName` | `string` | `varchar(150)` | `NOT NULL` | Mahallenin adı (Örn: `Bostanlı Mahallesi`). |
| `PostalCode` | `string?` | `varchar(20)` | Nullable | 5 haneli posta kodu (Örn: `35590`). |
| `DistrictId` | `Guid` | `uuid` | `FK → Districts.Id`, `NOT NULL` | Bağlı ilçe. |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `ContactInfos` — İletişim Bilgileri

**Entity:** `Store.Location.Domain/Entities/ContactInfo.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Companies.MainContactId`, `Branches.ContactInfoId` ve `Dealers.ContactInfoId` tarafından referans alınan paylaşımlı bir iletişim tablosudur.

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `Email` | `string` | `varchar(150)` | — | E-posta adresi. |
| `Phone` | `string` | `varchar(30)` | — | Sabit telefon numarası. |
| `MobilePhone` | `string` | `varchar(30)` | — | Cep telefonu numarası. |
| `AuthorizedPerson` | `string` | `varchar(100)` | — | Yetkili kişinin adı soyadı. |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `Companies` — Şirketler

**Entity:** `Store.Location.Domain/Entities/Company.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Branches` ile 1:N, `ContactInfos` ile N:1

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `LegalName` | `string` | `varchar(250)` | `NOT NULL` | Faturadaki resmi ticari unvan. |
| `BrandName` | `string` | `varchar(250)` | `NOT NULL` | Piyasada bilinen marka adı. |
| `TaxOffice` | `string` | `varchar(100)` | — | Bağlı vergi dairesi. |
| `TaxNumber` | `string` | `varchar(50)` | — | Vergi Kimlik Numarası (VKN). |
| `TradeRegistryNumber` | `string` | `varchar(100)` | — | Ticaret Sicil Numarası. |
| `MersisNumber` | `string` | `varchar(100)` | — | MERSİS numarası. |
| `Website` | `string` | `varchar(200)` | — | Kurumsal web sitesi URL. |
| `LogoUrl` | `string` | `varchar(500)` | — | Şirket logosunun CDN bağlantı yolu. |
| `MainContactId` | `Guid?` | `uuid` | `FK → ContactInfos.Id`, Nullable | Ana iletişim kaydı. |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `Branches` — Şubeler / Tesisler

**Entity:** `Store.Location.Domain/Entities/Branch.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Companies` ile N:1, `BranchAddresses` ile 1:N, `Warehouses` ile 1:N

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `Name` | `string` | `varchar(150)` | `NOT NULL` | Şubenin adı (Örn: `Karşıyaka Merkez Mağaza`). |
| `BranchType` | `string` | `varchar(100)` | — | Faaliyet türü (Örn: `Mağaza`, `Depo`, `Üretim`). |
| `CompanyId` | `Guid` | `uuid` | `FK → Companies.Id`, `NOT NULL` | Bağlı ana şirket. |
| `ContactInfoId` | `Guid?` | `uuid` | `FK → ContactInfos.Id`, Nullable | Şube iletişim bilgisi. |
| `GeoLocationCoordinates` | `string?` | `varchar(100)` | Nullable | GPS koordinatları `Enlem,Boylam` formatında (Örn: `38.456,27.123`). |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `BranchAddresses` — Şube / Depo Adresleri

**Entity:** `Store.Location.Domain/Entities/BranchAddress.cs`
**Miras:** `AddressBase` → `BaseEntity`
**Amaç:** TPC mimarisi ile `AddressBase`'in tüm alanları bu tabloya dahil edilir. Şirketin kendi işlettiği fiziksel tesislerin adreslerini tutar.

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Kaynak | Açıklama |
|:---|:---|:---|:---|:---:|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | BaseEntity | Primary Key. |
| `Title` | `string` | `varchar(100)` | `NOT NULL` | AddressBase | Adres başlığı (Örn: `Merkez Depo Girişi`). |
| `ContactFullName` | `string` | `varchar(150)` | `NOT NULL` | AddressBase | Teslimat/iletişim yetkilisi. |
| `PhoneNumber` | `string` | `varchar(30)` | `NOT NULL` | AddressBase | İletişim telefonu. |
| `AddressDetail` | `string` | `varchar(500)` | `NOT NULL` | AddressBase | Cadde, sokak, bina gibi açık adres. |
| `CityId` | `Guid` | `uuid` | `FK → Cities.Id` | AddressBase | Şehir referansı. |
| `DistrictId` | `Guid` | `uuid` | `FK → Districts.Id` | AddressBase | İlçe referansı. |
| `NeighborhoodId` | `Guid` | `uuid` | `FK → Neighborhoods.Id` | AddressBase | Mahalle referansı. |
| `BranchId` | `Guid` | `uuid` | `FK → Branches.Id`, `NOT NULL` | BranchAddress | Adresin ait olduğu şube/depo. |
| `LoadingDockInstructions` | `string?` | `varchar(500)` | Nullable | BranchAddress | Tır yanaşma ve boşaltma rampası talimatı. |
| *(+ BaseEntity audit alanları)* | — | — | — | BaseEntity | 9 ortak audit sütunu. |

---

### Tablo: `Dealers` — Bayiler (B2B Müşterileri)

**Entity:** `Store.Location.Domain/Entities/Dealer.cs`
**Miras:** `BaseEntity`
**Amaç:** Toptan alım/kiralama yapan B2B müşteri profillerini (bayi hesaplarını) tutar. Kredi limiti ve cari bakiye ile açık hesap yönetimi yapılır.

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `CompanyName` | `string` | `varchar(250)` | `NOT NULL` | Bayinin ticari unvanı veya şahıs firma adı. |
| `TaxNumber` | `string` | `varchar(50)` | `NOT NULL` | Fatura kesimi için Vergi Kimlik Numarası. |
| `DiscountRate` | `decimal` | `numeric` | — | Bayiye otomatik uygulanan iskonto oranı (Örn: `0.15` = %15). |
| `CreditLimit` | `decimal` | `numeric` | — | Açık hesap çalışma için maksimum kredi limiti. |
| `CurrentBalance` | `decimal` | `numeric` | — | Bayinin anlık borç/alacak bakiyesi (negatif = alacak). |
| `ContactInfoId` | `Guid` | `uuid` | `FK → ContactInfos.Id`, `NOT NULL` | Bayi iletişim bilgileri. |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

### Tablo: `UserAddresses` — Müşteri / Kullanıcı Adresleri

**Entity:** `Store.Location.Domain/Entities/UserAddress.cs`
**Miras:** `AddressBase` → `BaseEntity`
**Amaç:** Hem B2C (bireysel) hem B2B (kurumsal) müşterilerin teslimat ve fatura adreslerini KVKK uyumlu şekilde tutar. `AnonymizeData()` metodu ile kişisel veriler anonimleştirilebilir.

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Kaynak | Açıklama |
|:---|:---|:---|:---|:---:|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | BaseEntity | Primary Key. |
| `Title` | `string` | `varchar(100)` | `NOT NULL` | AddressBase | Örn: `Ev Adresi`, `İş Adresi`. |
| `ContactFullName` | `string` | `varchar(150)` | `NOT NULL` | AddressBase | Teslimat alıcısının adı soyadı. KVKK'da anonimleştirilir. |
| `PhoneNumber` | `string` | `varchar(30)` | `NOT NULL` | AddressBase | Alıcı telefonu. KVKK'da maskelenir. |
| `AddressDetail` | `string` | `varchar(500)` | `NOT NULL` | AddressBase | Açık adres. KVKK'da "silinmiştir" yazısı ile değiştirilir. |
| `CityId` | `Guid` | `uuid` | `FK → Cities.Id` | AddressBase | Şehir. |
| `DistrictId` | `Guid` | `uuid` | `FK → Districts.Id` | AddressBase | İlçe. |
| `NeighborhoodId` | `Guid` | `uuid` | `FK → Neighborhoods.Id` | AddressBase | Mahalle. |
| `CustomerId` | `Guid` | `uuid` | `NOT NULL` | UserAddress | Adresin sahibi kullanıcının kimliği (IdentityServer User ID). |
| `InvoiceType` | `InvoiceType` (Enum) | `integer` | `NOT NULL` | UserAddress | `Individual(1)` = Bireysel, `Corporate(2)` = Kurumsal fatura türü. |
| `IdentityNumber` | `string?` | `varchar(11)` | Nullable | UserAddress | T.C. Kimlik No. Bireysel fatura için. Şifrelenmiş saklanmalıdır. |
| `TaxOffice` | `string?` | `varchar(100)` | Nullable | UserAddress | Kurumsal fatura vergi dairesi. |
| `TaxNumber` | `string?` | `varchar(50)` | Nullable | UserAddress | Kurumsal fatura vergi numarası. |
| `CompanyId` | `Guid?` | `uuid` | `FK → Companies.Id`, Nullable | UserAddress | Kurumsal adresin bağlı olduğu bayi/şirket. |
| `IsAnonymized` | `bool` | `boolean` | — | UserAddress | KVKK anonimleştirme bayrağı. `true` = kişisel veriler silinmiştir. |
| *(+ BaseEntity audit alanları)* | — | — | — | BaseEntity | 9 ortak audit sütunu. |

---

### Tablo: `Warehouses` — Depolar

**Entity:** `Store.Location.Domain/Entities/Warehouse.cs`
**Miras:** `BaseEntity`
**İlişkiler:** `Branches` ile N:1 (isteğe bağlı)

| Sütun Adı | C# Tipi | PostgreSQL Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `Guid` | `uuid` | `PK`, `NOT NULL` | Birincil anahtar. |
| `Name` | `string` | `varchar(150)` | `NOT NULL` | Deponun adı (Örn: `İzmir Bornova Lojistik Merkezi`). |
| `Code` | `string?` | `varchar(50)` | Nullable | Depoya atanan kısa işletme kodu (Örn: `WH-01`). |
| `Address` | `string?` | `varchar(500)` | Nullable | Deponun açık adres metni. |
| `City` | `string?` | `varchar(100)` | Nullable | Deponun bulunduğu şehir adı (düz metin). |
| `Phone` | `string?` | `varchar(30)` | Nullable | Depo operasyon iletişim telefonu. |
| `IsDefault` | `bool` | `boolean` | — | Sistemin varsayılan (ana) deposu mu? Yalnızca bir depo `true` olabilir. |
| `BranchId` | `Guid?` | `uuid` | `FK → Branches.Id`, Nullable | Deponun bağlı olduğu şube (bağımsız depolar için null). |
| *(+ BaseEntity alanları)* | — | — | — | 9 ortak audit sütunu. |

---

## 5. DISCOUNT MİKROSERVİSİ — MS SQL Server (`StoreDiscountDb`)

**Bağlantı:** `Server=(localdb)\MSSQLLocalDB; Database=StoreDiscountDb; integrated security=true;`
**ORM / Erişim:** Dapper (ham SQL sorguları) + EF Core (`DapperContext : DbContext` olarak tanımlanmıştır)
**DbContext:** `Services/Discount/Store.Discount/Context/DapperContext.cs`
**Proje:** `Services/Discount/Store.Discount`
**Mimari:** Tek proje (monolitik mikroservis)
**Kimlik Stratejisi:** `int` — SQL Server `IDENTITY(1,1)` (Auto-Increment)

> **Not:** `Coupon` entity'si herhangi bir `BaseEntity` sınıfından türememektedir. Soft-delete, audit veya aktiflik alanları **yoktur**. Sadece iş alanlarını içerir.

---

### Tablo: `Coupons` — İndirim Kuponları

**Entity:** `Store.Discount/Entities/Coupon.cs`

| Sütun Adı | C# Tipi | SQL Server Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `int` | `INT` | `PK`, `IDENTITY(1,1)`, `NOT NULL` | Otomatik artan birincil anahtar. |
| `Code` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Kullanıcının sepete girebileceği kupon kodu (Örn: `YAZ2026`). |
| `Rate` | `int` | `INT` | — | İndirim yüzdesi (Örn: `10` = %10 indirim). |
| `IsActive` | `bool` | `BIT` | — | Kuponun aktif/kullanılabilir olup olmadığı. |
| `ValidDate` | `DateTime` | `DATETIME2` | — | Kuponun son kullanma tarihi. Bu tarihten sonra sistem tarafından geçersiz sayılır. |

---

## 6. ORDER MİKROSERVİSİ — MS SQL Server (`StoreOrderDb`)

**Bağlantı:** EF Core `AddDbContext` ile `Program.cs`'de konfigüre edilir (LocalDB / SQL Server)
**ORM:** Entity Framework Core
**DbContext:** `Services/Order/Core/Infrastructure/Store.Order.Persistance/Context/OrderContext.cs`
**Mimari:** Clean Architecture (Domain / Application / Infrastructure/Persistence / Presentation)
**Kimlik Stratejisi:** `int` — SQL Server `IDENTITY(1,1)` (Auto-Increment)
**Global Query Filter:** Tüm tablolara `IsDeleted == false` filtresi otomatik uygulanır.

---

### Tablo: `Addresses` — Sipariş Adres Anlık Görüntüsü

**Entity:** `Store.Order.Domain/Entitties/Address.cs`
**Miras:** `BaseEntity` (int kimlikli)
**Amaç:** Sipariş verildiği andaki kargo/teslimat adresini **değişmez (immutable snapshot)** olarak kaydeder. Location servisindeki adres sonradan değişse bile sipariş tarihsel adresini korur.

| Sütun Adı | C# Tipi | SQL Server Tipi | Kısıtlama | Açıklama |
|:---|:---|:---|:---|:---|
| `Id` | `int` | `INT` | `PK`, `IDENTITY(1,1)` | Otomatik artan birincil anahtar. |
| `UserId` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Siparişi veren kullanıcının IdentityServer kimliği. |
| `Country` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Ülke (düz metin snapshot). |
| `City` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Şehir (düz metin snapshot). |
| `District` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | İlçe (düz metin snapshot). |
| `Region` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Bölge/Eyalet (düz metin snapshot). |
| `Street` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Cadde/Sokak (düz metin snapshot). |
| `PostalCode` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Posta kodu. |
| `Detail` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Daire no, kat vb. açık adres detayı. |
| `Phone` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | Alıcı telefon numarası. |
| *(+ BaseEntity alanları)* | — | — | — | `Notes`, `IsActive`, `IsDeleted`, audit tarihleri ve kullanıcı kimlikleri. |

> **Tasarım Kararı:** `Addresses` tablosu, Location servisine foreign key bağı içermez. Bu durum kasıtlıdır: sipariş adresi, Location veritabanı değişse dahi bozulmayan bağımsız bir tarihsel kayıttır (eventual consistency).

---

### Tablo: `Orderings` — Sipariş Master Kartı

**Entity:** `Store.Order.Domain/Entitties/Ordering.cs`
**Miras:** `BaseEntity` (int kimlikli)
**İlişkiler:** `OrdersDetails` ile 1:N

| Sütun Adı | C# Tipi | SQL Server Tipi | Kısıtlama | Varsayılan | Açıklama |
|:---|:---|:---|:---|:---:|:---|
| `Id` | `int` | `INT` | `PK`, `IDENTITY(1,1)` | Auto | Sipariş numarası. |
| `UserId` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | — | Siparişi veren kullanıcının IdentityServer kimliği. |
| `TotalPrice` | `decimal` | `decimal(18,2)` | `NOT NULL` | `0` | KDV ve tüm kalemler dahil sipariş toplam tutarı. |
| `OrderDate` | `DateTime` | `datetime2` | `NOT NULL` | `DateTime.Now` | Siparişin verildiği tarih ve saat. |
| *(+ BaseEntity alanları)* | — | — | — | — | Ortak audit alanları. |

---

### Tablo: `OrdersDetails` — Sipariş Kalem Detayları

**Entity:** `Store.Order.Domain/Entitties/OrderDetail.cs`
**Miras:** `BaseEntity` (int kimlikli)
**İlişkiler:** `Orderings` ile N:1
**Amaç:** Siparişin içindeki her bir ürün kalemi ve bu kalemin sipariş anındaki fiyat/adet bilgilerini tutar.

| Sütun Adı | C# Tipi | SQL Server Tipi | Kısıtlama | Varsayılan | Açıklama |
|:---|:---|:---|:---|:---:|:---|
| `Id` | `int` | `INT` | `PK`, `IDENTITY(1,1)` | Auto | Birincil anahtar. |
| `ProductId` | `string` | `NVARCHAR(MAX)` | `NOT NULL` | `""` | Sipariş edilen ürünün MongoDB ObjectId değeri (Catalog servisi referansı). |
| `ProductPrice` | `decimal` | `decimal(18,2)` | `NOT NULL` | `0` | Ürünün sipariş anındaki birim fiyatı (fiyat snapshot). |
| `ProductAmount` | `int` | `INT` | `NOT NULL` | `0` | Sipariş edilen adet. |
| `TotalPrice` | `decimal` | `decimal(18,2)` | `NOT NULL` | `0` | Kalem tutarı = `ProductPrice x ProductAmount`. |
| `OrderingId` | `int` | `INT` | `FK → Orderings.Id`, `NOT NULL` | `0` | Bağlı sipariş master kartı. |
| *(+ BaseEntity alanları)* | — | — | — | — | Ortak audit alanları. |

---

## 7. ENUM REFERANSLARI

### 7.1 `PriceType` — Fiyat Stratejisi

**Dosya:** `Store.Location.Domain/Entities/PriceType.cs`

MongoDB'de `BsonRepresentation(BsonType.String)` ile metin olarak saklanır.

| Değer | Tamsayı | Açıklama |
|:---|:---:|:---|
| `Cost` | `1` | Ürünün firmaya geliş veya üretim **maliyeti**. Raporlama ve kâr marjı hesabı için kullanılır. |
| `Retail` | `2` | Son kullanıcıya (B2C) sunulan **perakende satış** fiyatı. |
| `Rental` | `3` | Organizasyonlara sunulan günlük/dönemlik **kiralama** fiyatı. |
| `Wholesale` | `4` | Bayilere/toptancılara (B2B) sunulan özel **toptan satış** fiyatı. |
| `Wholerental` | `5` | Bayilere/toptancılara (B2B) sunulan özel **toptan kiralama** fiyatı. |

---

### 7.2 `InvoiceType` — Fatura Türü

**Dosya:** `Store.Location.Domain/Entities/InvoiceType.cs`

PostgreSQL'de `integer` olarak saklanır.

| Değer | Tamsayı | Açıklama |
|:---|:---:|:---|
| `Individual` | `1` | **Bireysel** fatura. T.C. Kimlik No ile kesilir. |
| `Corporate` | `2` | **Kurumsal** fatura. Vergi No + Vergi Dairesi bilgisi gerektirir. |

---

## 8. SERVİSLER ARASI İLİŞKİ HARİTASI

Mikroservis mimarisinde her servis kendi veritabanına sahip olduğundan, servisler arası referanslar **Foreign Key kısıtlaması olmadan** yalnızca ID değerleri (string/Guid) olarak tutulur. Bu ilişkiler veritabanı düzeyinde değil, **uygulama/servis katmanında** çözümlenir.

```
+------------------------------------------------------------------+
|                         STORE.SLN                                |
|                                                                  |
|  +-------------------+      ID (string)     +----------------+   |
|  | Catalog (MongoDB) |<------------------->| Order (MSSQL)  |   |
|  |                   |   OrderDetail.      |                |   |
|  | Products._id      |   ProductId         | OrdersDetails  |   |
|  | (ObjectId/Guid)   |                     | .ProductId     |   |
|  +-------------------+                     +----------------+   |
|                                                                  |
|  +-------------------+                     +----------------+   |
|  | Location (PgSQL)  |                     |Discount(MSSQL) |   |
|  |                   |                     |                |   |
|  | UserAddresses     |<-- (Uygulama Kat.)  | (Bağımsız)     |   |
|  | .CustomerId       |    IdentityServer   |                |   |
|  +-------------------+    User ID          +----------------+   |
|           |                                                      |
|           |  Tüm servisler kullanıcı kimliği için               |
|           +---------> IdentityServer (OAuth 2.0 / OIDC)         |
|                        https://localhost:5001                    |
+------------------------------------------------------------------+
```

### Servisler Arası ID Eşleme Tablosu

| Kaynak Servis | Kaynak Alan | Hedef Servis | Hedef Alan | Çözümleme Yöntemi |
|:---|:---|:---|:---|:---|
| **Order** | `OrderDetail.ProductId` (string) | **Catalog** | `Products._id` (Guid/string) | HTTP API çağrısı |
| **Order** | `Ordering.UserId` (string) | **IdentityServer** | `User.Id` | JWT Token Claim |
| **Location** | `UserAddress.CustomerId` (Guid) | **IdentityServer** | `User.Id` | Uygulama katmanı eşleme |
| **Location** | `UserAddress.CompanyId` (Guid?) | **Location** | `Companies.Id` | Aynı veritabanı FK |
| **Catalog** | `ProductPrice.CurrencyId` (Guid) | **Catalog** | `Currencies._id` | Aynı MongoDB DB referansı |
| **Catalog** | `Product.CategoryId` (Guid) | **Catalog** | `Categories._id` | Aynı MongoDB DB referansı |
| **Catalog** | `Product.BrandId` (Guid?) | **Catalog** | `Brands._id` | Aynı MongoDB DB referansı |
| **Catalog** | `Product.MeasurementId` (Guid) | **Catalog** | `Measurements._id` | Aynı MongoDB DB referansı |

---

*Bu belge, `Store.sln` çözümündeki entity sınıfları (`.cs`), DbContext tanımları ve `appsettings.json` konfigürasyonları doğrudan okunarak oluşturulmuştur. Kaynak kod değiştiğinde bu belgenin güncellenmesi önerilir.*
