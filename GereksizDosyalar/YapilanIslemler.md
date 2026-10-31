# Yapılan İşlemler (Para Birimi ve Ölçü Birimi Entegrasyonu)

Aşağıdaki işlemler, "Para Birimleri" (Currency) ve "Ölçü İşlemleri" (Measurement) modülleri için `Admin` Area içerisinde başarıyla gerçekleştirilmiştir.

## 1. Veri Transfer Objelerinin (DTO) Oluşturulması
`Store.DtoLayer\CatalogDtos\CurrencyDtos` dizininde:
- `ResultCurrencyDto.cs`
- `CreateCurrencyDto.cs`
- `UpdateCurrencyDto.cs`

`Store.DtoLayer\CatalogDtos\MeasurementDtos` dizininde:
- `ResultMeasurementDto.cs`
- `CreateMeasurementDto.cs`
- `UpdateMeasurementDto.cs`
*Bu nesneler `IsActive`, `IsDeleted`, `DeleteUserId`, `DeleteDate` gibi özellikler eklenerek listeleme ve "silinmiş kayıt" gösterimi için hazırlandı.*

## 2. Controller'ların Eklenmesi
`Store.WebUI\Areas\Admin\Controllers` dizininde:
- `CurrencyController.cs`
- `MeasurementController.cs`
*Bu controller'lar, Katalog servisine (port 7301) HTTP üzerinden istek atacak şekilde ayarlandı. Daha önce CountryController'da kurulan yapı (Get, Create, Update, Soft Delete) tamamen aynı mantıkla uygulandı.*

## 3. Görünümlerin (Views) Oluşturulması
`Store.WebUI\Areas\Admin\Views\Currency\Index.cshtml`
`Store.WebUI\Areas\Admin\Views\Measurement\Index.cshtml`
* Görünüm standartları olarak `Country` modülünde yer alan renk, çerçeve, silinmiş kayıtları üstü çizili (kırmızı arka planlı) gösterme gibi özellikler aynen aktarıldı.
* AJAX ile Ekleme, Güncelleme ve modal içerisine verileri çekip Detay görüntüleme yapısı başarıyla eklendi.

## 4. Sol Menü Bağlantıları (Layout Düzenlemesi)
`_AdminLayoutComponentLeftMenuPartial\Default.cshtml`
* "Genel Ayarlar" alt menüsündeki "Para Birimleri" ve "Ölçü İşlemleri" bağlantılarına ilgili controller/action linkleri ( `/Admin/Currency/Index` ve `/Admin/Measurement/Index` ) eklendi.
* Bulunulan sayfaya göre menü elemanının aktif/seçili (active class) olması için Route üzerinden kontrol (c == "Currency", c == "Measurement") eklendi.

## 5. Projenin Derlenmesi (Build)
* Tüm oluşturulan C# kodlarındaki hataların giderildiğinden emin olmak için `dotnet build` komutu yürütüldü ve başarıyla derlendi.
