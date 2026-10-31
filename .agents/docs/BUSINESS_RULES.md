# BUSINESS RULES — STORE.SLN İŞ KURALLARI VE KISITLAMALAR
# ═══════════════════════════════════════════════════════════
# Bu dosya, projede kodlanmış veya kodlanması gereken iş kurallarını,
# kısıtlamaları ve doğrulama gereksinimlerini tanımlar.
# Antigravity, validasyon veya iş akışı kodlarken bu dosyaya başvurur.
#
# Son Güncelleme: 16 Eylül 2026

---

## 1. ÜRÜN KURALLARI

### 1.1 Ürün Kartı Zorunlulukları
- Her ürünün bir `Name`, `ProductCode`, `CategoryId` ve `MeasurementId` alanı zorunludur
- `ProductCode` benzersiz (unique) olmalıdır — aynı kodla iki ürün oluşturulamaz
- `Barcode` benzersiz olmalıdır (varsa)
- `Name` benzersiz olmalıdır — ✅ Backend'de kontrol edilmektedir (duplicate check)
- `Slug` alanı SEO uyumlu, Türkçe karakter destekli ve benzersiz olmalıdır

### 1.2 Ürün Durumları
- `IsActive = false` → Ürün listelerden ve aramalardan gizlenir
- `IsPublished = false` → Ürün taslak olarak kalır, yayınlanmaz
- `IsDeleted = true` → Ürün soft-delete ile silinmiş; tüm sorgularda filtrelenir

### 1.3 Ürün Silme Kuralları
- Ürünler asla fiziksel olarak silinmez (Soft Delete)
- Silme işlemi: `IsDeleted = true`, `DeleteUserId` ve `DeleteDate` güncellenir
- Silinen ürünler geri alınabilir (Restore)

---

## 2. FİYATLANDIRMA KURALLARI

### 2.1 Fiyat Türleri (PriceType Enum)

| Enum Değeri | Kod | Açıklama | Kim Görür |
|:---|:---|:---|:---|
| `Cost` | 1 | Maliyet fiyatı | Yalnızca Admin |
| `Retail` | 2 | Perakende satış fiyatı | B2C Müşteri |
| `Rental` | 3 | Perakende kiralama fiyatı | B2C Müşteri |
| `Wholesale` | 4 | Toptan satış fiyatı | B2B Bayi |
| `WholeRental` | 5 | Toptan kiralama fiyatı | B2B Bayi |

### 2.2 Fiyat Geçerlilik Kuralları
- `StartDate` ve `EndDate` ile dönemsel fiyat tanımlanabilir
- `EndDate = null` → Fiyat süresiz geçerlidir
- Aynı ürün için aynı `PriceType` ve aynı `CurrencyId` ile çakışan tarih aralığında birden fazla fiyat tanımlanmamalıdır
- `DiscountedAmount` değeri `Amount` değerinden büyük olamaz
- `MinimumQuantity` ile miktar bazlı fiyatlandırma desteklenir (Varsayılan: 1)

### 2.3 KDV Kuralları
- `TaxRate` ürün bazında tanımlanır (Örn: %20, %10, %1)
- `IsTaxIncluded` fiyatın KDV dahil mi yoksa hariç mi olduğunu belirtir
- Sepet ve fatura hesaplamalarında bu iki alan birlikte değerlendirilir

---

## 3. BAYİ (B2B) KURALLARI

### 3.1 Bayi Kayıt
- Her bayinin `TaxNumber` alanı zorunlu ve benzersiz (UNIQUE INDEX) olmalıdır
- `DealerFinancial` entity'si `Store.B2B` mikroservisinde yönetilir (Bounded Context ayrımı)

### 3.2 Bayi Finansal
- `DiscountRate` → Bayiye otomatik uygulanan iskonto oranı (0.00 - 1.00 arası)
- `CreditLimit` → Açık hesap çalışma limiti
- `CurrentBalance` → Anlık borç/alacak bakiyesi
- Bayi, `CreditLimit`'i aşan sipariş veremez (⚠️ Henüz kodlanmadı)

---

## 4. SİPARİŞ KURALLARI

### 4.1 Sipariş Oluşturma
- Sipariş adresi, sipariş anında "dondurularak" kaydedilir (adres sonradan değişse bile siparişteki kayıt değişmez)
- `TotalPrice` = Σ(ürün birim fiyatı × adet) + KDV - İndirim
- Her sipariş satırı (`OrderDetail`) sipariş anındaki birim fiyatı kaydeder

### 4.2 Kupon Kullanımı
- Kupon kodu (`Code`) benzersiz olmalıdır
- `IsActive = false` olan kupon kullanılamaz
- `ValidDate` geçmiş olan kupon geçersizdir
- `Rate` → Yüzdesel indirim oranı (Örn: 10 = %10 indirim)

---

## 5. ADRES VE COĞRAFİ VERİ KURALLARI

### 5.1 Adres Hiyerarşisi
```
Ülke (Country)
  └── Şehir (City)
       └── İlçe (District)
            └── Mahalle (Neighborhood)
```

### 5.2 Adres Zorunlulukları
- Tüm adres tipleri `Title`, `ContactFullName`, `PhoneNumber`, `AddressDetail` alanlarını zorunlu tutar
- `CityId`, `DistrictId`, `NeighborhoodId` cascade seçimi ile doldurulur

### 5.3 KVKK Kuralları
- `IdentityNumber` (T.C. Kimlik No) veritabanında **AES-256 CBC ile şifrelenerek** saklanır (✅ Tamamlandı — ValueConverter & Migration).
- `IsAnonymized` flag'i ile KVKK gereği adres anonimleştirilebilir
- Kişisel verilerin (Phone, Email, AddressDetail) loglanması, açık metin olarak yapılmamalıdır

---

## 6. KİRALAMA KURALLARI (`Store.Rental` Mikroservisi)

### 6.1 Uygulanan Kurallar
- Bir ürün aynı anda birden fazla müşteriye kiralanamaz (stok takvimi çakışma kontrolü / Conflict Check).
- Kiralama süresi başlangıç (`StartDate`) ve bitiş (`EndDate`) tarihi ile tanımlanır.
- Kiralama talebi geldiğinde mevcut aktif rezervasyonlar sorgulanır; çakışma varsa işlem reddedilir.
- Rezervasyon başarılı olduğunda `RentalBooking` kaydı oluşturulur ve toplam kira bedeli hesaplanır.
- MRP yerine e-ticaret rezervasyon takibi sınırları korunur (ADR-004).

### 6.2 Sepet Bütünlüğü ve Ayrımı Kuralları (Satış vs Kiralama)
- **Tür Ayrımı Kuralı:** Bir sepette ya yalnızca Satış (`BasketType.Sale`) ya da yalnızca Kiralama (`BasketType.Rental`) ürünleri bulunabilir; ikisi aynı anda aynı sepette birleştirilemez.
- **Çakışma (Conflict) Yönetimi:** Sepette satış ürünü varken kiralama ürünü eklenmek istendiğinde (veya tam tersi) sistem HTTP 409 döner; kullanıcıdan sepeti boşaltma onayı alınarak yeni tipte sepet başlatılır.
- **Sepet İçi Adet Güncelleme:** Sağ sepet çekmecesinden ürün adedi artırılabilir (+), eksiltilebilir (-). Adet 0'a indiğinde ürün sepetten otomatik olarak silinir.
- **Sepetten Silme:** Kullanıcı dilerse "Kaldır" butonuyla ürünü doğrudan sepetten çıkartabilir; sepet boşaldığında varsayılan sepet tipine dönülür.

---

## 7. STOK YÖNETİMİ VE SINIRLARI (ADR-004)

### 7.1 E-Ticaret Stok Takibi İlkeleri
- Sistem ağır endüstriyel MRP (hammadde, reçete, fason üretim vb.) süreçlerini kapsamaz.
- Depo bazlı ürün stok giriş ve çıkışları takip edilir (`Store.Location` Warehouses & `Store.Catalog` Ürün varyant stokları).
- Sipariş onaylandığında ürün stok adedi düşürülür; sipariş iptalinde iade edilir.
- Kiralama işlemlerinde ise ürün mülkiyeti transfer edilmez; yalnızca belirlenen tarih aralığı rezerve edilir.

---

## 7. VİTRİN VE İÇERİK KURALLARI

### 7.1 Slider (DefaultSlider1)
- Her slider'ın bir `OrderIndex` (sıra numarası) vardır
- `IsActive = false` olan slider'lar ön yüzde gösterilmez

### 7.2 Instagram Akışı (InstagramPost)
- Instagram gönderi linki girildiğinde, kapak resmi (og:image) otomatik olarak çekilir
- Resimler `wwwroot/Images/Instagram/` dizinine kaydedilir
- `OrderIndex` yeni eklemede otomatik olarak max + 1 hesaplanır

### 7.3 Trend Ürünler, Çok Satanlar, Yeni Ürünler
- Bu bölümler `DefaultTrend`, `BestSeller`, `NewProduct` entity'leri ile yönetilir
- Her birinin `OrderIndex` ve `IsActive` alanları vardır

---

## 8. STOK VE LOJİSTİK KURALLARI (TASARIM AŞAMASINDA)

### 8.1 Stok Takibi (E-Ticaret Odaklı — MRP Kapsam Dışı)
- **MRP Kararı (ADR-004):** Karmaşık endüstriyel MRP (Üretim Planlama) sistemi kesin olarak kapsam dışı bırakılmıştır. Sistem tamamen **e-ticaret odaklı ürün stok takibi (giriş/çıkış ve rezervasyon)** şeklinde çalışır.
- Sipariş oluşturulduğunda stok düşülecek, iptal/iade durumunda stok geri eklenecektir.
- Kiralama modelinde stok, takvim aralığı bazında kiralama rezervasyonu olarak bloke edilir.
- Gelecekte kargo ve lojistik durumları için sipariş statüleri (Hazırlanıyor, Kargoya Verildi, Teslim Edildi vb.) basit bir state machine ile takip edilecektir.

---

## 9. GENEL SİSTEM KURALLARI

| Kural | Detay |
|:---|:---|
| **Soft Delete** | Tüm entity'lerde `IsDeleted` flag'i kullanılır; fiziksel silme yapılmaz |
| **Audit Trail** | Her kayıt `InsertDate`, `InsertUserId`, `UpdateDate`, `UpdateUserId`, `DeleteDate`, `DeleteUserId` alanlarını tutar |
| **Tarih Formatı** | `DateTimeOffset` ile UTC-safe tarih yönetimi; `GetLocalTime()` helper metodu kullanılır |
| **Kimlik (ID)** | Catalog ve Location: `Guid`; Order ve Discount: `int` (Auto-Increment) |

---

> **Güncelleme Kuralı:** Yeni bir iş kuralı keşfedildiğinde, mevcut bir kural değiştiğinde veya planlanan bir kural implemente edildiğinde bu doküman güncellenir.

### 4.3 Sipariş Durum Makinesi (Order State Machine)
- `Ordering` durumu `OrderStatus` enum'u ile yönetilir ve domain seviyesinde `CanTransitionTo` kuralına tabidir:
  - `Pending` (Beklemede) → Yalnızca `Processing` veya `Cancelled` olabilir.
  - `Processing` (Hazırlanıyor) → `Shipped` veya `Cancelled` olabilir.
  - `Shipped` (Kargoda) → `Delivered` veya `Returned` olabilir.
  - `Delivered` (Teslim Edildi) → `Completed` veya `Returned` olabilir.
  - `Cancelled` (İptal Edildi) & `Completed` (Tamamlandı) → Terminal durumdur, değiştirilemez.
- Geçersiz bir geçiş talebinde `InvalidOperationException` fırlatılır.
- Kargolanmış veya işleme alınmış siparişler müşteri tarafından doğrudan silinemez (`RemoveOrdering` engeli).

### 4.4 Dağıtık Stok Rezervasyonu ve Kompansasyon (Choreography)
- Sipariş başarıyla veritabanına yazıldığında `OrderCreatedIntegrationEvent` fırlatılır.
- Catalog servisi `OrderCreatedConsumer` üzerinden sipariş kalemlerini inceler:
  - Varyant stoğu sipariş miktarını karşılıyorsa stok düşülür ve `StockReservedIntegrationEvent` fırlatılır (Sipariş statüsü `Processing` yapılır).
  - Herhangi bir kalemde yetersiz stok varsa veya iyimser eşzamanlılık çakışması olursa daha önce düşülen kalemler `$inc` ile iade edilir ve `StockReservationFailedIntegrationEvent` fırlatılır (Sipariş statüsü `Cancelled` yapılır ve iptal sebebi kaydedilir).

### 4.5 Kiralama Çakışma ve GiST EXCLUDE Koruması
- `Store.Rental` mikroservisinde, aynı ürünün aynı tarih aralığında mükerrer kiralanmasını önlemek için PostgreSQL motoru seviyesinde GiST `EXCLUDE` kısıtı (`btree_gist`) devrededir. Uygulama seviyesindeki race conditions bu kısıt ile %100 engellenir.

### 4.6 Sunucu Taraflı Kupon ve Fiyat Doğrulama (Server-Side Checkout)
- İstemciden gelen `DiscountAmount` veya indirimli `TotalPrice` değerine asla güvenilmez.
- Müşteri yalnızca indirim kupon kodunu (`DiscountCode`) gönderir.
- Checkout esnasında kupon kodu `Store.Discount` servisinden Dapper tabanlı sorgu ile doğrulanır:
  - Kupon bulunamazsa veya pasifse (`IsActive == false`), sipariş engellenir (`CouponValidationResult.NotFoundOrInactive`).
  - Kuponun son geçerlilik tarihi dolmuşsa (`ValidDate < DateTime.UtcNow`), sipariş engellenir (`CouponValidationResult.Expired`).
  - Kupon geçerliyse: indirim oranı sepet ara toplamına (`SubTotal`) uygulanır (`CouponPricing.CalculateDiscount`), indirim tutarı 2 basamağa yuvarlanır (`Math.Round(subTotal * (coupon.Rate / 100.0m), 2)`), nihai `TotalPrice` sunucuda `Math.Max(0, subTotal - discountAmount)` formülüyle hesaplanır ve Order servisine güvenli olarak gönderilir.
  - İndirim servisine ulaşılamazsa sipariş askıya alınır ve kullanıcıya bilgilendirme uyarısı gösterilir (`CartWarning`). Kupon doğrulanamadan indirimli sipariş oluşturulmasına izin verilmez.
