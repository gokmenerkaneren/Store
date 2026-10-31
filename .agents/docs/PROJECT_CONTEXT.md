# PROJECT CONTEXT — STORE.SLN
# ═══════════════════════════════════════════════
# Projenin amacı, iş modeli ve kullanıcı tiplerini tanımlar.
# Bu dosya, Antigravity'nin projeyi "tanımasını" sağlayan temel referanstır.
#
# Son Güncelleme: 16 Eylül 2026

---

## 1. PROJENİN AMACI

**Store.sln**, bir organizasyona ait fiziksel ürünlerin (dekoratif ürünler, etkinlik malzemeleri vb.) **tanıtımını, satışını ve kiralamasını** yapan, mikroservis mimarili bir **B2C & B2B E-Ticaret ve Kiralama Platformu**dur.

Proje, hem bireysel müşterilere (B2C) hem de toptan alım yapan bayilere (B2B) hizmet vererek çift taraflı bir iş modeli yürütür.

---

## 2. İŞ MODELLERİ

### 2.1 Satış Modeli (Sale / Purchase)
- Müşteri (B2C veya B2B) ürünü satın alır ve mülkiyeti geçer.
- Sipariş akışı: Sepet → Ödeme → Kargo → Teslim.
- **Fiyat Türleri:**
  - `Cost (1)` — Maliyet fiyatı (sadece dahili görünür)
  - `Retail (2)` — Perakende satış fiyatı (B2C müşteriye görünür)
  - `Wholesale (4)` — Toptan satış fiyatı (B2B bayilere görünür)

### 2.2 Kiralama Modeli (Rental) — ⚠️ HENÜZ KODLANMADI
- Müşteri ürünü belirli bir süre için kiralar; süre sonunda iade eder.
- Stok takvimi, çakışma kontrolü ve kira hesaplama motoru gerektirir.
- **Fiyat Türleri:**
  - `Rental (3)` — Perakende kiralama fiyatı
  - `WholeRental (5)` — Toptan kiralama fiyatı (Bayiler için)
- **Durum:** PriceType enum'ları tanımlı, iş akışı henüz implementasyon aşamasında.

### 2.3 B2B Bayi Modeli
- Bayiler özel indirim oranlarına (`DiscountRate`) sahiptir.
- Açık hesap/veresiye limiti (`CreditLimit`) ve anlık bakiye (`CurrentBalance`) ile çalışırlar.
- Bayi finansal verileri `Store.B2B` mikroservisinde yönetilir (`DealerFinancial` entity'si).

---

## 3. KULLANICI TİPLERİ VE ROL HİYERARŞİSİ

```
SuperAdmin          → Tüm sistem yönetimi, kullanıcı yönetimi, konfigürasyon
  └── Admin         → Ürün, sipariş, bayi, içerik yönetimi
       └── Manager  → Belirli modüllerde operasyonel yetki
            ├── B2BDealer     → Toptan alım, bayi fiyatları görme, sipariş verme
            ├── B2CCustomer   → Perakende alım, kiralama, sipariş takibi
            └── Guest         → Katalog görüntüleme, sepete ekleme (ödeme için kayıt gerekli)
```

---

## 4. TEMEL İŞ SÜREÇLERİ

### 4.1 Ürün Yaşam Döngüsü
1. Ürün kartı oluşturulur (Ad, Kod, Barkod, Kategori, Marka, Birim)
2. Ürün görselleri eklenir (Ana görsel + galeri görselleri)
3. Fiyatlar tanımlanır (Maliyet, Perakende, Kiralama, Toptan)
4. Ürün yayına alınır (`IsPublished = true`)
5. Admin Panel'den stok, fiyat ve içerik güncellemeleri yapılır

### 4.2 Sipariş Akışı
1. Müşteri ürünleri sepete ekler (Store.Basket → Redis)
2. Adres ve ödeme bilgileri girilir
3. Sipariş oluşturulur (Store.Order → PostgreSQL)
4. Sipariş durumu takip edilir
5. Kargo ve teslimat süreçleri yönetilir

### 4.3 Admin Panel İşlevleri
- Ürün CRUD (Ekleme, Düzenleme, Silme, Listeleme)
- Kategori, Marka, Renk, Beden yönetimi
- Slider, Trend, Instagram Akışı gibi vitrin öğelerinin yönetimi
- Sipariş ve müşteri yönetimi
- Bayi yönetimi (B2B)
- Kupon ve indirim yönetimi

---

## 5. ALAN ADI (DOMAIN) TERİMLERİ SÖZLÜĞÜ

| Terim | Anlamı |
|:---|:---|
| **Product** | Satışa veya kiralamaya konu fiziksel ürün |
| **Brand** | Ürünün markası (Örn: Sezadan Dekoratif) |
| **Category** | Ürünün sınıflandırma kategorisi (hiyerarşik, ParentId destekli) |
| **Measurement** | Ürünün ölçü birimi (Adet, Metre, Kg, Takım) |
| **PriceType** | Fiyat türü enum'u (Cost, Retail, Rental, Wholesale, WholeRental) |
| **Dealer** | B2B toptan müşteri (Bayi) |
| **DealerFinancial** | Bayinin finansal verileri (Vergi, İndirim, Kredi Limiti) |
| **Branch** | Şirketin fiziksel şubesi veya deposu |
| **Warehouse** | Ürünlerin depolandığı fiziksel lokasyon |
| **Coupon** | İndirim kuponu (Kod, Oran, Geçerlilik Tarihi) |
| **Ordering** | Sipariş master kartı |
| **OrderDetail** | Sipariş içindeki ürün kalemleri |
| **Slug** | SEO dostu URL yapısı (Örn: alice-arka-fon) |
| **Soft Delete** | Kayıtların fiziksel olarak silinmemesi; IsDeleted = true ile işaretlenmesi |
| **DefaultSlider1** | Ana sayfa slider görselleri |
| **DefaultTrend** | Trend ürünler vitrini |
| **InstagramPost** | Ana sayfa Instagram akışı görselleri |
| **BestSeller** | En çok satan ürünler vitrini |
| **NewProduct** | Yeni ürünler vitrini |

---

> **Not:** Bu doküman projenin iş bağlamını tanımlar. Teknik mimari için `ARCHITECTURE.md`'ye, veritabanı detayları için `DATABASE_MAP.md`'ye bakınız.
