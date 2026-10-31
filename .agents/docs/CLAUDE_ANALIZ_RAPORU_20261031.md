# Store.sln — Claude Code Detaylı Analiz ve Uygulama Raporu
**Tarih:** 31 Ekim 2026  
**Oluşturan:** Claude Code CLI (v2.1.316)  
**Yönetici:** Agent Gravity (Kıdemli Proje Yöneticisi & Orkestratör)

---

## 1. Yönetici Özeti (Executive Summary)
Mimari iskelet (Clean Architecture, DDD, servis izolasyonu) oldukça sağlamdır. Ancak proje şu an **üretime hazır değildir**. İki temel engel mevcuttur:
1. **Kritik Güvenlik Açıkları:** `UsersController` ve `Location API` tamamen anonim erişime açıktır; müşteri token'ları ile admin paneli kontrol edilebilir durumdadır; AES anahtarları ve veritabanı/OAuth sırları kodlarda ve açık config dosyalarında yer almaktadır.
2. **Kopuk İş Akışları:** Sipariş akışı uçtan uca çalışmamaktadır (sipariş başarısız olsa da kullanıcıya başarılı gösterilmekte, `OrderDetail` kaydedilmemekte, durum makinesi bulunmamaktadır).

**Stratejik Sıralama:** Önce Güvenlik Sertleştirmesi (Faz 1), ardından Temel İş Akışları (Faz 2), sonrasında Altyapı/DevOps (Faz 3 - YARP, RabbitMQ) ve Ölçekleme (Faz 4).

---

## 2. Güçlü Yanlar (Strong Points)
- **Bağlam İzolasyonu:** Servisler arasında Domain düzeyinde doğrudan `ProjectReference` yoktur; ortak yapılar `Store.Shared` ve `Store.DtoLayer` üzerinden izole edilmiştir.
- **Katmanlı Mimari:** Location, Order, B2B, Favorite ve Rental servisleri Clean Architecture standartlarına (Domain, Application, Persistence, WebApi) uygundur.
- **Doğru Teknoloji Seçimleri:** Catalog (MongoDB + Hangfire), Basket (Redis), Discount (PostgreSQL + Dapper), Identity (IdentityServer4).
- **Global Soft Delete:** `OrderContext` ve `RentalContext` üzerinde `BaseEntity` filtreleri mevcuttur.
- **Favorite IDOR Koruması:** `FavoritesController` ve `BasketsController` kullanıcı kimliğini güvenli biçimde JWT token claim'lerinden almaktadır.
- **WebUI Merkezi İletişim:** Refit ve `BearerTokenHandler` ile merkezi token yönetimi sağlanmıştır.

---

## 3. Zayıf Yanlar ve Teknik Borçlar (Weak Points & Debts)

### 3.1 Kritik Güvenlik Açıkları (K1 - K8)
- **K1 (L3 - Kritik):** `UsersController` tamamen anonimdir. Kullanıcı listeleme, silme, parola değiştirme ve rol atama anonim yapılabilmektedir.
- **K2 (L3 - Kritik):** `Location API` genelinde yetkilendirme yoktur (14 controller'da `[Authorize]` eksiktir). Adresler TCKN ve vergi numaralarıyla anonim okunabilmektedir.
- **K3 (L3 - Kritik):** AES-256 şifreleme anahtarları kaynak kod içerisine hardcoded yazılmıştır (`AesEncryptionHelper.cs`, `EncryptionHelper.cs`).
- **K4 (L2 - Yüksek):** Düz metin sırlar (OAuth credential'ları, veritabanı parolaları, secret'lar) dosyalarda yer almaktadır.
- **K5 (L3 - Kritik):** WebUI müşterileri `StoreAdminId` istemcisi ile giriş yapmakta; `CustomProfileService` rol claim'i üretmemekte ve Admin alanı role dayalı kısıt içermemektedir.
- **K6 (L2 - Yüksek):** Catalog servisindeki bazı yazma uçları (`BrandsController`, `Sizes`, `Colors` vb.) anonimdir.
- **K7 (L2 - Yüksek):** Sipariş ve kiralamada IDOR ve fiyat manipülasyonu riski (`UserId` ve `UnitPrice` istekten manipüle edilebilir).
- **K8 (L2 - Yüksek):** `CheckoutController`'da kullanıcının adresi yoksa sistemdeki ilk adres fallback olarak çekilmekte, başka bir müşterinin adresi ekrana gelebilmektedir.

### 3.2 İş Mantığı Hataları (İ1 - İ10)
- **İ1:** Sipariş backend'de başarısız olsa da hata yutulmakta, sepet silinmekte ve kullanıcıya sipariş başarılı ekranı gösterilmektedir (`CheckoutController.cs:219-232`).
- **İ2:** Sipariş kalemleri (`OrderDetail`) kaydedilmemekte, sipariş kodu ve adres bağı kurulmamaktadır.
- **İ3:** `Ordering` entity'sinde sipariş durum makinesi (`Status` alanı) bulunmamaktadır.
- **İ4:** Tutar sunucuda Catalog'dan teyit edilmemekte, doğrudan sepet verisine güvenilmektedir.
- **İ5:** Kiralama çakışma kontrolü pending siparişleri ve adet bilgisini dikkate almamaktadır (concurrency açığı).
- **İ6:** Rental servisinde migration eksiktir; FluentValidation middleware'e kaydedilmemiştir.
- **İ7:** B2B iskonto ve fiyatlandırma hesaplaması (`CalculatePriceAsync`) akışa bağlı değildir.
- **İ8:** Kupon/Discount akışı WebUI'da bağlı değildir (indirim 0 geçmektedir).
- **İ9:** `CheckoutController` içinde test amaçlı hardcoded kişisel veriler ve GUID'ler yer almaktadır.
- **İ10:** Favori kalp butonu frontend script'i (`favorite.js`) çalışmamaktadır.

---

## 4. Önceliklendirilmiş Yol Haritası (Actionable Roadmap)

### Faz 1: Sertleştirme & Acil Güvenlik (1 - 3 Hafta)
- **İlk 48 Saat Acil Müdahale:**
  1. `UsersController` üzerine `[Authorize(Roles="Admin")]` eklenmesi (K1).
  2. `Location API` genelinde global yetkilendirme ve adreslerin `UserId` token kontrolü (K2).
  3. `Catalog API` açık controller'ların kapatılması (K6).
  4. `CheckoutController` adres sızıntısı (K8) ve gömülü kişisel verilerin (İ9) temizlenmesi.
- **Devam Eden Güvenlik Adımları:**
  5. `StoreWebClient` ayrımı ve `CustomProfileService` rol claim entegrasyonu (K5).
  6. AES anahtarlarının konfigürasyona taşınması (K3).
  7. Token sürelerinin kısaltılması ve kalıcı imza anahtarı.
  8. Yetki regresyon testleri yazılması.

### Faz 2: Temel İş Akışları (6 - 10 Hafta)
1. Sipariş Durum Makinesi (`Ordering.Status`) ve `OrderDetail` tamamlama.
2. Sunucu taraflı güvenli Checkout (`POST /orders/checkout`).
3. Merkezi Stok Yönetimi (ADR ile servis sınırı belirlenerek).
4. Ödeme Entegrasyonu (iyzico / PayTR).
5. Rental yaşam döngüsü ve migration'ların tamamlanması.
6. B2B bayi yönetim arayüzü ve iskonto motoru.

### Faz 3: Altyapı ve DevOps (4 - 6 Hafta)
1. Docker Compose & Dockerfile izolasyonu.
2. YARP API Gateway entegrasyonu.
3. RabbitMQ + MassTransit (Outbox Pattern & Saga).
4. Serilog, OpenTelemetry ve Health Check standartlaştırması.

### Faz 4: Ölçekleme & İleri Yetenekler (Sürekli)
1. Redis Caching & Arama Motoru (Meilisearch/Elasticsearch).
2. Kargo & E-Fatura entegrasyonları.
3. Gelişmiş Raporlama Servisi.
