# Yapılan İşlemler - 29 Ağustos 2026

## 1. Güvenlik İyileştirmeleri
* **B2B WebApi:** Program.cs içine JWT Bearer (IdentityServer4) Authentication yapısı entegre edildi ve controller'lara [Authorize] attribute'u eklendi.
* **B2B Appsettings:** IdentityServerUrl parametresi config dosyasına eklendi.

## 2. Mimari Bağımlılık ve Kod Temizliği
* **Enum Çakışması Giderildi:** Store.Location.Domain içerisindeki AddressType.cs, InvoiceType.cs, PriceType.cs silindi. Bunlar zaten Store.Shared.Enums içerisinde vardı. Sistemde enum kullanan toplam 25 dosyaya using Store.Shared.Enums; referansı eklendi.
* **Dead Code Temizliği:** 
  * B2B ve Shared klasörlerinde bulunan 4 adet boş Class1.cs silindi.
  * Order.WebApi/Controllers içerisindeki unused DeleteOrderingCommand.cs silindi (Yerine zaten Application içinde RemoveOrderingCommand mevcuttu).
  * Ordering.cs içindeki self-referencing (kendini import etme) hatası ve kullanılmayan kütüphaneler temizlendi.
  * BasketsController içindeki kullanılmayan yorum satırları, kullanılmayan HTTP usings ve unused variable'lar silindi.
  * Store.WebUI/Program.cs içindeki çift yazılmış AddHttpClient() çağrısı kaldırıldı.

## 3. Paket Versiyon Hizalaması (MSB3277 Warning Çözümü)
* Npgsql kütüphane versiyonları 8.0.4'ten **8.0.11**'e çekildi (B2B, Order, Discount projelerinde) böylece tüm sistemde PostgreSQL kütüphanesi Location projesi ile uyumlu hale geldi.
* Location.Tests projesindeki FluentAssertions kütüphanesi güncel sürüme çekildi.

## 4. B2B Mikroservisi İskelet (CQRS & Repository) Oluşturma
* DealerFinancial domain entity'si için CQRS yapısı tamamlandı:
  * Commands: Create, Update, Remove
  * Queries: GetAll, GetById
  * Results: GetAllResult, GetByIdResult
  * Handlers: Tüm operasyonların EF Core Repository üzerinden işlendiği handler sınıfları yazıldı. BaseEntityden gelen InsertDate ve UpdateDate özellikleri kullanıldı.
* IB2BRepository<T> arayüzü ve B2BRepository<T> sınıfı yazılarak generic repository altyapısı kuruldu.
* DealerFinancialsController CRUD endpointleri ile yayına alındı.