# Yapılan İşlemler - 002

CurrencyController yeni kayıt girişindeki GUID dönüştürme hatası çözülmüş, tüm projede ve hata yönetiminde profesyonel bir yapı kurulup entegrasyonu tamamlanmıştır.

## 1. GUID Dönüştürme Hatasının Çözülmesi ("Unrecognized Guid format.")
* **Sorun:** Formdan gönderilen veya `CreateCurrencyDto`/`CreateMeasurementDto` içinde varsayılan değer olarak tanımlanan `InsertUserId` bilgisi `"1"` dizesi (string) olarak geliyordu. Veri tabanı seviyesindeki `BaseEntity` üzerinde ise bu alan `Guid` türündeydi. AutoMapper, `"1"` dizesini geçerli bir Guid'e dönüştüremediği için `"Unrecognized Guid format."` hatası fırlatıyordu.
* **Yapılan Düzeltme:** 
  * DTO katmanındaki `CreateCurrencyDto` ve `CreateMeasurementDto` varsayılan değerleri geçerli bir boş GUID formatı olan `"00000000-0000-0000-0000-000000000000"` dizesine güncellendi.
  * İlgili ekranlardaki (Para Birimi, Ölçü Birimi, İl, İlçe, Mahalle, Ülke) form hidden input alanlarındaki `value="1"` değerleri `value="00000000-0000-0000-0000-000000000000"` olarak güncellendi.

## 2. Merkezi Hata Yönetimi Yapısının Kurulması (Error Handling)
* **Yeni Yapı:** [ErrorHelper.cs](file:///c:/User/Projects/Store/FrontEnds/Store.WebUI/Helpers/ErrorHelper.cs) adında genel ve statik bir yardımcı sınıf oluşturuldu. Bu sınıf, API tarafındaki global middleware'den veya veri tabanından dönen JSON formatındaki detaylı hata mesajlarını akıllıca çözümleyerek (parse ederek) anlamlı, temiz ve kullanıcı dostu hata dökümlerini ayırt etmektedir.
* **Uygulanan Controller'lar:**
  * Aşağıdaki tüm WebUI controller'larının `Create` (Post) ve `Edit` (Post) metotlarında `response.Content.ReadAsStringAsync()` ile yapılan doğrudan ham mesaj dönüşleri, `ErrorHelper.GetErrorMessageAsync(response)` çağrısı ile değiştirilerek profesyonel hata yönetimi sistemine entegre edilmiştir:
    * `CurrencyController`
    * `MeasurementController`
    * `CompanyController`
    * `WarehouseController`
    * `UserAddressController`
    * `ContactInfoController`
    * `BranchController`
    * `BranchAddressController`
    * `DealerController`
    * `CountryController`
    * `CityController`
    * `DistrictController`
    * `NeighborhoodController`

## 3. Derleme Testi
* Proje `dotnet build` komutu kullanılarak başarıyla derlenmiştir.
