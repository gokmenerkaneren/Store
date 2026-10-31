# DATABASE MAP — STORE.SLN VERİTABANI TOPOLOJİSİ
# ═══════════════════════════════════════════════════
# Hangi servisin hangi veritabanını, koleksiyonu ve tabloyu yönettiğini tanımlar.
# Entity eklerken, migration yazarken ve sorgu tasarlarken bu dosyaya başvurulur.
#
# Son Güncelleme: 23 Eylül 2026

---

## 1. VERİTABANI GENEL HARİTASI

| # | Servis | DBMS | Veritabanı Adı | Bağlantı | ID Tipi |
|:--|:---|:---|:---|:---|:---|
| 1 | **Store.Catalog** | MongoDB 7.x | `StoreMD` | `mongodb://localhost:27017` | `Guid` |
| 2 | **Store.Location** | PostgreSQL 16.x | `StoreLocationDb` | `localhost:5432` | `Guid` |
| 3 | **Store.B2B** | PostgreSQL 16.x | `StoreB2BDb` | `localhost:5432` | `Guid` |
| 4 | **Store.Order** | PostgreSQL 16.x | `StoreOrderDb` | `localhost:5432` | `Guid` |
| 5 | **Store.Discount** | PostgreSQL 16.x | `StoreDiscountDb` | `localhost:5432` | `Guid` |
| 6 | **Store.IdentityServer** | PostgreSQL 16.x | `StoreIdentityDB` | `localhost:5432` | IdentityServer4 |
| 7 | **Store.Favorite** | PostgreSQL 16.x | `StoreFavoriteDb` | `localhost:5432` | `Guid` |
| 8 | **Store.Rental** | PostgreSQL 16.x | `StoreRentalDb` | `localhost:5432` | `Guid` |
| 9 | **Store.Basket** | Redis | — | `localhost:6379` | Key-Value |

---

## 2. STORE.CATALOG (MongoDB — StoreMD)

### Ortak BaseEntity Alanları (Tüm koleksiyonlarda miras)
`Id (Guid)`, `Notes`, `IsActive`, `IsDeleted`, `InsertUserId`, `InsertDate`, `UpdateUserId`, `UpdateDate`, `DeleteUserId`, `DeleteDate`

### Koleksiyon Listesi

| Koleksiyon | Açıklama | Özel Alanlar |
|:---|:---|:---|
| **Products** | Ana ürün kartları | ProductCode, Barcode, Name, Slug, CategoryId, BrandId, MeasurementId, TaxRate, Satis, Kiralama, IsPublished, Weight, Desi, Width, Height, Depth, MetaTitle, MetaDescription, Attributes[], Variants[], ActivePrices[], Tags[] |
| **Categories** | Ürün kategorileri (hiyerarşik) | ParentId, Code, Name |
| **Brands** | Ürün markaları | Code, Name |
| **Measurements** | Ölçü birimleri | Code, Name, Symbol |
| **Currencies** | Para birimleri | Code, Name, Symbol |
| **Colors** | Renk lookup tablosu | Name, HexCode |
| **Sizes** | Beden lookup tablosu | Name, Code |
| **PriceTypes** | Fiyat türü lookup | Name, Code |
| **ProductImages** | Ürün görselleri | ProductId, ImageUrl, IsMain, DisplayOrder, AltText |
| **ProductPrices** | Ürün fiyatları | ProductId, CurrencyId, PriceType, Amount, DiscountedAmount, MinimumQuantity, IsTaxIncluded, StartDate, EndDate |
| **DefaultSlider1s** | Ana sayfa slider | ImageUrl, OrderIndex |
| **DefaultTrends** | Trend ürünler vitrini | ProductId, OrderIndex |
| **BestSellers** | En çok satanlar vitrini | ProductId, OrderIndex |
| **NewProducts** | Yeni ürünler vitrini | ProductId, OrderIndex |
| **InstagramPosts** | Instagram akışı | ImageUrl, PostUrl, OrderIndex |

### Gömülü Dokümanlar (Embedded Documents)
- `Product.Attributes[]` → `ProductAttribute` (AttributeName, AttributeValue)
- `Product.Variants[]` → `ProductVariant` (ColorId, SizeId, StockQuantity, SKU)
- `Product.ActivePrices[]` → `ProductPrice` (yukarıdaki fiyat yapısı)

---

## 3. STORE.LOCATION (PostgreSQL — StoreLocationDb)

### Tablo Listesi

| Tablo | Açıklama | Foreign Keys |
|:---|:---|:---|
| **Countries** | Ülkeler | — |
| **Cities** | Şehirler | CountryId → Countries |
| **Districts** | İlçeler | CityId → Cities |
| **Neighborhoods** | Mahalleler | DistrictId → Districts |
| **Companies** | Şirketler | MainContactId → ContactInfos |
| **Branches** | Şubeler | CompanyId → Companies, ContactInfoId → ContactInfos |
| **BranchAddresses** | Şube adresleri | BranchId → Branches, CityId, DistrictId, NeighborhoodId |
| **UserAddresses** | Müşteri adresleri | CityId, DistrictId, NeighborhoodId, CompanyId → Companies |
| **Warehouses** | Depolar | BranchId → Branches |
| **ContactInfos** | İletişim bilgileri | — |
| **Dealers** | Bayiler (temel bilgiler) | ContactInfoId → ContactInfos |

### Önemli Notlar
- `UserAddresses.IdentityNumber` → KVKK kapsamında veritabanında AES-256 CBC şifrelidir (`varchar(256)`). EF Core `EncryptedStringConverter` üzerinden otomatik şifrelenir/çözülür.
- `Dealers` tablosu yalnızca temel bilgileri tutar; finansal veriler `Store.B2B`'dedir
- Global Query Filter: `IsDeleted = false` tüm sorgulara otomatik uygulanır (Soft delete aktif)

---

## 4. STORE.B2B (PostgreSQL — StoreB2BDb)

| Tablo | Açıklama | Özel Alanlar |
|:---|:---|:---|
| **DealerFinancials** | Bayi finansal verileri | TaxNumber (UNIQUE), TaxOffice, DiscountRate, CreditLimit, CurrentBalance |

---

## 5. STORE.ORDER (PostgreSQL — StoreOrderDb)

| Tablo | Açıklama | ID Tipi | Foreign Keys |
|:---|:---|:---|:---|
| **Orderings** | Sipariş master kartı | Guid | — |
| **OrdersDetails** | Sipariş kalemleri | Guid | OrderingId → Orderings |
| **Addresses** | Sipariş adresleri (dondurulmuş) | Guid | — |

### Önemli Notlar
- Order BaseEntity'sinde ID tipi `Guid` olarak güncellenmiştir.

---

## 6. STORE.DISCOUNT (PostgreSQL — StoreDiscountDb)

| Tablo | Açıklama | ID Tipi |
|:---|:---|:---|
| **Coupons** | İndirim kuponları | Guid |

### Önemli Notlar
- Dapper + EF Core hibrit erişim
- `DeleteCouponAsync` soft-delete uygular (`IsDeleted = true, IsActive = false`)
- `Code` ve `IsActive` alanları üzerinde indeksleme mevcuttur

---

## 7. STORE.IDENTITYSERVER (PostgreSQL — StoreIdentityDB)

IdentityServer4 tarafından yönetilir. OAuth 2.0 / OpenID Connect konfigürasyonu, kullanıcı ve client tanımları bu veritabanında tutulur.

---

## 8. STORE.FAVORITE (PostgreSQL — StoreFavoriteDb)

| Tablo | Açıklama | ID Tipi |
|:---|:---|:---|
| **FavoriteItems** | Kullanıcı favori ürün listesi | Guid |

### Önemli Notlar
- `UserId` ve `ProductId` zorunlu alanlardır.
- Clean Architecture kullanılarak `MediatR` ile komutlar işlenir.
- `UserId` kontrolü JWT token claims üzerinden IDOR korumalıdır.

---

## 9. STORE.RENTAL (PostgreSQL — StoreRentalDb)

| Tablo | Açıklama | ID Tipi |
|:---|:---|:---|
| **RentalBookings** | Ürün kiralama rezervasyonları | Guid |

### Önemli Notlar
- `ProductId`, `UserId`, `StartDate`, `EndDate`, `TotalPrice` alanlarını barındırır.
- Kiralama rezervasyon çakışması (`Conflict Check`) iş mantığı rezervasyon öncesinde doğrulanır.
- MRP yerine e-ticaret tabanlı stok/kira periyodu rezervasyonu sınırlarıyla yönetilir.

---

## 10. STORE.BASKET (Redis)

- Key-Value yapısında geçici sepet verisi tutulur.
- Bellek sızıntısını ve atıl anahtar birikmesini önlemek için 30 günlük TTL (`TimeSpan.FromDays(30)`) uygulanır.

---

## 11. MİGRATION STANDARDI

### EF Core Migration Komutu
```bash
dotnet ef migrations add <MigrationAdi> \
  --project Services/<Servis>/Store.<Servis>.Persistence \
  --startup-project Services/<Servis>/Store.<Servis>.WebApi
```

### Kurallar
- Migration dosyaları ilgili servisin `Persistence/Migrations/` klasörüne yazılır
- Farklı bir konuma kesinlikle yazılmaz
- Her yeni tablo/kolon eklenmeden önce mevcut DbContext konfigürasyonları kontrol edilir
- PostgreSQL view güncellemeleri migration içinde `CREATE OR REPLACE VIEW` ile yapılır

---

> **Güncelleme Kuralı:** Yeni entity/tablo eklendiğinde, mevcut şema değiştiğinde veya yeni bir veritabanı oluşturulduğunda bu doküman güncellenir.

---

## 11. ORTAM SENKRONİZASYON KURALI (Local & Docker)

> [!WARNING]  
> **Kritik Kural:** Projede iki farklı PostgreSQL ortamı bulunmaktadır:  
> 1. **Local Ortam (Host):** `localhost:5432`  
> 2. **Docker Ortamı (Container):** `localhost:5434` (docker-compose üzerinden)  
>  
> Veritabanı şemasında (Migration), veri modellerinde veya tabloların yapısında yapılan **TÜM DEĞİŞİKLİKLER (Update/Migration)** her iki ortamda da **BİREBİR AYNI (Senkron)** olmak zorundadır.  
> Herhangi bir serviste veritabanı güncellemesi (`dotnet ef database update`) yapıldığında, işlem mutlaka her iki port/ortam hedef alınarak tekrarlanmalıdır. Bu kural, ortamlar arası tutarsızlığı (schema mismatch) kesin olarak engellemek içindir.


### MassTransit EF Core Outbox Tabloları (StoreOrderDb)
- `InboxState`: Gelen mesajların tekillik (idempotency) takibi.
- `OutboxMessage`: Transaction içinde kaydedilen ve RabbitMQ'ya iletilecek mesaj kuyruğu.
- `OutboxState`: Outbox kuyruk işleyicisinin durum bilgisi.


### Özel Kısıtlar (Constraints)
- `EXCLUDE_RentalOrderItems_ProductId_DateRange`: PostgreSQL GiST EXCLUDE constraint (`ProductId` WITH =, `tstzrange(StartDate, EndDate)` WITH &&). Çakışan kiralama tarihlerini engeller.
