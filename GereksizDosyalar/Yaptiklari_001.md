# Yapılan İşlemler (001)

Bu belgede Kategori (Category) modülünde "Silinen kategoriyi geri alma" ve "Kategori düzenleme" işlemleri sırasında yaşanan `404 Not Found` hatalarının çözümü için yapılan adımlar detaylandırılmıştır. İşlemler `CountryController` ve ilgili View (görünüm) sayfaları örnek alınarak yapılmıştır.

## 1. Hataların Tespiti
- Kategori silinmiş kayıtlarını geri almak için kullanılan butonun URL'si `RestoreCompany` olarak yazılmıştı. Ancak bu isimde bir Controller metodu bulunmuyordu.
- Kategori düzenlemek (Edit) için kullanılan butonun URL'si `Gategory/Edit` olarak hatalı (typo) yazılmıştı ve metot adı yanlıştı (`AddorEdit` olmalıydı).
- Kategori silme (Delete) butonu `DeleteCompany` URL'sine yönlendiriyordu. Ancak Controller'da `DeleteCategory` olarak tanımlıydı.

## 2. Görünüm (View) Sayfasının Düzeltilmesi
**Dosya:** `FrontEnds\Store.WebUI\Areas\Admin\Views\Category\Index.cshtml`

Aşağıdaki HTML linkleri (URL'ler) doğru Controller ve Action isimlerine göre güncellendi:
- `RestoreCompany` -> `RestoreCategory` (Geri alma butonu)
- `Gategory/Edit` -> `Category/AddorEdit` (Güncelleme butonu)
- `DeleteCompany` -> `DeleteCategory` (Silme butonu)

## 3. Controller'a Restore (Geri Alma) İşleminin Eklenmesi
**Dosya:** `FrontEnds\Store.WebUI\Areas\Admin\Controllers\CategoryController.cs`

Kategorileri geri alma işlemi için Controller sınıfına `RestoreCategory` isimli metot eklendi. Bu metot API'ye bir `POST` isteği (API tarafında `[HttpPost("Restore/{id}")]` şeklinde belirtilmişti) gönderecek şekilde yazıldı:
```csharp
public async Task<IActionResult> RestoreCategory(string id)
{
    var client = _httpClientFactory.CreateClient("ApiClient");
    var responseMessage = await client.PostAsync($"{_apiSettings.CatalogBaseUrl}/api/categories/Restore/{id}", null);
    if (responseMessage.IsSuccessStatusCode)
    {
        return RedirectToAction("Index", "Category", new { area = "Admin", showDeleted = true });
    }

    TempData["ErrorMessage"] = "Geri alma işlemi sırasında bir hata oluştu.";
    return RedirectToAction("Index", "Category", new { area = "Admin", showDeleted = true });
}
```

## 4. Test ve Derleme (Build)
Düzeltmeler sonrasında terminal üzerinden proje `dotnet build` komutu ile derlendi. Derleme işleminde 0 Hata ile karşılaşıldı. Belirtilen `404 Not Found` hatalarının kaynağı olan URL sorunları ve eksik metot problemi başarıyla giderildi.
