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
