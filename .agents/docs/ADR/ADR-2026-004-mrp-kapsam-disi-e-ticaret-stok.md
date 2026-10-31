# ADR-2026-004: MRP Sisteminden Vazgeçilmesi ve E-Ticaret Odaklı Stok Takibine Geçiş

- **Tarih:** 2026-10-13
- **Durum:** ✅ Onaylandı (Gökmen Onayı ile Karara Bağlandı)
- **Karar Vericiler:** Gökmen (Product Owner), Agent Gravity (Orkestratör & Kıdemli PM), Architect, Business Analyst

---

## Bağlam ve Problem Tanımı

Başlangıçta mimari belgelerde projenin "B2B + B2C + E-Commerce + MRP" şeklinde çok geniş ve karmaşık bir endüstriyel üretim/kaynak planlama sistemi olacağı belirtilmişti. 
Ancak yapılan kod ve mimari denetiminde:
1. Mevcut e-ticaret mikroservis ekosisteminin (Catalog, Order, Basket, Location, Rental, B2B) üzerine ağır reçete (BOM), fason iş emri, hammadde temini ve üretim hattı çizelgelemesi gibi endüstriyel MRP yapıları eklemenin Bounded Context sınırlarını ihlal ettiği,
2. Aşırı karmaşıklık, bakım maliyeti ve operasyonel risk yarattığı,
3. Projenin ana odağının modern bir e-ticaret ve kiralama platformu olduğu tespit edilmiştir.

---

## Alınan Karar

1. **MRP Kapsam Dışı Bırakıldı:** Endüstriyel üretim planlama, reçete ve fabrika operasyonları sistem sınırlarından tamamen çıkarılmıştır.
2. **E-Ticaret Stok Giriş/Çıkış Sınırı:** Stok yönetimi tamamen e-ticaret standartlarına çekilmiştir:
   - Depo bazlı ürün stok girişi ve stok çıkışı,
   - Satış anında stok düşümü ve iptal durumunda iade/stok artışı,
   - Kiralama modülünde (`Store.Rental`) belirli tarih aralığı için kiralama rezervasyonu ve çakışma (conflict) kontrolü.
3. **Doküman ve Kod Uyumu:** Tüm dokümantasyon, kurallar ve iş modelleri bu e-ticaret sınırlarına göre revize edilmiştir.

---

## Sonuçlar

### Olumlu:
- Mimari sınırlar netleşti; her mikroservis kendi tekil sorumluluğuna (SRP) odaklandı.
- Geliştirme ve bakım maliyetleri ciddi oranda düştü.
- Karmaşık üretim modelleri yerine hızlı, ölçeklenebilir ve test edilebilir bir e-ticaret akışı sağlandı.

### Olumsuz / Kısıtlar:
- Fabrika düzeyinde hammadde-yarı mamul üretim takibi bu sistem üzerinden yapılamayacaktır. Bu ihtiyaç doğarsa harici bir ERP/MRP sistemine entegrasyon API'si sağlanacaktır.
