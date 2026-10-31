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
| `OrderCreatedIntegrationEvent` | `Store.Order` | `Store.Catalog` | OrderId, UserId, TotalPrice, OrderDate, Items[] | Siparişin kaydedildiğini bildirir, stok düşümünü tetikler |
| `StockReservedIntegrationEvent` | `Store.Catalog` | `Store.Order` | OrderId, TotalPrice, ReservedDate, Items[] | Stok başarıyla rezerve edildi, sipariş durumunu `Processing` yapar |
| `StockReservationFailedIntegrationEvent` | `Store.Catalog` | `Store.Order` | OrderId, Reason, FailedDate, Items[] | Yetersiz stok; siparişi `Cancelled` yapar ve telafi notu düşer |

## 4. GÜVENLİK VE ROL STANDARTLARI
- **Yönetimsel Uçlar:** Tüm Create/Update/Delete ve admin paneli veri işlemleri `[Authorize(Roles = "Admin")]` ile korunur (Catalog, Location, Discount).
- **Katalog & Lokasyon & Kupon Doğrulama Genel Okuma:** Şehir, ilçe, mahalle ve ürün listeleme GET uçları ile `GET /api/Discounts/GetByCode/{code}` vitrin uçları `[AllowAnonymous]` ile açıktır.
- **IDOR Koruması:** `UserAddressController`, `FavoritesController` ve `OrderingsController` müşteri kimliğini kesinlikle istemci gövdesinden almaz; JWT `sub` / `ClaimTypes.NameIdentifier` üzerinden çözümler.

## 5. DISCOUNT SERVİS SÖZLEŞMELERİ
- `GET /api/Discounts/GetByCode/{code}` (AllowAnonymous):
  - 200 OK: `{ id, code, rate, isActive, validDate }`
  - 400 Bad Request: `{ message: "Kuponun geçerlilik süresi dolmuş." }`
  - 404 Not Found: `{ message: "Kupon bulunamadı." }`
- `POST /api/Discounts`, `PUT /api/Discounts`, `DELETE /api/Discounts/{id}` (`[Authorize(Roles = "Admin")]`).
- `IDiscountApiService` (WebUI Refit İstemcisi): `GetDiscountCouponByCodeAsync(string code)` ile güvenli çağrı.
