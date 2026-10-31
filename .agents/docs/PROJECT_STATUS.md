# PROJECT STATUS — STORE.SLN PROJE DURUMU
# ═══════════════════════════════════════════════
# Projenin güncel fazı, teknik borçlar, yol haritası ve mimari kararları tanımlar.
# Antigravity, öncelikleri belirlerken ve raporlama yaparken bu dosyaya başvurur.
#
# Son Güncelleme: 23 Eylül 2026

---

## 1. GÜNCEL FAZ

**Aktif Faz:** Faz 2 — İş Mantığı Tamamlama

Faz 1 (Acil Güvenlik) henüz tam tamamlanmadı; güvenlik borçları devam ederken Faz 2 işleri paralel yürütülüyor.

---

## 2. TEKNİK BORÇ TAKİP TABLOSU

| # | Seviye | Konu | Detay | Durum |
|:--|:---:|:---|:---|:---|
| 1 | 🔴 KRİTİK | SSL Bypass | `Store.Catalog` ve `Store.WebUI`'da `ServerCertificateCustomValidationCallback => true` mevcut | ✅ ÇÖZÜLDÜ |
| 2 | 🔴 KRİTİK | Açık Metin Sırlar | `appsettings.json` dosyalarında DB şifreleri plaintext | ✅ ÇÖZÜLDÜ |
| 3 | 🔴 KRİTİK | KVKK — IdentityNumber | `IdentityNumber` (T.C. Kimlik No) AES-256 ile veritabanı seviyesinde şifrelendi (Migration + ValueConverter) | ✅ ÇÖZÜLDÜ |
| 4 | 🔴 KRİTİK | tempkey.jwk | IdentityServer token imzalama anahtarı `.gitignore`'da olmalı | ✅ ÇÖZÜLDÜ |
| 5 | 🟡 YÜKSEK | Rental İş Akışı | `PriceType.Rental(3)` ve `WholeRental(5)` tanımlı ama iş akışı kodlanmadı | ✅ ÇÖZÜLDÜ |
| 6 | 🟡 YÜKSEK | API Gateway Eksikliği | YARP/Ocelot yok; WebUI her servise doğrudan HTTP yapıyor | ❌ AÇIK |
| 7 | 🟡 YÜKSEK | Mesajlaşma Yok | Servisler arası asenkron iletişim (RabbitMQ/MassTransit) yok | ❌ AÇIK |
| 8 | 🔵 ORTA | Birim Test Kapsamı | `Store.Location.Tests` (14 test) ve `Store.Order.Tests` (2 test) olmak üzere 16 test aktif | ✅ İYİLEŞTİRİLDİ |
| 9 | 🔵 ORTA | Discount Coupon İndeksi | `Code` alanı `NVARCHAR(MAX)`, `IsActive` üzerinde index yok | ✅ ÇÖZÜLDÜ |
| 10 | 🔵 ORTA | Order Namespace Hatası | `Entitties` (çift t) namespace/klasör adı | ✅ ÇÖZÜLDÜ |
| 11 | ✅ ÇÖZÜLDÜ | Catalog → Location Bağımlılığı | `Store.Shared.Enums` kullanımıyla çözüldü (15.09.2026) | ✅ KAPALI |
| 12 | ✅ ÇÖZÜLDÜ | Dealer B2B Ayrımı | `DealerFinancial` entity'si `Store.B2B`'ye taşındı | ✅ KAPALI |
| 13 | ✅ ÇÖZÜLDÜ | InstagramPost BsonSerializationException | Duplicate `IsActive` property kaldırıldı | ✅ KAPALI |
| 14 | ✅ ÇÖZÜLDÜ | InstagramPost AutoMapper | `GeneralMapping.cs`'ye mapping profilleri eklendi | ✅ KAPALI |
| 15 | 🔴 KRİTİK | UsersController Güvenliği | `UsersController`'dan `[AllowAnonymous]` kısımlarının kaldırılıp doğru `[Authorize]` politikasının yazılması | ✅ ÇÖZÜLDÜ |
| 16 | 🟡 YÜKSEK | UI Favori Ekleme Hatası | `/Default/Index` sayfasında ürün üzerindeki kalp butonu ile favoriye ekleme/çıkarma işlemi (favorite.js) düzgün çalışmıyor, askıya alındı. | ❌ AÇIK |
| 17 | 🟡 YÜKSEK | Soft Delete Bütünlüğü (RV-005) | Discount, B2B, Order, Location servislerinde fiziksel silme yerine IsDeleted soft delete sağlandı | ✅ ÇÖZÜLDÜ |
| 18 | 🟡 YÜKSEK | Mongo Bellek Taşması (RV-006) | Catalog servislerinde tüm koleksiyonu RAM'e çeken Find(_=>true) DB regex filtrelerine dönüştürüldü | ✅ ÇÖZÜLDÜ |
| 19 | 🔵 ORTA | Redis TTL Sızıntısı (RV-009) | BasketService sepet kayıtlarına 30 günlük TTL eklendi | ✅ ÇÖZÜLDÜ |
| 20 | 🔵 ORTA | Order Sync-over-Async & DI (RV-011) | Order WebApi .Result kaldırıldı; Location WebApi 130 satırlık DI şişkinliği dinamik reflection kaydına geçirildi | ✅ ÇÖZÜLDÜ |
| 21 | 🔵 ORTA | WebUI Hesabım N+1 (RV-007) | 12 sıralı HTTP çağrısı Task.WhenAll ile paralel çekmeye dönüştürüldü, ILogger entegre edildi | ✅ ÇÖZÜLDÜ |

---

## 3. SON TAMAMLANAN İŞLER (13.10.2026)

| İş | Tarih | Detay |
|:---|:---|:---|
| KVKK AES-256 Şifreleme (RV-004) | 13.10.2026 | `AesEncryptionHelper` (Store.Shared), `EncryptedStringConverter` (EF ValueConverter), Migration ve ModelSnapshot uygulandı; xUnit testleri yazıldı. |
| Soft Delete Bütünlüğü (RV-005) | 13.10.2026 | Discount, B2B, Order, Location servislerindeki fiziksel DELETE operasyonları soft-delete yapısına geçirildi; global query filtreleri eklendi. |
| Catalog Mongo OOM Engelleme (RV-006) | 13.10.2026 | Product, Category, Currency, Measurement, PriceType servislerindeki `Find(_ => true)` RAM yüklemeleri DB-seviyesi case-insensitive regex aramaya dönüştürüldü. |
| Redis Basket TTL (RV-009) | 13.10.2026 | Sepet verilerine 30 günlük otomatik TTL eklendi; bellek birikmesi önlendi. |
| Order Kırık Test & Çözüm Entegrasyonu (RV-010) | 13.10.2026 | `Store.Order.Tests` net10.0'dan net8.0'a çekilerek Store.slnx'e dahil edildi; testler yeşile döndürüldü. |
| DI & Sync-Over-Async Temizliği (RV-011) | 13.10.2026 | Order WebApi Program.cs `.Result` deadlock riski temizlendi; Location WebApi'daki 130 satırlık elle DI kaydı otomatik CQRS assembly reflection kaydına dönüştürüldü. |
| WebUI Hesabım N+1 İyileştirmesi (RV-007) | 13.10.2026 | HesabimController içerisindeki 12 sıralı senkron HTTP isteği `Task.WhenAll` ile paralel hale getirildi; loglama eklendi. |
| Catalog Write Yetkilendirme (RV-001) | 13.10.2026 | Catalog controller'larındaki açık uçlar kapatıldı; yazma işlemleri [Authorize], okuma işlemleri [AllowAnonymous] yapıldı. |
| Rental WebApi Runtime Fix (RV-003) | 13.10.2026 | Store.Rental.WebApi Program.cs (DbContext, MediatR, Controllers, JWT Auth) ayağa kaldırıldı; WebUI RentalBaseUrl bağlandı. |
| DB Sır & Parola Temizliği (RV-002) | 13.10.2026 | LocationContext.cs hardcoded connection string temizlendi, PassWords.txt temizlenip .gitignore'a eklendi. |
| Favorite IDOR Koruması (RV-008) | 13.10.2026 | FavoritesController üzerinde token ClaimTypes.NameIdentifier / sub doğrulaması eklendi; IDOR engellendi. |
| WebUI BearerTokenHandler | 13.10.2026 | WebUI Refit istemcilerine DelegatingHandler eklenerek JWT Bearer token otomatik iletimi sağlandı. |
| MRP Kapsam Kararı (ADR-004) | 13.10.2026 | Ağır MRP mimari kapsamdan çıkarıldı; e-ticaret odaklı stok takibi kabul edildi. |
| Rental (Kiralama) Modülü | 02.10.2026 | Store.Rental mikroservisi, CQRS, Conflict Check iş mantığı, WebUI Arayüzü eklendi |
| Güvenlik & KVKK Yamaları | 02.10.2026 | appsettings.json plaintext şifre temizliği, AES-256 TCKimlik şifreleme, UsersController yetkilendirme (Authorize) kapatıldı |

---

## 4. STRATEJİK YOL HARİTASI

### 🔴 Faz 1 — Acil Güvenlik & Teknik Borç Temizliği
1. ✅ SSL bypass kodlarının `#if DEBUG` ile sınırlandırılması
2. ✅ `appsettings.json` bağlantı cümleciklerinin `dotnet user-secrets`'e taşınması (Hazırlığı)
3. ✅ `IdentityNumber` alanının veritabanı seviyesinde şifrelenmesi (AES-256 CBC ValueConverter + Migration)
4. ✅ `tempkey.jwk` dosyasının `.gitignore`'a eklenmesi
5. ✅ `UsersController` içerisindeki `[AllowAnonymous]` zafiyetinin kapatılıp yetkilendirmenin sağlanması

### 🟡 Faz 2 — İş Mantığı Tamamlama
1. ✅ **Rental (Kiralama) İş Akışı** — Stok takvimi, çakışma kontrolü, kira hesaplama, mikroservis
2. ✅ **Instagram Akışı** — Admin Panel'den Instagram gönderi yönetimi (16.09.2026)
3. ❌ **B2B Admin Ekranları** — Bayi Finans Yönetimi WebUI ekranları
4. ❌ **Server-Side DataTables** — Tüm yüksek hacimli ekranlara yayılması
5. ✅ **Favorite Modülü** — Standalone mikroservis olarak kurulması (28.09.2026)
6. ✅ **IdentityServer Login Entegrasyonu** — WebUI'da `UyelikController.Login` içerisinden token alınarak oturum açılması (TAMAMLANDI)

### 🔵 Faz 3 — Altyapı & DevOps Olgunlaşması
1. ❌ YARP / Ocelot API Gateway
2. ❌ RabbitMQ / MassTransit asenkron mesajlaşma
3. ❌ Redis Cache (sık erişilen katalog verileri)
4. ❌ Docker & Container (tüm servislerin container'da çalışması)
5. ❌ Test kapsamı artırma (xUnit, Testcontainers)

---

## 5. MİMARİ KARAR KAYITLARI (ADR — Architecture Decision Records)

### ADR-001: Favorite Servisi Standalone Mikroservis Olacak (16.09.2026)
- **Karar:** Catalog'a gömmek yerine bağımsız `Store.Favorite` mikroservisi oluşturulacak
- **Neden:** Bounded Context ayrımı; farklı okuma/yazma yükü; bağımsız ölçeklenebilirlik
- **Alternatif:** Catalog MongoDB'sine koleksiyon olarak eklemek (reddedildi)
- **Durum:** ✅ Onaylandı, 🔧 Geliştirmede

### ADR-002: Instagram Medya Çekme — Sadece Kapak Resmi (16.09.2026)
- **Karar:** Instagram gönderisinden yalnızca `og:image` (kapak resmi) çekilecek
- **Neden:** Instagram API erişimi gerektirmeden çalışır; karousel/video desteği karmaşıklık yaratır
- **Alternatif:** Tüm medyaları çekmek (reddedildi — Instagram bot koruması)
- **Durum:** ✅ Onaylandı, ✅ Tamamlandı

### ADR-003: BaseEntity'den Kalıtım Zorunluluğu (15.09.2026)
- **Karar:** Tüm entity'ler `Store.Shared.Entities.BaseEntity`'den türeyecek
- **Neden:** Audit trail, soft-delete ve tutarlı ID yönetimi
- **Alternatif:** Her servis kendi BaseEntity'sini tanımlasın (reddedildi — kod tekrarı)
- **Durum:** ✅ Onaylandı, ✅ Uygulandı

### ADR-004: MRP Kapsam Dışı Bırakıldı — E-Ticaret Odaklı Stok Takibi (13.10.2026)
- **Karar:** Ağır endüstriyel MRP (Üretim Planlama) sistemi mimari kapsamdan çıkarıldı; stok yönetimi tamamen e-ticaret odaklı giriş/çıkış ve rezervasyon mantığıyla sınırlandırıldı.
- **Neden:** Bounded Context sınırlarının korunması, aşırı mimari ve operasyonel karmaşıklıktan kaçınılması.
- **Alternatif:** Ayrı bir MRP mikroservisi kurmak (Gökmen ve Architect konsensüsüyle iptal edildi).
- **Durum:** ✅ Onaylandı, Uygulandı

---

> **Güncelleme Kuralı:** Bir faz tamamlandığında, teknik borç kapandığında veya mimari karar alındığında bu doküman güncellenir.
