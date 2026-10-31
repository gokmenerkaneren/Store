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
