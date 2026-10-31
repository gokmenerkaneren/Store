# API CONTRACTS — SERVİS İLETİŞİM SÖZLEŞMELERİ
# ══════════════════════════════════════════════════
# Servisler arası HTTP istek/yanıt şemalarını ve temel endpoint sözleşmelerini tanımlar.

---

## 1. GENEL RESPONSE WRAPPER
Mikroservislerimizden dönen tüm HTTP 200 (ve hata) yanıtları standart bir Response objesi ile sarılmalıdır:

```json
{
  "data": { ... },       // İşlem sonucu verisi (başarılıysa)
  "errors": [ ... ],     // Hata mesajları listesi (başarısızsa)
  "isSuccessful": true,  // İşlem durumu
  "statusCode": 200      // HTTP statü kodu
}
```

## 2. TEMEL ENDPOINT STANDARTLARI (REST)
Tüm servisler için varsayılan CRUD rotaları şu formatı izlemelidir:

- `GET /api/[EntityPlural]` -> Liste döner
- `GET /api/[EntityPlural]/{id}` -> Tekil kayıt döner
- `POST /api/[EntityPlural]` -> Yeni kayıt oluşturur
- `PUT /api/[EntityPlural]` -> Kayıt günceller
- `DELETE /api/[EntityPlural]/{id}` -> Kaydı (soft delete) siler

> **Güncelleme Kuralı:** Servislere özgü kritik yeni endpoint'ler eklendikçe burada belgelendirilmelidir.


---

## 3. MASSTRANSIT ENTEGRASYON OLAYLARI (EVENTS)

| Olay Adı | Yayınlayan | Dinleyen | Alanlar | Amacı |
|:---|:---|:---|:---|:---|
| `OrderCreatedIntegrationEvent` | `Store.Order` | `Store.Catalog`, `Store.B2B` | OrderId, UserId, TotalPrice, OrderDate, Items[] | Siparişin kaydedildiğini bildirir; stok düşümünü ve bayi bakiye artırımını tetikler |
| `StockReservedIntegrationEvent` | `Store.Catalog` | `Store.Order`, `Store.Payment` | OrderId, UserId, TotalPrice, ReservedDate, Items[] | Stok başarıyla rezerve edildi; ödeme sürecini tetikler |
| `StockReservationFailedIntegrationEvent` | `Store.Catalog` | `Store.Order`, `Store.B2B` | OrderId, UserId, TotalPrice, Reason, FailedDate, Items[] | Yetersiz stok; siparişi iptal eder ve bayi limitini iade eder |
| `PaymentCompletedIntegrationEvent` | `Store.Payment` | `Store.Order` | OrderId, UserId, TotalPrice, CompletedDate | Ödeme başarılı; sipariş durumunu `Processing` mühürler |
| `PaymentFailedIntegrationEvent` | `Store.Payment` | `Store.Order`, `Store.Catalog`, `Store.B2B` | OrderId, UserId, TotalPrice, Reason, FailedDate, Items[] | Ödeme başarısız; siparişi iptal eder, stokları iade eder, bayi limitini iade eder |

## 4. GÜVENLİK VE ROL STANDARTLARI
- **Yönetimsel Uçlar:** Tüm Create/Update/Delete ve admin paneli veri işlemleri `[Authorize(Roles = "Admin")]` ile korunur (Catalog, Location, Discount, B2B).
- **Katalog & Lokasyon & Kupon Doğrulama Genel Okuma:** Şehir, ilçe, mahalle ve ürün listeleme GET uçları ile `GET /api/Discounts/GetByCode/{code}`, `GET /api/DealerFinancials/GetByDealerId/{dealerId}` vitrin uçları `[AllowAnonymous]` ile açıktır.
- **IDOR Koruması:** `UserAddressController`, `FavoritesController` ve `OrderingsController` müşteri kimliğini kesinlikle istemci gövdesinden almaz; JWT `sub` / `ClaimTypes.NameIdentifier` üzerinden çözümler.

## 5. DISCOUNT SERVİS SÖZLEŞMELERİ
- `GET /api/Discounts/GetByCode/{code}` (AllowAnonymous):
  - 200 OK: `{ id, code, rate, isActive, validDate }`
  - 400 Bad Request: `{ message: "Kuponun geçerlilik süresi dolmuş." }`
  - 404 Not Found: `{ message: "Kupon bulunamadı." }`
- `POST /api/Discounts`, `PUT /api/Discounts`, `DELETE /api/Discounts/{id}` (`[Authorize(Roles = "Admin")]`).
- `IDiscountApiService` (WebUI Refit İstemcisi): `GetDiscountCouponByCodeAsync(string code)` ile güvenli çağrı.

## 6. B2B SERVİS SÖZLEŞMELERİ
- `GET /api/DealerFinancials/GetByDealerId/{dealerId}`:
  - 200 OK: `{ id, dealerId, taxNumber, taxOffice, discountRate, creditLimit, currentBalance }`
  - 404 Not Found
- `IB2BApiService` (WebUI Refit İstemcisi): `GetDealerFinancialByDealerIdAsync(Guid dealerId)` ile bayi finansal bilgilerini ve limitlerini sorgular.
- `OrderCreatedB2BConsumer`: `OrderCreatedIntegrationEvent`'i dinleyerek sipariş tutarını bayinin cari borç bakiyesine ekler.
- `PaymentFailedB2BConsumer` & `StockReservationFailedB2BConsumer`: Hata durumlarında bayinin cari borç bakiyesini düşerek açık hesap limitini iade eder (Rollback).

## 7. PAYMENT SERVİS SÖZLEŞMELERİ
- Port: `7309` (`http://localhost:7309`)
- `POST /api/Payments/Process`:
  - Request: `{ orderId, userId, amount, cardNumber }`
  - Response: `{ isSuccess, transactionId, errorMessage }`
- `StockReservedPaymentConsumer`: `StockReservedIntegrationEvent`'i dinleyerek mock ödeme yürütür ve sonuca göre `PaymentCompletedIntegrationEvent` veya `PaymentFailedIntegrationEvent` yayınlar.

## 8. API GATEWAY SÖZLEŞMELERİ (YARP)
- Port: `7000` (`http://localhost:7000`)
- **Tersine Dizin (Reverse Proxy) Standartları:**
  - `Store.WebUI` ve dış istemciler tüm mikroservis çağrılarını `http://localhost:7000` üzerinden yapar.
  - Varlık bazlı doğrudan yollar (`/api/Products`, `/api/Orderings`, `/api/DealerFinancials` vb.) ilgili mikroservis cluster'ına iletilir.
  - Servis bazlı prefix yolları (`/api/catalog/{**catch-all}`, `/api/order/{**catch-all}` vb.) `PathRemovePrefix` ve `PathPrefix` transformları ile ilgili cluster'a yönlendirilir.
  - Sağlık ve durum uç noktası: `GET /health` -> `{ status: "Healthy", timestamp: ... }`.
