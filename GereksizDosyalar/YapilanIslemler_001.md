# Yapılan İşlemler - 001

CurrencyController ile yeni kayıt girişi yaparken yaşanan ve hata detayının görünmemesine yol açan sorunlar giderilmiştir. Yapılan değişiklikler ve düzeltmeler aşağıda listelenmiştir:

## 1. AutoMapper Eşleştirmelerinin Eklenmesi (Mapping)
* **Dosya:** [GeneralMapping.cs](file:///c:/User/Projects/Store/Services/Catalog/Store.Catalog/Mapping/GeneralMapping.cs)
* **Açıklama:** `Currency` Entity ile `ResultCurrencyDto`, `CreateCurrencyDto`, `UpdateCurrencyDto` ve `GetByIdCurrencyDto` sınıfları arasındaki AutoMapper eşleştirmeleri (CreateMap) eksikti. Bu durum, veri tabanına kayıt ekleme veya güncelleme esnasında `AutoMapperMappingException` fırlatılmasına sebep oluyordu. Bu eşleştirmeler ve gerekli `using Store.Catalog.Dtos.CurrencyDtos;` import ifadesi dosyaya eklendi.

## 2. API Controller Rota ve İsim Uyuşmazlığının Giderilmesi
* **Dosya:** [CurrenciesController.cs](file:///c:/User/Projects/Store/Services/Catalog/Store.Catalog/Controllers/CurrenciesController.cs)
* **Açıklama:** WebUI tarafındaki `CurrencyController` API isteklerini `http://localhost:7301/api/Currencies` adresine yaparken, API tarafındaki controller sınıfı yanlışlıkla `CurrencysController` olarak tanımlanmıştı ve `[Route("api/[controller]")]` özniteliği nedeniyle URL `api/Currencys` oluyordu. Sınıf ismi ve yapıcı metot (constructor) ismi `CurrenciesController` olarak düzeltildi.

## 3. Derleme Testi
* Proje `dotnet build "c:\User\Projects\Store\Store.slnx"` komutu ile başarıyla derlenmiş, 0 hata ile süreç tamamlanmıştır.
