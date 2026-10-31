# 🚀 Yeni Bir Entity Eklerken Yapılması Gerekenler (Yaşam Döngüsü Raporu)

Bu rehber, projenizde (örneğin `Store.Location` mikroservisinde) baştan sona eksiksiz bir şekilde yeni bir Entity oluşturup, API ve Web UI tarafına kadar tüm katmanlarda çalışır hale getirmeniz için hazırlanmıştır. Rapor, **Country** entitisi referans alınarak adım adım düzenlenmiştir.

Lütfen yeni bir Entity eklerken (Örn: `Region`, `Town` vb.) aşağıdaki adımları sırasıyla (Checklist olarak) takip edin.

---

## 🏗️ Adım 1: Domain Katmanı (Veritabanı Karşılığı)
İlk adımda veritabanında oluşacak tablomuzun C# tarafındaki nesnesini oluşturmalıyız.

*   **Proje:** `Store.Location.Domain`
*   **Klasör:** `Entities`
*   **İşlem:** `YeniEntity.cs` adında sınıfı oluşturun.
*   **Detay:**
    *   Eğer base bir entity sınıfı kullanılıyorsa (`BaseEntity` vb.), bu sınıftan türetmeyi unutmayın.
    *   Entity'nin property'lerini (Id, Name, IsActive, IsDeleted vb.) tanımlayın. Base entity'de Id, InsertDate, IsDeleted gibi alanlar zaten varsa sadece özel alanları yazın.

---

## 📦 Adım 2: DTO (Data Transfer Object) Katmanı
Veritabanı nesnemizi (Entity) API ve UI arasında doğrudan taşımamak için ara taşıyıcı nesneler oluşturmalıyız.

*   **Proje:** `Store.DtoLayer`
*   **Klasör:** `LocationDtos/YeniEntityDtos`
*   **İşlem:** Aşağıdaki 3 DTO sınıfını oluşturun:
    1.  `CreateYeniEntityDto.cs` (Ekleme işlemi için gerekli alanlar)
    2.  `UpdateYeniEntityDto.cs` (Güncelleme işlemi için gerekli alanlar, `Id` içermelidir)
    3.  `ResultYeniEntityDto.cs` (Listeleme ve okuma işlemleri için gösterilecek alanlar)

---

## ⚙️ Adım 3: Application (CQRS) Katmanı
API üzerinden gelecek isteklerin iş mantığını yöneteceğimiz katmandır. CQRS deseni uygulandığı için her bir işlem için `Command`/`Query` ve `Handler` sınıflarını oluşturmamız gerekiyor.

*   **Proje:** `Store.Location.Application`
*   **Klasörler:** `Features/CQRS/Commands`, `Features/CQRS/Queries`, `Features/CQRS/Handlers`

### 3.1. Command ve Query Sınıfları (İstek Nesneleri)
*   **Commands (YeniEntityCommands klasörüne):**
    *   `CreateYeniEntityCommand.cs` (Create DTO'ya benzer alanlar içerir)
    *   `UpdateYeniEntityCommand.cs` (Update DTO'ya benzer alanlar içerir)
    *   `RemoveYeniEntityCommand.cs` (Sadece `Guid Id` içerir - Hard Delete)
    *   `SoftDeleteYeniEntityCommand.cs` (Sadece `Guid Id` içerir - Çöp Kutusuna Gönder)
    *   `RestoreYeniEntityCommand.cs` (Sadece `Guid Id` içerir - Çöp Kutusundan Çıkar)
*   **Queries (YeniEntityQueries klasörüne):**
    *   `GetYeniEntitiesQuery.cs` (İsteğe bağlı `bool showDeleted = false` parametresi alabilir)
    *   `GetYeniEntityByIdQuery.cs` (Sadece `Guid Id` içerir)

### 3.2. Handler Sınıfları (İşlemi Yapan Sınıflar)
*   **Handlers (YeniEntityHandlers klasörüne):**
    *   `CreateYeniEntityCommandHandler.cs` (`_repository.CreateAsync`)
    *   `UpdateYeniEntityCommandHandler.cs` (`_repository.UpdateAsync`)
    *   `RemoveYeniEntityCommandHandler.cs` (`_repository.DeleteAsync`)
    *   `SoftDeleteYeniEntityCommandHandler.cs` (İlgili kaydı çekip `IsDeleted = true` yapar veya `_repository.SoftDeleteAsync`)
    *   `RestoreYeniEntityCommandHandler.cs` (Kayıt `IgnoreQueryFilters` ile bulunur, `_repository.RestoreAsync` çağırılır)
    *   `GetYeniEntitiesQueryHandler.cs` (Tüm kayıtları veya silinmeyenleri listeleyip DTO'ya map'ler)
    *   `GetYeniEntityByIdQueryHandler.cs` (Tek kaydı bulup DTO'ya map'ler)

---

## 🗄️ Adım 4: Persistence Katmanı (Veritabanı Güncellemesi)
Oluşturduğumuz Entity'yi DbContext'e bildirip veritabanında tablosunu açtırmalıyız.

*   **Proje:** `Store.Location.Persistence`
*   **İşlem 1 (DbContext):** `LocationContext.cs` içerisine `public DbSet<YeniEntity> YeniEntities { get; set; }` property'sini ekleyin.
*   **İşlem 2 (Migration):** Package Manager Console'da Default Project olarak `Store.Location.Persistence` seçiliyken:
    *   `Add-Migration Add_YeniEntity_Table`
    *   `Update-Database` komutlarını çalıştırın.

---

## 🌐 Adım 5: Web API Katmanı
Oluşturduğumuz CQRS yapılarını dış dünyaya açtığımız yer.

*   **Proje:** `Store.Location.WebApi`
*   **İşlem 1 (Controller):** `Controllers` klasörü altına `YeniEntityController.cs` (veya `YeniEntitiesController.cs`) oluşturun.
    *   Yukarıda yazdığınız **7 adet Handler'ı** constructor'da (Dependency Injection) içeri alın.
    *   Şu uçları (endpoint) hazırlayın:
        *   `[HttpGet]` -> Tümünü Listele
        *   `[HttpGet("{id}")]` -> Id'ye göre getir
        *   `[HttpPost]` -> Yeni Ekle
        *   `[HttpPut]` -> Güncelle
        *   `[HttpDelete("{id}")]` -> Kalıcı Sil (Hard Delete)
        *   `[HttpDelete("soft-delete/{id}")]` -> Çöp Kutusuna Gönder (Soft Delete)
        *   `[HttpPut("restore/{id}")]` -> Geri Al (Restore)
*   **İşlem 2 (DI Container / Program.cs):**
    *   `Program.cs` dosyasını açın.
    *   CQRS Handlers Register bölgesine bu 7 handler için `builder.Services.AddScoped<...>();` tanımlamalarını ekleyin. *(Bunu unutursanız API 500 hatası fırlatır!)*

---

## 💻 Adım 6: Web UI Katmanı (MVC - Frontend)
Kullanıcının bu verileri göreceği ekranların hazırlanması.

*   **Proje:** `Store.WebUI`

### 6.1. Controller İşlemleri
*   **Klasör:** `Areas/Admin/Controllers`
*   **İşlem:** `YeniEntityController.cs` oluşturun.
*   `IHttpClientFactory` kullanarak Web API projenize (Örn: `https://localhost:7303/api/YeniEntity`) istek atacak Action'ları yazın.
*   Yazılması gereken Action'lar:
    *   `Index(bool showDeleted = false)` (Listeleme View'ı)
    *   `Create` (HttpPost - Ekleme İşlemi)
    *   `Edit` (HttpPost - Güncelleme İşlemi)
    *   `DeleteYeniEntity(Guid id)` (Soft Delete İşlemi)
    *   `RestoreYeniEntity(Guid id)` (Restore İşlemi)

### 6.2. View (Görünüm) İşlemleri
*   **Klasör:** `Areas/Admin/Views/YeniEntity`
*   **İşlem:** `Index.cshtml` dosyasını oluşturun. (Country vb. ekranlardan kopyalayabilirsiniz).
*   **Detaylar:**
    *   DataTable ile verileri listeleyin.
    *   **"Silinenleri Göster/Gizle"** butonlarını ekleyin.
    *   Tablonun işlemler (Actions) sütununda `@if(item.IsDeleted)` kontrolü yapın:
        *   Silinmişse: **Detay** ve **Geri Al** butonu.
        *   Silinmemişse: **Detay**, **Güncelle** ve **Sil** butonu.
    *   Ekleme ve Güncelleme Modal formlarını sayfa altına ekleyip AJAX veya standart form post ile submit edilmesini sağlayın.

### 6.3. Sidebar (Menü) Güncellemesi
*   Kullanıcının yeni sayfaya girebilmesi için Layout veya Sidebar menüsüne "Yeni Entity İşlemleri" adında link ekleyin.
*   **Dosya:** `Areas/Admin/Views/AdminLayout/PartialSidebar.cshtml` (veya ilgili layout menü dosyası).

---

## ✅ Adım 7: Test ve Doğrulama
1.  **Derleme:** Tüm çözümü (`dotnet build`) derleyin ve hatasız olduğundan emin olun.
2.  **API Testi:** Swagger ekranına giderek Ekleme, Güncelleme, Soft-Delete ve Restore uçlarının `200 OK` döndüğünden emin olun.
3.  **UI Testi:** Tarayıcıdan giriş yapıp menüye tıklayın. Modal üzerinden kayıt ekleyin, silin ve "Silinenleri Göster" diyerek Geri Al (Restore) işleminin arayüzde doğru yansıdığını gözlemleyin.

🎉 **Tebrikler!** Bu 7 adımı sırasıyla ve atlamadan uyguladığınızda yeni Entity'niz sistemde %100 entegre olarak çalışacaktır.
