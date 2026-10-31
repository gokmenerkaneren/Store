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
- **Yönetimsel Uçlar:** Tüm Create/Update/Delete ve admin paneli veri işlemleri `[Authorize(Roles = "Admin")]` ile korunur.
- **Katalog & Lokasyon Genel Okuma:** Şehir, ilçe, mahalle ve ürün listeleme GET uçları `[AllowAnonymous]` ile vitrine açıktır.
- **IDOR Koruması:** `UserAddressController`, `FavoritesController` ve `OrderingsController` müşteri kimliğini kesinlikle istemci gövdesinden almaz; JWT `sub` / `ClaimTypes.NameIdentifier` üzerinden çözümler.
