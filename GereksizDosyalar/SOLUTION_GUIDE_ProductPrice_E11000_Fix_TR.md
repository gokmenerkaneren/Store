# ProductPrice E11000 Duplicate Key Error - Çözüm Rehberi

## 🔴 Problem
MongoDB'de `ProductPriceService` yapıcısı çalıştığında aşağıdaki hata oluşuyor:
```
E11000 duplicate key error collection: StoreMD.ProductPrices 
index: ProductId_1_CurrencyId_1_PriceTypeId_1 
dup key: { ProductId: "...", CurrencyId: "00000000-0000-0000-0000-000000000000", PriceTypeId: "..." }
```

**Kök Neden:** MongoDB'de aynı `ProductId + CurrencyId + PriceTypeId` kombinasyonunda ve `ValidTo = null` (aktif fiyat) olan **2 veya daha fazla kayıt** bulunmaktadır.

---

## ✅ Çözüm Adımları

### 1. Veritabanını Temizleme

#### Seçenek A: MongoDB Shell (mongosh) ile
```bash
# MongoDB shell'i açın
mongosh

# Script'i çalıştırın
load("Scripts/MongoDB/CleanupDuplicateProductPrices.js")
```

**Script yapacakları:**
- ✓ Tüm duplicate kombinasyonları bulur
- ✓ Her kombinasyon için en yeni belgeyi saklar
- ✓ Diğer duplicate belgeleri siler
- ✓ İşlem sonrası rapor gösterir

#### Seçenek B: Compass veya Atlas UI ile (Manuel)
1. MongoDB Compass açın
2. `StoreMD.ProductPrices` koleksiyonuna gidin
3. Aşağıdaki aggregation pipeline ile duplicatları bulun:
```javascript
[
  { $match: { ValidTo: null } },
  { $group: {
	  _id: { 
		pid: "$ProductId", 
		cid: "$CurrencyId", 
		ptid: "$PriceTypeId" 
	  },
	  count: { $sum: 1 },
	  ids: { $push: "$_id" }
	}
  },
  { $match: { count: { $gt: 1 } } }
]
```
4. Seçilen her grup için, **en yeni tarihli belge dışında** olanları silin

---

### 2. Kod Güncellemesini Kontrol Etme

Yapılan iyileştirmeler:
- ✓ `ProductPriceService.cs`'de error handling eklendi
- ✓ E11000 hatası yakalmak için try-catch eklendi
- ✓ Hatanın sebebini açıklayan log mesajı eklendi
- ✓ Eski index'i silmeyi deneme mantığı eklendi

**Dosya:** `Services/Catalog/Store.Catalog/Services/ProductPriceServices/ProductPriceService.cs`

---

### 3. Uygulamayı Yeniden Başlatma

Veritabanı temizlemesi tamamlandıktan sonra:
```bash
# Uygulamayı durdurun
# Veritabanı temizliğini çalıştırın
mongosh < Scripts/MongoDB/CleanupDuplicateProductPrices.js

# Uygulamayı başlatın
# Store.Catalog servisini başlatın
```

---

## 🔍 Doğrulama

Temizleme işleminden sonra, aşağıdaki query ile duplicate kalmadığını kontrol edin:

```javascript
// MongoDB Compass veya mongosh'ta çalıştırın
db.ProductPrices.aggregate([
  { $match: { ValidTo: null } },
  { $group: {
	  _id: { pid: "$ProductId", cid: "$CurrencyId", ptid: "$PriceTypeId" },
	  count: { $sum: 1 }
	}
  },
  { $match: { count: { $gt: 1 } } }
])
```

**Beklenen Çıktı:** Boş sonuç (duplicate yok)

---

## 🛡️ Gelecekte Tekrar Oluşmasını Önleme

### Data Validation
ProductPrice oluştururken, mevcut kontroller uygulanıyor:
- ✓ `CreateProductPriceAsync()` içinde overlap kontrol yapılıyor
- ✓ Aynı kombinasyonda ValidTo=null olanların sayısı kontrol ediliyor

### Index Kontrol
- ✓ Uygulama başladığında index otomatik oluşturulmaya çalışılıyor
- ✓ E11000 hatası yakalanıyor ve logllanıyor
- ✓ Eski index silinmeye çalışılıyor

---

## 📝 İlgili Dosyalar

- `Services/Catalog/Store.Catalog/Services/ProductPriceServices/ProductPriceService.cs` - Güncellenmiş servis
- `Services/Catalog/Store.Catalog/Entities/ProductPrice.cs` - Entity modeli
- `Scripts/MongoDB/CleanupDuplicateProductPrices.js` - Temizleme scripti

---

## 📞 Sorun Giderme

| Problem | Çözüm |
|---------|-------|
| Script çalıştırılmıyor | MongoDB connection stringini kontrol edin |
| Hala E11000 hatası alıyorum | Tüm duplicate kombinasyonların silindiğini kontrol edin |
| Index oluşturulmuyor | Temizlemeden sonra uygulamayı yeniden başlatın |
| Sorgu sonuç vermiyor | Sorgu yazımını kontrol edin veya Compass UI kullanın |

