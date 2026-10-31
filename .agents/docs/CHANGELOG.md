# CHANGELOG — VERSİYON VE DEĞİŞİKLİK GEÇMİŞİ
# ══════════════════════════════════════════════════
# Proje genelinde sprint bazlı geliştirilen modüllerin ve düzeltilen hataların listesi.

---

## [Unreleased]
### Added
- **Sepet (Basket) Mimarisi ve Sağ Çekmece Entegrasyonu (TASK-BASKET-001):**
  - `Store.DtoLayer` altına `BasketDtos` (`BasketType`, `BasketTotalDto`, `BasketItemDto`) eklendi.
  - `Store.Basket` mikroservisine `BasketType` ve `ImageUrl` desteği entegre edildi.
  - `Store.WebUI`'a Refit istemcisi `IBasketApiService` ve AJAX controller'ı `BasketController.cs` eklendi.
  - Satış ve Kiralama siparişlerinin aynı sepette bulunmasını engelleyen HTTP 409 Conflict ve kullanıcı onay akışı kodlandı.
  - Sağ taraftan açılan popup sepet çekmecesi dinamik render ve adet artırma/azaltma/silme yetenekleriyle `wwwroot/js/basket.js` üzerinden bağlandı.
  - Ürün detay sayfasındaki miktar seçimi (`Quantity`) ve 'Sepete Ekle' butonu tam fonksiyonel hale getirildi.
- **Port ve Başlatma Konfigürasyonları Hizalaması (TASK-CONF-002):**
  - `Store.Basket` launchSettings.json ve .csproj.user profilleri düzeltildi (trailing noktalı virgül ve https/http uyumsuzluğu giderildi, `ActiveDebugProfile` `http` yapıldı, Swagger UI otomatik açılışı sağlandı, JWT Bearer desteği eklendi).
  - `Store.Favorite.WebApi` portu `7307` (`http://localhost:7307`) olarak güncellendi (`launchSettings.json`, `.csproj.user`, `.http`, `Store.WebUI/appsettings.json`, dokümanlar).
  - `Store.Rental.WebApi` portu `7308` (`http://localhost:7308`) olarak güncellendi (`launchSettings.json`, `.csproj.user`, `.http`, `Store.WebUI/appsettings.json`, dokümanlar).

---

## [v2.0.0] - 2026-11-01
### Added
- **Paket 7: Kupon / İndirim Servisi Entegrasyonu ve Sunucu Taraflı Checkout:**
  - `Store.DtoLayer` altına `ResultCouponDto` eklendi.
  - `Store.Discount` servisine `GetByCodeCouponAsync` Dapper sorgusu ve anonim `[HttpGet("GetByCode/{code}")]` uç noktası eklendi; kupon süresi ve varlığı kontrolü sağlandı.
  - `Store.Discount` servisindeki Create, Update ve Delete uçlarına `[Authorize(Roles = "Admin")]` yetkilendirmesi getirildi.
  - `Store.Shared` içerisine saf fonksiyonlarla kupon doğrulama ve indirim hesaplama mantığını barındıran `CouponPricing` (`Store.Shared.Pricing`) sınıfı eklendi.
  - `Store.WebUI` üzerine Refit tabanlı `IDiscountApiService` bağlandı; `CheckoutController`'da istemciden gelen indirim ve fiyat manipülasyonu engellendi, tüm fiyat ve kupon doğrulama sunucu tarafına taşındı.
  - `Checkout/Index.cshtml` sayfasına `CartWarning` uyarı alanı entegre edilerek kupon geçersizliği ve son kullanma tarihi bildirimleri kullanıcıya açıldı.
  - `Store.Order.Tests` altına 4 yeni birim testi (`DiscountCalculationTests.cs`) eklenerek toplam test sayısı 108'e yükseltildi.
- **Paket 6: Koreografi Tabanlı Stok Rezervasyonu ve Dağıtık İşlemler:**
  - `Store.Shared` altında `StockReservedIntegrationEvent` ve `StockReservationFailedIntegrationEvent` sözleşmeleri eklendi.
  - `Store.Catalog` servisi `OrderCreatedConsumer` içine varyant stok düşüm mantığı entegre edildi. Çoklu kalemlerde kısmi hata durumunda stok iadesi (rollback) ve `UpdateDate` ile iyimser eşzamanlılık (Optimistic Concurrency) sağlandı.
  - `Store.Order.WebApi` içerisine `StockReservedConsumer` (`OrderStatus.Processing`) ve `StockReservationFailedConsumer` (`OrderStatus.Cancelled` + hata telafi notu) tüketicileri eklenip MassTransit RabbitMQ dinleyicisine kaydedildi.
  - 2 yeni dağıtık tüketici birim testi (`OrderStockConsumersTests.cs`) yazılarak toplam test sayısı 104'e yükseltildi.
- **Paket 5: RabbitMQ & MassTransit EF Core Outbox Altyapısı:**
  - `docker-compose.yml` içerisine `rabbitmq:3-management` ve `rabbitmq_data` volume eklendi.
  - `Store.Shared` altında `OrderCreatedIntegrationEvent` ve `OrderItemMessageDto` sözleşmesi oluşturuldu.
  - `Store.Order` servisine MassTransit PostgreSQL EF Core Outbox entegre edildi (`AddOutboxMessageEntity`, `AddOutboxStateEntity`, `AddInboxStateEntity`) ve `AddMassTransitOutbox` migration'ı üretildi.
  - `Store.Catalog` servisine MassTransit dinleyicisi ve `OrderCreatedConsumer` iskeleti eklendi.
  - Dual-write koruması sağlandı; sipariş ve Outbox kaydı tek transaction içinde atomik kaydedilmektedir.
- **Paket 4: Rental Concurrency & EF Core Migration'ları:**
  - PostgreSQL GiST `EXCLUDE` kısıtı eklendi (`btree_gist`), aynı ürünün çakışan tarihlerde kiralanması DB motoru seviyesinde engellendi.
  - Rental servisine `InitialCreateRentalDb` initial migration'ı eklendi.
  - Order servisine `AddOrderStatusAndOrderDetails` migration'ı üretildi.
  - Catalog servisine anonim `CheckStock/{id}` uç noktası ve WebUI Checkout öncesi temel stok doğrulaması eklendi.
- **Faz 1 Sertleştirme & Order Durum Makinesi:**
  - `Ordering` entity'si içine `CanTransitionTo` ve `ChangeStatus` domain metodları entegre edildi; geçersiz durum geçişleri engellendi.
  - 56 yeni durum makinesi birim testi (`OrderingStateMachineTests.cs`) yazıldı.

### Security
- **K1: UsersController Yetkilendirme:** `[Authorize(Roles = "Admin")]` uygulanarak anonim erişim tamamen engellendi.
- **K2: Location API Koruma:** Global fallback authorization politikası kuruldu; yönetimsel uçlar `[Authorize(Roles = "Admin")]` ile kilitlendi; `UserAddressController` müşteri kimliğini JWT token'dan bağlayarak IDOR açığını kapattı.
- **K3: AES-256 GCM Şifreleme:** `AesEncryptionHelper` ortam değişkeni desteği, katı UTF-8 denetimi ve AES-GCM geçiş altyapısıyla güçlendirildi.
- **K5: Identity Client Ayrımı:** `UyelikController` ve sosyal login bağlantıları `StoreAdminId` yerine `StoreWebClientId`'ye bağlandı.
- **K6: Catalog API Koruma:** `Products`, `Categories`, `ProductPrices`, `ProductImages` vb. controller'ların tüm yazma uçları `[Authorize(Roles = "Admin")]` ile sınırlandırıldı.
- **K7: Order & OrderDetails IDOR Koruması:** `OrderDetailsController` Admin rolüne bağlandı; `OrderingsController.UpdateOrdering` ucunda durum, fiyat ve kullanıcı manipülasyonu engellendi; işleme alınmış siparişlerin silinmesi (`RemoveOrdering`) yasaklandı.
- **K8 & İ9: Checkout Fiyat & Adres Koruması:** `DiscountAmount` sıfırlanarak indirim manipülasyonu engellendi; güvenli guard clause ve `Uyelik` yönlendirmeleri sağlandı.

---

## [v1.3.0] - 2026-10-13
### Added
- **KVKK AES-256 Şifreleme (RV-004):** `AesEncryptionHelper` (Store.Shared), `EncryptedStringConverter` (EF ValueConverter) ve migration ile `UserAddress.IdentityNumber` veritabanında şifrelendi. xUnit testleri yazıldı.
- **Birim Test Kapsamı & Onarımı (RV-010):** `Store.Order.Tests` net8.0'a çekilerek Store.slnx'e bağlandı. Toplam test sayısı 16'ya yükseltildi ve tümü yeşil.
- **ADR-004:** Ağır MRP mimari kapsamdan çıkarıldı; e-ticaret odaklı stok giriş/çıkış ve rezervasyon takibi kararlaştırıldı.
- **Rental WebApi Entegrasyonu (RV-003):** `Store.Rental.WebApi` Program.cs konfigürasyonu tamamlandı ve WebUI'a bağlandı.

### Changed
- **Soft-Delete Bütünlüğü (RV-005):** Discount, B2B, Order ve Location servislerindeki fiziksel silme işlemleri `IsDeleted = true, IsActive = false` soft delete mantığına geçirildi.
- **Catalog Mongo OOM Önleme (RV-006):** RAM'e tüm koleksiyonu yükleyen `Find(_ => true)` sorguları veritabanı seviyesi regex ve tekil sorgularla değiştirildi.
- **Location & Order DI & Async Temizliği (RV-011):** Order WebApi'daki senkron `.Result` çağrısı kaldırıldı. Location WebApi'daki 130 satırlık elle DI kaydı otomatik assembly reflection mekanizmasına dönüştürüldü.
- **WebUI Hesabım Sayfası (RV-007):** 12 seri HTTP isteği `Task.WhenAll` ile paralel çekilerek sayfa açılış performansı artırıldı, `ILogger` eklendi.
- **Redis TTL (RV-009):** `BasketService` sepet anahtarlarına 30 günlük TTL eklendi.

### Security
- **Catalog Write Koruması (RV-001):** Tüm Catalog controller yazma endpoint'leri `[Authorize]` ile korundu, okuma endpoint'leri anonim vitrine açıldı.
- **Sır ve Parola Temizliği (RV-002):** LocationContext'teki hardcoded connection string ve repo içerisindeki `PassWords.txt` dosyası temizlendi, .gitignore'a eklendi.
- **Favorite IDOR Koruması (RV-008):** `FavoritesController`'a kullanıcı token identity doğrulaması eklendi; IDOR açığı kapatıldı.

---

## [v1.2.0] - 2026-10-03
### Added
- `Store.Rental` modülü e-ticaret odaklı kiralama kuralları ile yapılandırıldı.
- `Ajan Sistemi` güncellendi, hafıza dosyaları tam kapsamlı olarak aktif edildi (Business Rules, Database Map, Tech Stack, API Contracts).

### Changed
- Sipariş ID'leri (Orderings) GUID formatına güncellendi.
- `BUSINESS_RULES.md` e-ticaret tabanlı stok takibini yönetecek şekilde güncellendi.

### Fixed
- SSL bypass sınırlandırma (sadece #if DEBUG) tamamlandı.
- Kimlik bilgilerinin appsettings üzerinde plaintext saklanma açığı giderildi.
- tempkey.jwk dosyası git ignore'a eklenerek repo güvenliği sağlandı.

---

## [v1.1.0] - 2026-09-16
### Added
- `Store.Favorite` mikroservisi oluşturuldu ve kullanıma hazır.
- Instagram akışı medya entegrasyonu tamamlandı.

### Changed
- BaseEntity kalıtımı tüm domain katmanlarında standartlaştırıldı.

> **Güncelleme Kuralı:** Geliştirme döngüsü sonunda önemli güncellemeler buraya manuel olarak işlenmelidir.
