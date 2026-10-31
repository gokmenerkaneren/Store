# 🌳 STORE PROJESİ — DETAYLI DOSYA AĞACI ANALİZİ

> **Hazırlayan:** Kıdemli PM + Kıdemli İş Analisti + Kıdemli Yazılım Mimarı + Kıdemli DBA + Kıdemli Siber Güvenlik Mühendisi + Kıdemli Test Mühendisi  
> **Tarih:** 31 Temmuz 2026  
> **Kaynak:** ProjeAnalizi_2026.07.30.md + Canlı Kaynak Kod İncelemesi  
> **Kapsam:** `bin/`, `obj/`, `.vs/`, `publish/`, `Migrations/` klasörleri hariç tutulmuştur.

---

## İÇİNDEKİLER

1. Kök Dizin (Store/) — Belge ve Yardımcı Dosyalar  
2. Store.slnx — Çözüm Dosyası  
3. FrontEnds/ — Kullanıcı Arayüzü Katmanı  
4. IdentityServer/ — Kimlik ve Yetkilendirme Servisi  
5. Services/ — Mikroservisler  
6. Store.DtoLayer/ — Paylaşımlı DTO Kütüphanesi  
7. docker-compose.yml  
8. Mimari Bağımlılık Haritası  
9. Dosya Sağlığı ve Teknik Borç Özeti  

---

## 1. Kök Dizin (Store/) — Belge ve Yardımcı Dosyalar

Kök dizin kaynak kod içermez; proje dokümantasyonu, PowerShell scriptleri ve geliştirici notlarından oluşur.

| Dosya | Rol | Açıklama |
|:------|:----|:---------|
| `Store.slnx` | Çözüm Dosyası | Tüm projeleri bir arada tutan Visual Studio çözüm dosyası (yeni .slnx XML formatı). |
| `Store.slnLaunch.user` | IDE Ayarları | Visual Studio başlatma profili. Kaynak kontrolüne alınmamalı. |
| `agent.md` | Geliştirici Talimatı | AI asistanının projeyi anlayabilmesi için hazırlanan bağlam dosyası. Tech stack, mimari kararlar, bilinen borçlar ve çalışma prensibi tanımlanmış. |
| `project_context.md` | Proje Bağlamı | İş gereksinimleri ve teknik bağlam özeti. |
| `ProjeAnalizi_2026.07.30.md` | 6-Şapka Analizi | 6 farklı uzman perspektifinden kapsamlı proje analizi raporu. |
| `ProjeDosyaAgaci_2026.07.31.md` | Bu Dosya | Tüm kaynak dosyaların detaylı analizi. |
| `GelistirilebilirYanlar_2026.07.30.md` | İyileştirme Listesi | Mimari, güvenlik ve performans iyileştirme alanları (24KB). |
| `VeriTabani_01.md` | VT Dokümantasyonu | Veritabanı şema analizi — 1. bölüm. |
| `VeriTabani_02.md` | VT Dokümantasyonu | Veritabanı şema analizi — 2. bölüm (41KB, kapsamlı). |
| `ProjeYapisi_02.md` | Mimari Belge | Genel mimari yapıyı anlatan doküman (16KB). |
| `ProjeDokumai.md` | Proje Belgesi | Genel proje dokümantasyonu. |
| `20260619_SonDurum.md` | Durum Raporu | 19 Haziran 2026 itibarıyla projenin anlık durumu (11.5KB). |
| `BirEntityIcinYapilacaklar.md` | Geliştirici Notu | Tek bir entity eklerken izlenecek adımlar. Yeni geliştirici rehberi. |
| `GeriAlmaProceduresiHayatDongusu.md` | İş Süreci | Geri alma işleminin iş akışı ve yaşam döngüsü. |
| `hatalar ve çözümleri.md` | Hata Kütüphanesi | Karşılaşılan hatalar ve çözümleri. Yeni geliştiriciler için kritik rehber. |
| `IdentityServerVersionErrorSolution.txt` | Çözüm Notu | IdentityServer versiyon uyumsuzluk hatasının çözümü (45 byte). |
| `PortNumbers.txt` | Port Rehberi | Servislerin çalıştığı port numaraları. Hızlı geliştirme referansı. |
| `YapilanIslemler.md` | Değişiklik Günlüğü | Genel yapılan işlemler kaydı. |
| `YapilanIslemler_001.md` | Değişiklik Günlüğü | Spesifik bir seansa ait işlem kaydı. |
| `YapilanIslemler_002.md` | Değişiklik Günlüğü | Spesifik bir seansa ait işlem kaydı. |
| `Yaptiklari_001.md` | Sprint Özeti | Tamamlanan görevlerin özeti. |
| `FixCsproj.ps1` | PowerShell Script | .csproj dosyalarındaki sorunları otomatik düzelten script (528B). |
| `FixMissingViews.ps1` | PowerShell Script | Eksik Razor view dosyalarını tespit edip oluşturan script (14KB). |
| `RegenerateAllWebUI.ps1` | PowerShell Script | Tüm WebUI görünümlerini yeniden oluşturan büyük otomasyon scripti (14KB). |
| `dart_test.log` | YANLIŞ DOSYA | Flutter/Dart test log. Bu .NET projesinde yeri yok. Silinmeli, .gitignore ekle! |

> [!WARNING]
> `dart_test.log` dosyası .NET proje kökünde bulunmamalıdır. Muhtemelen başka bir projeden taşınmış. Temizlenmeli.

---

## 2. Store.slnx — Çözüm Dosyası

Visual Studio Solution dosyası (yeni XML formatı). Aşağıdaki 13 projeyi kapsar:

```
Store.slnx
├── FrontEnds/Store.WebUI/Store.WebUI.csproj
├── Store.DtoLayer/Store.DtoLayer.csproj
├── IdentityServer/Store.IdentityServer/Store.IdentityServer.csproj
├── Services/Catalog/Store.Catalog/Store.Catalog.csproj
├── Services/Discount/Store.Discount/Store.Discount.csproj
├── Services/Location/Core/Store.Location.Application/Store.Location.Application.csproj
├── Services/Location/Core/Store.Location.Domain/Store.Location.Domain.csproj
├── Services/Location/Infrastructure/Store.Location.Persistence/Store.Location.Persistence.csproj
├── Services/Location/Presentation/Store.Location.WebApi/Store.Location.WebApi.csproj
├── Services/Order/Core/Store.Order.Application/Store.Order.Application.csproj
├── Services/Order/Core/Store.Order.Domain/Store.Order.Domain.csproj
├── Services/Order/Core/Infrastructure/Store.Order.Persistance/Store.Order.Persistance.csproj
└── Services/Order/Presentation/Store.Order.WebApi/Store.Order.WebApi.csproj
```


---

## 3. FrontEnds/ — Kullanıcı Arayüzü Katmanı

### Store.WebUI — ASP.NET Core MVC Uygulaması

**Amaç:** Kullanıcıların (B2C müşteri + yönetici) sisteme erişeceği web arayüzü.  
**Mimari:** ASP.NET Core MVC (Razor Views). API'lere doğrudan HTTP çağrısı yapıyor (Gateway yok!).  
**Veritabanı:** Yok — tamamen API tüketicisi.

```
FrontEnds/
└── Store.WebUI/
    ├── Program.cs (2.6KB)
    │   ACIKLAMA: Uygulama giriş noktası ve DI container kurulumu.
    │   KRİTİK SORUN: SSL bypass (ServerCertificateCustomValidationCallback => true)
    │   tüm ortamlarda aktif. Sadece #if DEBUG bloğuna alınmalı.
    │
    ├── Store.WebUI.csproj (16.8KB)
    │   ACIKLAMA: Proje bağımlılıkları. Çok büyük boyutu libman referansları ve
    │   statik dosyalar içerdiğinden kaynaklanıyor. Temizlik adayı.
    │
    ├── appsettings.json (423B)
    │   ACIKLAMA: API URL'leri ve yapılandırma. SORUN: Secret bilgiler düz metin.
    │
    ├── appsettings.Development.json (345B)
    │   ACIKLAMA: Geliştirme ortamı yapılandırma overrides.
    │
    ├── Controllers/
    │   └── HomeController.cs (819B)
    │       ACIKLAMA: Sadece ana sayfa (/) endpoint'ini yöneten minimal controller.
    │
    ├── Areas/Admin/
    │   ACIKLAMA: ASP.NET Core Areas özelliği kullanılarak yönetici paneli ayrı bir
    │   modül olarak organize edilmiş. Doğru bir ayrıştırma kararı.
    │
    │   Controllers/ (18 dosya):
    │   ├── AdminLayoutController.cs (273B)
    │   │   ACIKLAMA: Admin panel layout ve navigasyon controller'ı. Minimal.
    │   │
    │   ├── BrandController.cs (6.7KB)
    │   │   ACIKLAMA: Marka yönetimi. Index(Liste), Create(GET/POST),
    │   │   Update(GET/POST), Delete işlemleri. Catalog API'ye HTTP çağrısı.
    │   │
    │   ├── CategoryController.cs (7.1KB)
    │   │   ACIKLAMA: Kategori yönetimi. Üst kategori seçimi (ParentId) dahil
    │   │   hiyerarşik yapı yönetimi. Catalog API'ye HTTP çağrısı.
    │   │
    │   ├── ProductController.cs (23KB)
    │   │   ACIKLAMA: En büyük ve en karmaşık controller. Ürün CRUD işlemleri +
    │   │   görsel yükleme + fiyat yönetimi + kategori/marka/ölçü birimi seçimi içeriyor.
    │   │   Refactoring adayı — parçalanabilir.
    │   │
    │   ├── CurrencyController.cs (3.9KB)
    │   │   ACIKLAMA: Para birimi (TRY, USD, EUR) yönetimi. Catalog API'ye HTTP çağrısı.
    │   │
    │   ├── MeasurementController.cs (4KB)
    │   │   ACIKLAMA: Ölçü birimi (Adet, Kg, Metre) yönetimi. Catalog API'ye HTTP çağrısı.
    │   │
    │   ├── CompanyController.cs (5.9KB)
    │   │   ACIKLAMA: Şirket yönetimi. Location API'ye HTTP çağrısı.
    │   │
    │   ├── BranchController.cs (6.1KB)
    │   │   ACIKLAMA: Şube yönetimi. Şirket seçimi ve şubeye ait bilgileri yönetir.
    │   │   Location API'ye HTTP çağrısı.
    │   │
    │   ├── BranchAddressController.cs (3KB)
    │   │   ACIKLAMA: Şube adres yönetimi. Location API'ye HTTP çağrısı.
    │   │
    │   ├── CityController.cs (5.5KB)
    │   │   ACIKLAMA: Şehir yönetimi. Ülke dropdown ile bağlantılı. Location API.
    │   │
    │   ├── CountryController.cs (5.6KB)
    │   │   ACIKLAMA: Ülke yönetimi (Türkiye, vb.). Location API'ye HTTP çağrısı.
    │   │
    │   ├── DistrictController.cs (6.4KB)
    │   │   ACIKLAMA: İlçe yönetimi. Şehir dropdown ile bağlantılı. Location API.
    │   │
    │   ├── NeighborhoodController.cs (9.7KB)
    │   │   ACIKLAMA: Mahalle yönetimi — en karmaşık lokasyon controller'ı. 9.7KB boyutu,
    │   │   İlçe + Şehir cascading dropdown ve özel filtreleme mantığından kaynaklanıyor.
    │   │
    │   ├── DealerController.cs (5.8KB)
    │   │   ACIKLAMA: Bayi yönetimi. DiscountRate, CreditLimit, CurrentBalance alanları
    │   │   form üzerinden yönetiliyor. Location API'ye HTTP çağrısı.
    │   │
    │   ├── ContactInfoController.cs (3KB)
    │   │   ACIKLAMA: İletişim bilgisi yönetimi (Email, Telefon, Web). Location API.
    │   │
    │   ├── UserAddressController.cs (3KB)
    │   │   ACIKLAMA: Kullanıcı adres görüntüleme/yönetimi. KVKK anonimleştirme
    │   │   butonu burada sunuluyor olabilir. Location API'ye HTTP çağrısı.
    │   │
    │   ├── WarehouseController.cs (5.8KB)
    │   │   ACIKLAMA: Depo yönetimi. Şubeye bağlı depo CRUD işlemleri. Location API.
    │   │
    │   └── DenemeEkraniController.cs (552B)
    │       ACIKLAMA: SORUN! Test/deneme amaçlı yazılmış controller. Üretim öncesi
    │       silinmesi şart. Kod kalitesi açısından kabul edilemez.
    │
    │   Views/Admin/ (18 alt klasör + views):
    │   ACIKLAMA: Her controller için ayrı bir View klasörü. Razor .cshtml dosyaları.
    │   Tipik içerik: Index.cshtml, Create.cshtml, Update.cshtml, Detail.cshtml.
    │   AdminLayout/, DenemeEkrani/ (silinmeli), Shared/ (ortak _Layout) dahil.
    │
    ├── Models/
    │   ├── ApiSettings.cs (343B)
    │   │   ACIKLAMA: Catalog/Location/Order/Discount API URL'lerini tutan
    │   │   strongly-typed settings modeli. appsettings.json'dan IOptions<T> ile okunur.
    │   │
    │   └── ErrorViewModel.cs (205B)
    │       ACIKLAMA: Hata sayfasında görüntülenen veri modeli. RequestId alanı.
    │
    ├── Views/
    │   ├── Home/               ACIKLAMA: Ana sayfa görünümleri
    │   ├── Shared/             ACIKLAMA: Ortak _Layout.cshtml, _ValidationScripts vb.
    │   ├── Default.cshtml      ACIKLAMA: Varsayılan görünüm
    │   ├── Head.cshtml         ACIKLAMA: HTML head kısmı için partial view
    │   ├── Index.cshtml        ACIKLAMA: Giriş sayfası view
    │   ├── _ViewImports.cshtml ACIKLAMA: Global namespace, Tag Helper importları
    │   └── _ViewStart.cshtml   ACIKLAMA: Tüm view'lar için varsayılan layout tanımı
    │
    ├── Helpers/
    │   ├── ErrorHelper.cs (1.6KB)
    │   │   ACIKLAMA: HTTP hata kodlarını (404, 500 vb.) anlamlı mesajlara çeviren
    │   │   yardımcı sınıf. View katmanında kullanılır.
    │   │
    │   └── NavigationHelper.cs (932B)
    │       ACIKLAMA: Admin panel navigasyon linklerini ve menü aktif durumunu
    │       yöneten yardımcı sınıf.
    │
    └── wwwroot/
        ACIKLAMA: Statik dosyalar (CSS, JavaScript, görseller, Bootstrap, jQuery).
        CDN yerine local kütüphaneler libman ile yönetiliyor.
```

---

## 4. IdentityServer/ — Kimlik ve Yetkilendirme Servisi

**Amaç:** OAuth 2.0 ve OpenID Connect ile tüm sistemin kimlik doğrulama/yetkilendirme merkezi.  
**Veritabanı:** PostgreSQL (StoreIdentityDB) — ASP.NET Identity (~12 tablo).  
**Port:** localhost:5001  
**KRİTİK:** IdentityServer4 End-of-Life (Aralık 2022). Güvenlik yamaları gelmiyor.

```
IdentityServer/
└── Store.IdentityServer/
    ├── Program.cs (3.1KB)
    │   ACIKLAMA: Uygulama giriş noktası. Startup.cs'i çağırır (.NET 6 öncesi stil).
    │
    ├── Startup.cs (3.9KB)
    │   ACIKLAMA: IdentityServer4 + ASP.NET Identity + EF Core + CORS + Cookie
    │   yapılandırması. Tüm middleware pipeline burada tanımlanmış.
    │
    ├── Config.cs (4.4KB)
    │   ACIKLAMA: TÜM SİSTEMİN GÜVENLİK KONFIGÜRASYONU BURADADIR!
    │   - ApiScopes: LocationFullPermission, DiscountFullPermission,
    │                CatalogFullPermission, OrderFullPermission
    │   - Clients: StoreVisitorId (sadece konum), StoreManagerId (indirim+konum),
    │               StoreAdminId (her şey)
    │   - IdentityResources: openid, profile, email, roles
    │   Değişiklik yaparken son derece dikkatli olunmalı!
    │
    ├── SeedData.cs (4.6KB)
    │   ACIKLAMA: İlk kurulumda bir kez çalışır. Veritabanına başlangıç kullanıcıları,
    │   rolleri ve client'ları ekler. Sonraki çalıştırmalarda tekrar eklemez (idempotent).
    │
    ├── Store.IdentityServer.csproj (1.5KB)
    │   ACIKLAMA: IdentityServer4, EF Core, Npgsql bağımlılıkları.
    │
    ├── appsettings.json (222B)
    │   ACIKLAMA: PostgreSQL bağlantı dizesi. KRİTİK SORUN: Şifre düz metin!
    │
    ├── tempkey.jwk (1.7KB)
    │   ACIKLAMA: BÜYÜK GÜVENLİK AÇIĞI! JWT tokenları imzalamak için kullanılan
    │   RSA özel anahtarını içerir. Git'e yüklenmiş olması kabul edilemez.
    │   .gitignore'a eklenmeli, üretimde X.509 sertifikasıyla değiştirilmeli!
    │
    ├── updateUI.ps1 (11.7KB)
    │   ACIKLAMA: IdentityServer4 hazır UI şablonlarını indirip güncelleyen script.
    │
    ├── Data/
    │   ACIKLAMA: ASP.NET Identity EF Core migration context dosyaları.
    │   ApplicationDbContext burada tanımlı.
    │
    ├── Dtos/
    │   ACIKLAMA: Login (LoginDto) ve Register (RegisterDto) transfer nesneleri.
    │
    ├── Models/
    │   ACIKLAMA: ApplicationUser (ASP.NET Identity genişletilmiş kullanıcı modeli).
    │
    ├── Controllers/
    │   ACIKLAMA: Register ve Login API endpoint'leri. Token dönmez — yönlendirme yapar.
    │
    └── Quickstart/
        ACIKLAMA: IdentityServer4 hazır UI şablonu. Özelleştirilebilir sayfa/controller seti.
        │
        ├── Account/
        │   ACIKLAMA: Login, Logout, ExternalLogin, Register UI controller ve view'ları.
        │   Kullanıcı giriş/çıkış akışı burada.
        │
        ├── Consent/
        │   ACIKLAMA: OAuth consent ekranı. Kullanıcıya "Bu uygulama X iznini istiyor"
        │   diye sorar. Implicit flow ve Authorization Code flow için.
        │
        ├── Device/
        │   ACIKLAMA: Device Authorization Grant akışı için UI. IoT ve TV cihazları için.
        │
        ├── Diagnostics/
        │   ACIKLAMA: Debug bilgileri ekranı. Sadece geliştirme ortamında aktif!
        │   Production'da kesinlikle kapalı olmalı.
        │
        ├── Grants/
        │   ACIKLAMA: Kullanıcının verdiği yetkileri (grant) yönettiği sayfa.
        │   "Bu uygulamanın erişimini iptal et" işlevi.
        │
        ├── Home/
        │   ACIKLAMA: IdentityServer varsayılan ana sayfası ve hata sayfası.
        │
        ├── TestUsers.cs (2.2KB)
        │   ACIKLAMA: SORUN! Hardcoded test kullanıcıları (alice, bob vb.). 
        │   Geliştirme ortamında kullanışlı ama Production'da devre dışı bırakılmalı!
        │   Gerçek kullanıcılar SeedData.cs + veritabanıyla yönetilmeli.
        │
        ├── Extensions.cs (984B)
        │   ACIKLAMA: IS4 HttpContext extension metodları. Token parsing ve claim okuma.
        │
        └── SecurityHeadersAttribute.cs (2.8KB)
            ACIKLAMA: Güvenlik HTTP başlıklarını ekleyen action filter attribute.
            X-Content-Type-Options, Content-Security-Policy (CSP), Referrer-Policy vb.
            Tüm IS4 controller action'larına uygulanıyor. Güvenlik açısından değerli.

---

## 5. Services/ — Mikroservisler

### 5.1 Catalog Servisi — MongoDB Monolitik Yapı

**Amaç:** Ürün kataloğunu (ürünler, markalar, kategoriler, fiyatlar, görseller) yönetir.  
**Mimari:** Monolitik — tüm kod tek projede. CQRS kısmen (sadece Product için) uygulanmış.  
**Veritabanı:** MongoDB (StoreMD) — 8 koleksiyon.

```
Services/
└── Catalog/
    └── Store.Catalog/
        ├── Program.cs (6.3KB)
        │   ACIKLAMA: DI, MongoDB servis kaydı, JWT, CORS, Swagger, AutoMapper,
        │   ExceptionMiddleware kurulumu. KRİTİK SORUN: SSL bypass aktif!
        │
        ├── Store.Catalog.csproj (1.5KB)
        │   ACIKLAMA: MİMARİ SORUN! Store.Location.Domain'e ProjectReference var.
        │   Bu, Catalog'un Location domain sınıflarına derleme zamanında bağımlı olması
        │   demek. Mikroservis prensibini ihlal ediyor!
        │
        ├── appsettings.json (983B)
        │   ACIKLAMA: MongoDB bağlantı bilgileri + JWT ayarları. Şifre düz metin!
        │
        ├── Context/
        │   └── MongoDbService.cs
        │       ACIKLAMA: MongoDB bağlantısını ve koleksiyonlara erişimi yöneten merkezi sınıf.
        │       IMongoCollection<T> döndüren generic GetCollection<T>(collectionName) metodu içerir.
        │       Tüm service sınıfları bu üzerinden koleksiyon alır. Dependency Injection ile inject edilir.
        │
        ├── Entities/
        │   ├── Brand.cs (686B)
        │   │   ACIKLAMA: Marka domain sınıfı. BrandName, ImageUrl alanları.
        │   │   BaseEntity'den türeyerek Id(Guid), audit alanları, soft delete alır.
        │   │   MongoDB'de "Brands" koleksiyonuna map edilir.
        │   │
        │   ├── Category.cs (932B)
        │   │   ACIKLAMA: Kategori domain sınıfı. CategoryName, ParentId (recursive ağaç yapısı).
        │   │   ParentId null ise kök kategori, dolu ise alt kategori. Teorik sonsuz derinlik.
        │   │
        │   ├── Currency.cs (845B)
        │   │   ACIKLAMA: Para birimi. CurrencyCode (TRY, USD, EUR — ISO 4217),
        │   │   CurrencyName, Symbol (₺, $, €). Fiyat hesaplamalarında kullanılacak.
        │   │
        │   ├── Measurement.cs (852B)
        │   │   ACIKLAMA: Ölçü birimi. MeasurementName (Adet, Kilogram, Metre),
        │   │   MeasurementUnit. Ürünlerin miktarını ifade etmek için.
        │   │
        │   ├── Product.cs (3.3KB)
        │   │   ACIKLAMA: Ana ürün kartı. En kritik entity.
        │   │   SORUN: `public new string Id` tanımlamış ama Location.Domain.BaseEntity'den
        │   │   Guid Id kalıtıyor. ID Shadowing! MongoDB string _id ister, BaseEntity Guid.
        │   │   İçerik: ProductName, ProductCode, SKU, Barcode, BrandId, CategoryId,
        │   │   MeasurementId, CurrencyId, VatRate(KDV%), ProductStatus, SEO alanları.
        │   │
        │   ├── ProductDetail.cs (1.5KB)
        │   │   ACIKLAMA: Ürün teknik detayları. ProductId ile Product'a bağlı.
        │   │   Renk, boyutlar (GenislikCm, YukseklikCm, DerinlikCm), AgirlikKg,
        │   │   Malzeme, Garanti, arama etiketleri (Tags). MongoDB'de ayrı koleksiyon.
        │   │
        │   ├── ProductImage.cs (1KB)
        │   │   ACIKLAMA: Ürün görseli. ProductId bağlantısı, ImageUrl (CDN/S3 path),
        │   │   DisplayOrder (galeri sırası), IsMainImage flag. Bir ürünün N görseli olabilir.
        │   │
        │   └── ProductPrice.cs (2.1KB)
        │       ACIKLAMA: Fiyat kartı — en güçlü tasarıma sahip entity.
        │       PriceType enum: Cost(1)=Maliyet, Retail(2)=Perakende, Rental(3)=Kiralama,
        │       Wholesale(4)=Toptan, Wholerental(5)=Toptan Kiralama.
        │       Amount ve DiscountedAmount: Decimal128 (MongoDB para hassasiyeti için doğru seçim).
        │       StartDate, EndDate: Dönemsel kampanya desteği.
        │       PriceListTierId: VIP bayi kademesi için fiyat listesi.
        │
        ├── Controllers/
        │   ACIKLAMA: İnce controller prensibi uygulanmış. Controller sadece HTTP isteğini
        │   alır, Service katmanına yönlendirir, yanıtı döndürür. İş mantığı yok.
        │   │
        │   ├── BrandsController.cs (2.2KB)         GET/POST/PUT/DELETE /api/brands
        │   ├── CategoriesController.cs (2.5KB)     GET/POST/PUT/DELETE /api/categories
        │   ├── CurrenciesController.cs (2KB)       Para birimi endpoint'leri.
        │   ├── MeasurementsController.cs (2KB)     Ölçü birimi endpoint'leri.
        │   ├── ProductDetailsController.cs (2.5KB) Ürün detay endpoint'leri.
        │   ├── ProductImagesController.cs (5.4KB)  Dosya yükleme (IFormFile) mantığı içerir.
        │   ├── ProductPricesController.cs (2.7KB)  Fiyat endpoint'leri (5 tip).
        │   └── ProductsController.cs (2.7KB)       Ürün CRUD endpoint'leri.
        │
        ├── Services/
        │   ACIKLAMA: Repository Pattern yerine kullanılan servis katmanı. Her entity için
        │   Interface + Implementation çifti var. MongoDB koleksiyonlarına doğrudan erişim.
        │   │
        │   ├── UserContextAccessor.cs (1.6KB)
        │   │   ACIKLAMA: IHttpContextAccessor üzerinden JWT token claim'lerini okur.
        │   │   Özellikle InsertUserId ve UpdateUserId doldurmak için kullanılır.
        │   │   Her audit işleminde devreye girer.
        │   │
        │   ├── ProductServices/
        │   │   ├── IProductService.cs (629B)   GetAll, GetById, Create, Update, Delete arayüzü.
        │   │   └── ProductService.cs (5.4KB)   MongoDB sorgu implementasyonu. En büyük servis.
        │   │                                   FindAsync, InsertOneAsync, ReplaceOneAsync,
        │   │                                   DeleteOneAsync + soft delete mantığı.
        │   │
        │   ├── ProductImageServices/
        │   │   ├── IProductImageService.cs (631B)
        │   │   └── ProductImageService.cs (3.7KB) Görsel kaydetme, silme, ProductId'ye göre listeleme.
        │   │
        │   ├── BrandServices/         IBrandService + BrandService (MongoDB CRUD)
        │   ├── CategoryServices/      ICategoryService + CategoryService
        │   ├── CurrencyServices/      ICurrencyService + CurrencyService
        │   ├── MeasurementServices/   IMeasurementService + MeasurementService
        │   ├── ProductDetailServices/ IProductDetailService + ProductDetailService
        │   └── ProductPriceServices/  IProductPriceService + ProductPriceService
        │
        ├── Handlers/
        │   ACIKLAMA: Kısmi CQRS uygulaması — sadece Product entity'si için.
        │   Diğer entity'ler (Brand, Category, vb.) doğrudan Controller→Service gidiyor.
        │   ├── CreateProductCommandHandler.cs (720B)  MediatR IRequestHandler implementasyonu
        │   ├── GetAllProductsQueryHandler.cs (768B)   Ürün listesi sorgu handler'ı
        │   └── UpdateProductCommandHandler.cs (647B)  Güncelleme handler'ı
        │
        ├── Commands/
        │   ├── CreateProductCommand.cs (182B)  IRequest<Unit> uygulayan komut nesnesi
        │   └── UpdateProductCommand.cs (170B)  Güncelleme komutu
        │
        ├── Queries/
        │   └── GetAllProductsQuery.cs (218B)  IRequest<List<Product>> uygulayan sorgu nesnesi
        │
        ├── Dtos/
        │   ACIKLAMA: Catalog servisine özel iç DTO'lar. Store.DtoLayer'dan bağımsız.
        │   Her entity için Create, Update, Result DTO'ları + ErrorDtos (hata yanıt formatı).
        │
        ├── Mapping/
        │   └── GeneralMapping.cs (2.8KB)
        │       ACIKLAMA: AutoMapper profili. Tüm Entity↔DTO dönüşüm kuralları tek dosyada.
        │       CreateMap<Brand, ResultBrandDto>().ReverseMap() gibi tanımlar içerir.
        │
        ├── Settings/
        │   ├── IDatabaseSettings.cs (1.1KB)
        │   │   ACIKLAMA: MongoDB bağlantı ayarları arayüzü. ConnectionString, DatabaseName
        │   │   ve her koleksiyon için koleksiyon adı property'leri tanımlar.
        │   └── DatabaseSettings.cs (1.1KB)
        │       ACIKLAMA: IDatabaseSettings implementasyonu. appsettings.json'dan
        │       IOptions<DatabaseSettings> ile doldurulur.
        │
        ├── Exceptions/
        │   └── BusinessException.cs (198B)
        │       ACIKLAMA: İş kuralı ihlallerini temsil eden özel exception sınıfı.
        │       Exception'dan türer, mesaj alır. ExceptionMiddleware tarafından yakalanır.
        │
        ├── Extensions/
        │   └── ServiceRegistrationExtensions.cs (761B)
        │       ACIKLAMA: Tüm Catalog servis bağımlılıklarını IServiceCollection'a
        │       kaydeden extension metodu. Program.cs'i temiz tutar.
        │
        └── Middlewares/
            └── ExceptionMiddleware.cs (2.2KB)
                ACIKLAMA: Global hata yakalama middleware'i. Tüm yakalanmamış exception'ları
                loglayıp (Serilog) standart JSON hata yanıtına dönüştürür.
                BusinessException → 400 Bad Request, diğerleri → 500 Internal Server Error.
```

---

### 5.2 Discount Servisi — MSSQL + Dapper

**Amaç:** Kupon bazlı indirim yönetimi. Tek tablo, tek entity, basit yapı.  
**Mimari:** Monolitik. CQRS/MediatR yok — doğrudan servis pattern'i.  
**Veritabanı:** MSSQL LocalDB (StoreDiscountDb) — 1 tablo: Coupons.  
**Not:** Hem Dapper hem EF Core kullanıyor (hibrit).

```
Services/
└── Discount/
    └── Store.Discount/
        ├── Program.cs (2.5KB)
        │   ACIKLAMA: JWT doğrulama, CORS, Swagger, Dapper+EF Core kurulumu.
        │   Discount servisi basit olduğu için minimum konfigürasyon yeterli.
        │
        ├── Store.Discount.csproj (1.2KB)
        │   ACIKLAMA: Dapper, EF Core, Microsoft.Data.SqlClient, JWT bağımlılıkları.
        │
        ├── appsettings.json (345B)
        │   ACIKLAMA: MSSQL LocalDB bağlantı dizesi. KRİTİK: Şifre düz metin.
        │
        ├── Context/
        │   └── DapperContext.cs (999B)
        │       ACIKLAMA: Dapper için ham SQL bağlantı yönetimi.
        │       SqlConnection oluşturur, yönetir. IDbConnection döndüren factory.
        │       NOT: EF Core DbContext değil — ham SQL sorguları çalıştırmak için.
        │       SQL Injection riski: Parametreli sorgular (@Code, @Id) kullanılıyor mu kontrol et.
        │
        ├── Entities/
        │   └── Coupon.cs (294B)
        │       ACIKLAMA: Kupon domain sınıfı.
        │       KRİTİK SORUN: BaseEntity'den türememiş!
        │       Alanlar: Id(int-IDENTITY), Code(string), Rate(int), IsActive(bool), ValidDate(DateTime)
        │       Eksik: InsertDate, InsertUserId, UpdateDate, IsDeleted gibi audit alanları yok.
        │       DB'de Code alanı NVARCHAR(MAX) tanımlı — indeks verimsizliği!
        │
        ├── Controllers/
        │   └── DiscountController.cs
        │       ACIKLAMA: Kupon yönetimi endpoint'leri.
        │       GET /api/discounts — tüm kuponlar
        │       GET /api/discounts/{id} — id ile getir
        │       GET /api/discounts/code/{code} — KOD ile getir (WebUI'dan kullanılır)
        │       POST /api/discounts — kupon oluştur
        │       PUT /api/discounts — kupon güncelle
        │       DELETE /api/discounts/{id} — kupon sil
        │
        ├── Services/
        │   ├── IDiscountService.cs (421B)
        │   │   ACIKLAMA: GetAll, GetById, GetByCode, Create, Update, Delete arayüzü.
        │   │   GetByCode özellikle önemli — WebUI'dan kupon kodu girilince bu çağrılır.
        │   │
        │   └── DiscountService.cs (3.1KB)
        │       ACIKLAMA: Dapper ile ham SQL CRUD implementasyonu.
        │       Örnek: "SELECT * FROM Coupons WHERE Code = @Code AND IsActive = 1"
        │       Tarih kontrolü: ValidDate > DateTime.Now kontrolü var mı? Kontrol edilmeli.
        │       İş kuralı eksik: Aynı kupon iki kez kullanılabilir mi?
        │
        ├── Dtos/
        │   ACIKLAMA: Kupon DTO'ları. Create için (Code, Rate, IsActive, ValidDate),
        │   Update için (Id + tüm alanlar), Result için response modeli.
        │
        └── Migrations/
            ACIKLAMA: EF Core ile oluşturulan veritabanı şema migration dosyaları.
            Coupon tablosunun initial migration'ı burada.
```

---

### 5.3 Location Servisi — Clean Architecture

**Amaç:** Coğrafi konum (Ülke→Şehir→İlçe→Mahalle), şirket/şube, depo, bayi, kullanıcı adres yönetimi.  
**Mimari:** Clean Architecture — 4 katman. En olgun ve doğru mimariye sahip servis.  
**Veritabanı:** PostgreSQL (StoreLocationDb) — 11 tablo.

#### 5.3.1 Store.Location.Domain — Katman 1: Saf İş Nesneleri

```
Services/Location/Core/Store.Location.Domain/
├── Store.Location.Domain.csproj (218B)
│   ACIKLAMA: Dış bağımlılık SIFIR. Sadece .NET runtime. Bu Clean Architecture
│   gereği doğrudur — Domain saf iş nesnelerinden oluşur, dış kütüphaneye muhtaç olmaz.
│
└── Entities/
    ├── BaseEntity.cs (2.7KB)
    │   ACIKLAMA: Tüm entity'lerin temel sınıfı. Audit ve soft delete alanları:
    │   - Id (Guid): Primary key
    │   - InsertDate (DateTimeOffset, UTC): Kayıt oluşturma zamanı
    │   - InsertUserId (Guid?): Kaydı oluşturan kullanıcı
    │   - UpdateDate (DateTimeOffset?): Son güncelleme zamanı
    │   - UpdateUserId (Guid?): Güncelleyen kullanıcı
    │   - DeleteDate (DateTimeOffset?): Silinme zamanı (soft delete)
    │   - DeleteUserId (Guid?): Silen kullanıcı
    │   - IsDeleted (bool): Soft delete flag — false=aktif, true=silinmiş
    │   SORUN: Store.Catalog da bu sınıfı kullanıyor. Catalog→Location bağımlılığının nedeni.
    │
    ├── AddressBase.cs (1.9KB)
    │   ACIKLAMA: Soyut adres base sınıfı. BranchAddress ve UserAddress buradan türer.
    │   Ortak adres alanları: NeighborhoodId(FK), AddressDetail, ZipCode,
    │   ContactFullName, PhoneNumber, IdentityNumber (TC No!)
    │   NOT: IdentityNumber düz metin saklanıyor — KVKK ihlali!
    │
    ├── Branch.cs (1.3KB)
    │   ACIKLAMA: Şube entity'si. BranchName, BranchCode (benzersiz şube kodu),
    │   CompanyId (bağlı şirket FK), ContactInfoId (iletişim bilgisi FK).
    │
    ├── BranchAddress.cs (700B)
    │   ACIKLAMA: Şube fiziksel adresi. AddressBase'den türer (tüm adres alanları kalıtılır).
    │   Ek alan: BranchId (hangi şubeye ait).
    │
    ├── City.cs (1.2KB)
    │   ACIKLAMA: Şehir. CityName, PlateCode (81 il plaka kodu), CountryId(FK).
    │
    ├── Company.cs (1.9KB)
    │   ACIKLAMA: Şirket. CompanyName, TaxNumber (vergi no), TaxOffice (vergi dairesi),
    │   InvoiceType (enum: Bireysel/Kurumsal), MainContactId (FK).
    │
    ├── ContactInfo.cs (1.1KB)
    │   ACIKLAMA: İletişim bilgisi hub sınıfı. Email, PhoneNumber, WebSite, Notes.
    │   Hem Şube hem Bayi hem Şirket bu tabloya FK ile bağlanır.
    │   Paylaşımlı iletişim bilgisi için doğru normalizasyon kararı.
    │
    ├── Country.cs (1.1KB)
    │   ACIKLAMA: Ülke. CountryName (Türkiye, vb.), CountryCode (ISO — TR, US, DE).
    │
    ├── Dealer.cs (1.4KB)
    │   ACIKLAMA: Bayi entity'si. CompanyId (bağlı şirket), ContactInfoId,
    │   DiscountRate (bayi iskonto oranı — decimal), CreditLimit (açık hesap limiti),
    │   CurrentBalance (anlık borç/alacak).
    │   SORUN: Finansal alanlar (CreditLimit, CurrentBalance) Location servisinde yanlış yerde.
    │   İleride Customer/B2B servisine taşınmalı.
    │
    ├── District.cs (961B)
    │   ACIKLAMA: İlçe. DistrictName, CityId(FK).
    │
    ├── InvoiceType.cs (469B)
    │   ACIKLAMA: ENUM. Individual(1) = Bireysel fatura, Corporate(2) = Kurumsal fatura.
    │   Company.InvoiceType alanında kullanılır. Fatura kesim türünü belirler.
    │   SORUN: Bu enum Location Domain'de yanlış yerde — Catalog veya Order'da olmalı.
    │
    ├── Neighborhood.cs (1KB)
    │   ACIKLAMA: Mahalle. NeighborhoodName, DistrictId(FK). Adres hiyerarşisinin en alt seviyesi.
    │
    ├── PriceType.cs (823B)
    │   ACIKLAMA: ENUM. Cost(1), Retail(2), Rental(3), Wholesale(4), Wholerental(5).
    │   SORUN: Fiyatlandırma iş mantığı Location Domain'de tamamen yanlış yerde.
    │   Catalog Domain veya ortak bir Shared kütüphanede olmalı.
    │
    ├── UserAddress.cs (1.8KB)
    │   ACIKLAMA: Müşteri adresi. AddressBase'den türer + ek alanlar:
    │   CustomerId (Guid — IdentityServer kullanıcısı), IsDefault (varsayılan adres mi?),
    │   CompanyId (nullable — kurumsal müşteri adresi için).
    │   ÖNEMLİ: AnonymizeData() KVKK metodu burada!
    │   - ContactFullName → "silinmiştir"
    │   - PhoneNumber → maskeleme
    │   - AddressDetail → "silinmiştir"
    │   - IdentityNumber → null
    │   - IsAnonymized → true
    │
    ├── Warehouse.cs (1.5KB)
    │   ACIKLAMA: Depo entity'si. WarehouseName, BranchId(FK — hangi şubeye ait),
    │   Capacity (kapasite), WarehouseCode.
    │   [DÜZELTİLDİ - 05.08.2026] Eski düz metin Address ve City alanları silindi,
    │   yerine normalize edilmiş WarehouseAddress entity'si getirildi. Ve CRUD 
    │   işlemleri (CQRS) Warehouse üzerinden yönetilecek şekilde API'ye eklendi.
    │
    └── WarehouseAddress.cs
        ACIKLAMA: [EKLENDİ - 05.08.2026] Depo fiziksel adresi. AddressBase'den türer
        (tüm adres alanları kalıtılır). Ek alan: WarehouseId.
```

#### 5.3.2 Store.Location.Application — Katman 2: İş Mantığı

```
Services/Location/Core/Store.Location.Application/
├── Store.Location.Application.csproj (4.6KB)
│   ACIKLAMA: MediatR, FluentValidation, AutoMapper bağımlılıkları.
│   Store.Location.Domain'e proje referansı (doğru — aynı servis içinde).
│
├── Features/
│   ├── CQRS/                           SORUN! ESKİ YAPI — TEMİZLENECEK
│   │   ACIKLAMA: Sadece ContactInfo entity'si için yazılmış eski CQRS handler'ları.
│   │   Mediator/ klasörüyle çakışan duplicate kod. Silinmeli.
│   │   ├── Commands/   Eski format CreateContactInfoCommand vb.
│   │   ├── Handlers/
│   │   │   └── ContactInfoHandlers/
│   │   │       ├── CreateContactInfoCommandHandler.cs  DUPLICATE — Mediator'da da var!
│   │   │       └── GetContactInfosQueryHandler.cs      DUPLICATE
│   │   ├── Queries/    Eski format GetContactInfosQuery
│   │   └── Results/    Eski format GetContactInfosQueryResult
│   │
│   ├── CQRS_Yedek/                     SORUN! YEDEK KLASÖR — SİLİNMELİ
│   │   ACIKLAMA: Eski CQRS kodlarının yedek kopyası. Hiçbir aktif kod içermiyor.
│   │   Anlık görüntü olarak tutulmuş olabilir ama kaynak kontrolünde yeri yok.
│   │
│   └── Mediator/                       YENİ VE AKTİF YAPI
│       ACIKLAMA: Tüm servis iş mantığı burada. Her entity için kendi klasörü.
│       │
│       ├── Commands/
│       │   └── ContactInfo2Commands/
│       │       ACIKLAMA: IRequest<Unit> uygulayan komut nesneleri.
│       │       CreateContactInfo2Command (ContactInfoId olmadan yeni kayıt)
│       │       UpdateContactInfo2Command (mevcut kaydı güncelle)
│       │       DeleteContactInfo2Command (id ile sil)
│       │       NOT: "ContactInfo2" adı geçici — "ContactInfo" olarak değiştirilmeli.
│       │       Diğer entity'ler için de benzer Commands klasörleri mevcut.
│       │
│       ├── Handlers/
│       │   └── ContactInfo2Handlers/
│       │       ACIKLAMA: MediatR IRequestHandler<TCommand, TResponse> implementasyonları.
│       │       CreateContactInfo2CommandHandler — repository.AddAsync + SaveAsync
│       │       UpdateContactInfo2CommandHandler — repository.UpdateAsync + SaveAsync
│       │       DeleteContactInfo2CommandHandler — repository.DeleteAsync (soft delete!)
│       │       GetContactInfo2ByIdQueryHandler — repository.GetByIdAsync
│       │       GetContactInfosQueryHandler      — repository.GetAllAsync
│       │       Her handler constructor'a IRepository<ContactInfo> inject edilir.
│       │
│       ├── Queries/     GetByIdQuery ve GetAllQuery nesneleri (her entity için)
│       └── Results/     Handler'ların döndürdüğü DTO benzeri sonuç sınıfları
│
├── Interfaces/
│   ├── IRepository.cs (843B)
│   │   ACIKLAMA: Generic repository arayüzü.
│   │   GetAllAsync() → List<T>
│   │   GetByIdAsync(Guid id) → T
│   │   AddAsync(T entity) → Task
│   │   UpdateAsync(T entity) → Task
│   │   DeleteAsync(Guid id) → Task (soft delete — IsDeleted=true yapar)
│   │   SaveAsync() → Task (Unit of Work)
│   │   NOT: Bu arayüz Order.Application'da da kopyalanmış. Store.Shared'e taşınmalı.
│   │
│   └── IUserContext.cs (265B)
│       ACIKLAMA: JWT'den kullanıcı bilgisi okuma arayüzü.
│       UserId property — handler'lar audit alanlarını doldurmak için kullanır.
│
└── Services/
    └── ServiceRegistration.cs
        ACIKLAMA: Application katmanı DI kayıtları. MediatR, AutoMapper,
        FluentValidation assembly taraması burada yapılır.
```

#### 5.3.3 Store.Location.Persistence — Katman 3: Veri Erişimi

```
Services/Location/Infrastructure/Store.Location.Persistence/
├── Store.Location.Persistence.csproj (1.3KB)
│   ACIKLAMA: EF Core, Npgsql (PostgreSQL driver) bağımlılıkları.
│   Store.Location.Application ve Store.Location.Domain'e referans (doğru).
│
├── Context/
│   └── LocationContext.cs (2.4KB)
│       ACIKLAMA: EF Core DbContext. Tüm PostgreSQL tablolarını tanımlar.
│       11 DbSet: Countries, Cities, Districts, Neighborhoods, Companies,
│       Branches, BranchAddresses, ContactInfos, Dealers, UserAddresses, Warehouses.
│       ÖNEMLI: modelBuilder.Entity<T>().HasQueryFilter(x => !x.IsDeleted)
│       → Global Query Filter aktif! Silinen kayıtlar tüm sorgulardan otomatik filtreler.
│       Bu, soft delete'in şeffaf çalışmasını sağlar — geliştiricinin her sorguda
│       "WHERE IsDeleted = 0" eklemesi gerekmez.
│
├── Repositories/
│   └── Repository.cs (4.4KB)
│       ACIKLAMA: IRepository<T> implementasyonu. Generic CRUD:
│       GetAllAsync: DbSet.ToListAsync() — Global filter otomatik devreye girer
│       GetByIdAsync: FindAsync(id) — silinmiş kaydı döndürmez (filter yüzünden)
│       AddAsync: _context.AddAsync(entity) + audit alanlarını doldurur
│       UpdateAsync: _context.Update(entity) + UpdateDate günceller
│       DeleteAsync: GetByIdAsync + IsDeleted=true + DeleteDate=now (SOFT DELETE!)
│       SaveAsync: _context.SaveChangesAsync()
│
└── Migrations/
    ACIKLAMA: EF Core migration geçmişi. PostgreSQL'de tablo oluşturma, değiştirme,
    index ekleme operasyonlarının kod olarak tutulduğu yer.
    CI/CD pipeline'da `dotnet ef database update` ile otomatik uygulanır.
```

#### 5.3.4 Store.Location.WebApi — Katman 4: HTTP Sunum

```
Services/Location/Presentation/Store.Location.WebApi/
├── Program.cs (10.2KB)
│   ACIKLAMA: En kapsamlı Program.cs. JWT Bearer token doğrulama, CORS politikaları,
│   Swagger/OpenAPI (Bearer token ile), MediatR, FluentValidation, Serilog loglama,
│   Policy tabanlı yetkilendirme ("LocationFullPermission") kurulumları.
│   10.2KB boyutu bu kapsamlı konfigürasyondan kaynaklanıyor.
│
├── appsettings.json (349B)
│   ACIKLAMA: PostgreSQL bağlantı dizesi + Serilog yapılandırması. Şifre düz metin!
│
├── Controllers/ (12 dosya)
│   ACIKLAMA: Her controller [ApiController], [Route("api/[controller]")],
│   [Authorize] attribute'larıyla donatılmış. Sadece mediator.Send() çağrısı yapar.
│   │
│   ├── CountriesController.cs (4.4KB)  GET/POST/PUT/DELETE /api/countries
│   ├── CityController.cs (3.9KB)       GET/POST/PUT/DELETE /api/cities
│   ├── DistrictController.cs (4.3KB)   GET/POST/PUT/DELETE /api/districts
│   ├── NeighborhoodController.cs (4.6KB) GET/POST/PUT/DELETE /api/neighborhoods
│   ├── CompanyController.cs (4.2KB)    GET/POST/PUT/DELETE /api/companies
│   ├── BranchController.cs (4.1KB)     GET/POST/PUT/DELETE /api/branches
│   ├── BranchAddressController.cs (4KB) GET/POST/PUT/DELETE /api/branchaddresses
│   │
│   ├── ContactInfo2Controller.cs (2.2KB)
│   │   ACIKLAMA: KALACAK OLAN controller. /api/contactinfo2 endpoint'i.
│   │   NOT: URL "/contactinfos" olarak değiştirilmeli (isimlendirme tutarlılığı).
│   │
│   ├── ContactInfoController.cs (3.9KB)
│   │   ACIKLAMA: SİLİNMELİ! Eski CQRS handler'larını kullanan duplicate controller.
│   │   ContactInfo2Controller ile AYNI tabloyu yönetiyor. Çakışan endpoint'ler var.
│   │
│   ├── DealerController.cs (4.1KB)     GET/POST/PUT/DELETE /api/dealers
│   ├── UserAddressController.cs (4.4KB) GET/POST/PUT/DELETE /api/useraddresses
│   │   ACIKLAMA: AnonymizeData endpoint'i de burada.
│   │   POST /api/useraddresses/anonymize/{id} → KVKK silme talebi
│   │
│   └── WarehouseController.cs (4.3KB)  GET/POST/PUT/DELETE /api/warehouses
│
└── Services/
    └── UserContext.cs
        ACIKLAMA: IUserContext implementasyonu. IHttpContextAccessor inject edilir.
        HttpContext.User.Claims'den "sub" (subject) claim'ini okuyarak UserId döndürür.
        Her MediatR handler, audit alanlarını doldurmak için bunu kullanır.
```

---

### 5.4 Order Servisi — Clean Architecture

**Amaç:** Sipariş yaratma, listeleme ve yönetimi.  
**Mimari:** Clean Architecture — 4 katman. Hem CQRS hem Mediator pattern paralel kullanılmış.  
**Veritabanı:** MSSQL LocalDB (StoreOrderDb) — 3 tablo.  
**Kritik Tasarım:** Sipariş adresi Location'dan bağımsız snapshot olarak saklanır.

#### 5.4.1 Store.Order.Domain — Katman 1: İş Nesneleri

```
Services/Order/Core/Store.Order.Domain/
├── Store.Order.Domain.csproj (218B)
│   ACIKLAMA: Dış bağımlılık SIFIR. Doğru Clean Architecture tasarımı.
│
├── Entities/                               SORUN! BOŞ KLASÖR (kullanılmıyor)
│   └── BaseEntity.cs (2.4KB)
│       ACIKLAMA: Order servisine özel BaseEntity. Location.Domain.BaseEntity'den
│       kopyalanmış (aynı alanlar). Bu doğru bir yaklaşım — Catalog'un aksine
│       Order kendi BaseEntity'sini yazmış, Location'a bağımlı değil.
│       SORUN: Entities/ klasörü boş, entity sınıfları Entitties/ (çift t!) içinde.
│       Bu dosya Entitties/ klasörüne taşınmalı veya isim düzeltmesi yapılmalı.
│
└── Entitties/                              KRİTİK YAZIM HATASI — "Entitties" (çift t)
    ACIKLAMA: Gerçek entity sınıfları BURADADIR. Entities/ boş, Entitties/ dolu.
    Namespace: Store.Order.Domain.Entitties (yanlış yazım namespace'e de yansımış).
    │
    ├── Address.cs (766B)
    │   ACIKLAMA: Sipariş adresi SNAPSHOT sınıfı.
    │   Alanlar: FullAddress (tam adres metni), City (şehir adı), District (ilçe adı),
    │   ZipCode, AdditionalInfo.
    │   ÖNEMLI TASARIM KARARI: Location servisine FOREIGN KEY YOK!
    │   Sipariş oluşturulduğu andaki adres buraya kopyalanır.
    │   Kullanıcı sonradan adresini değiştirse bile sipariş adresi bozulmaz.
    │   Mikroservis veri tutarlılığı açısından mükemmel karar.
    │
    ├── OrderDetail.cs (595B)
    │   ACIKLAMA: Sipariş kalemleri. OrderId (FK → Ordering), 
    │   ProductId (STRING — MongoDB'deki ürün ID'si, cross-service FK değil!),
    │   ProductName (o anki ürün adı — snapshot), ProductAmount (miktar),
    │   ProductPrice (o anki fiyat — snapshot), TotalPrice (ProductAmount × ProductPrice).
    │   NOT: ProductId string, çünkü Catalog MongoDB string _id kullanıyor.
    │
    └── Ordering.cs (528B)
        ACIKLAMA: Sipariş master. UserId (IdentityServer kullanıcı ID'si — string!),
        OrderDate, TotalPrice, AddressId (FK → Address snapshot),
        OrderStatus (enum? kontrol edilmeli), [OrderDetails navigation property].
```

#### 5.4.2 Store.Order.Application — Katman 2: İş Mantığı

```
Services/Order/Core/Store.Order.Application/
├── Store.Order.Application.csproj (631B)
│   ACIKLAMA: MediatR bağımlılığı. Store.Order.Domain'e referans.
│
├── Features/
│   ├── CQRS/                           Address ve OrderDetail için
│   │   ACIKLAMA: Adres ve sipariş kalemi yönetimi için kullanılan CQRS yapısı.
│   │   ├── Commands/
│   │   │   ├── AddressCommands/
│   │   │   │   CreateAddressCommand (FullAddress, City, District, ZipCode)
│   │   │   │   UpdateAddressCommand (Id + güncelleme alanları)
│   │   │   └── OrderDetailCommands/
│   │   │       CreateOrderDetailCommand (OrderId, ProductId, Amount, Price)
│   │   │       UpdateOrderDetailCommand
│   │   ├── Handlers/
│   │   │   ├── AddressHandlers/       CreateAddressCommandHandler, GetAddressByIdQueryHandler
│   │   │   └── OrderDetailHandlers/   CRUD handler'ları (OrderDetail için)
│   │   ├── Queries/                   GetByIdQuery ve GetAllQuery nesneleri
│   │   └── Results/                   Handler dönüş DTO'ları
│   │
│   └── Mediator/                      Ana Sipariş (Ordering) için
│       ACIKLAMA: En kritik iş akışı — sipariş oluşturma/yönetimi burada.
│       ├── Commands/
│       │   └── OrderingCommands/
│       │       CreateOrderingCommand — UserId, TotalPrice, AddressId, OrderDetails listesi
│       │       UpdateOrderingCommand — durum güncellemesi vb.
│       │       DeleteOrderingCommand — siparişi iptal et (soft delete)
│       ├── Handlers/
│       │   └── OrderingHandlers/
│       │       CreateOrderingCommandHandler — Yeni sipariş kaydeder, AddressId atar
│       │       GetAllOrderingsQueryHandler  — Tüm siparişleri listeler
│       │       GetOrderingByIdQueryHandler  — Id ile sipariş getirir
│       │       DeleteOrderingCommandHandler — Soft delete uygular
│       ├── Queries/    GetAllOrderingsQuery, GetOrderingByIdQuery
│       └── Results/    GetAllOrderingsQueryResult, GetOrderingByIdQueryResult
│
├── Interfaces/
│   ├── IRepository.cs (708B)
│   │   ACIKLAMA: Location.Application.IRepository ile aynı arayüz — kopyalanmış.
│   │   Store.Shared oluşturulduğunda burası kaldırılmalı.
│   └── IUserContext.cs (262B)
│       ACIKLAMA: Location.Application.IUserContext ile aynı arayüz — kopyalanmış.
│
└── Services/
    └── ServiceRegistration.cs  Application DI kayıtları
```

#### 5.4.3 Store.Order.Persistance — Katman 3: Veri Erişimi

```
Services/Order/Core/Infrastructure/Store.Order.Persistance/
ACIKLAMA: "Persistance" yazım hatalı — doğrusu "Persistence"

├── Store.Order.Persistance.csproj (1.2KB)
│   ACIKLAMA: EF Core, Microsoft.Data.SqlClient (MSSQL driver) bağımlılıkları.
│
├── Context/
│   └── OrderContext.cs (1.7KB)
│       ACIKLAMA: EF Core DbContext. MSSQL bağlantısı.
│       3 DbSet: Orderings, OrdersDetails, Addresses.
│       using Store.Order.Domain.Entities;   ← BOŞ klasör kullanılıyor!
│       using Store.Order.Domain.Entitties;  ← Gerçek sınıflar burada
│       Bu iki using aynı anda var — konfüzyon kaynağı!
│       Global Query Filter: .HasQueryFilter(x => !x.IsDeleted) aktif.
│
└── Repositories/
    └── Repository.cs (3.4KB)
        ACIKLAMA: IRepository<T> implementasyonu.
        Location.Persistence.Repository ile aynı yapı — CRUD + soft delete.
        GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync (soft!), SaveAsync.
```

#### 5.4.4 Store.Order.WebApi — Katman 4: HTTP Sunum

```
Services/Order/Presentation/Store.Order.WebApi/
├── Program.cs (3.9KB)
│   ACIKLAMA: JWT, Swagger, CORS, MediatR, ServiceRegistration kurulumu.
│   Location.WebApi kadar kapsamlı değil — daha basit yapı.
│
├── appsettings.json (335B)
│   ACIKLAMA: MSSQL LocalDB bağlantı dizesi. Şifre düz metin!
│
├── Controllers/
│   ├── AdressesController.cs (3KB)
│   │   ACIKLAMA: KRİTİK YAZIM HATASI: "AdressesController" (tek d, tek s).
│   │   Doğrusu: "AddressesController". Route da muhtemelen /api/adresses.
│   │   Adres CRUD endpoint'leri — CQRS pattern kullanıyor.
│   │
│   ├── OrderDetailsController.cs (3.2KB)
│   │   ACIKLAMA: Sipariş kalemleri CRUD endpoint'leri. CQRS pattern kullanıyor.
│   │   GET /api/orderdetails, POST, PUT, DELETE.
│   │
│   ├── OrderingsController.cs (1.8KB)
│   │   ACIKLAMA: Ana sipariş endpoint'leri. Mediator pattern kullanıyor.
│   │   Boyutu (1.8KB) küçük — sadece GET (tümü ve id ile) ve POST var gibi görünüyor.
│   │   PUT ve DELETE eksik mi? Kontrol edilmeli.
│   │
│   └── DeleteOrderingCommand.cs (202B)
│       ACIKLAMA: KRİTİK YANLIŞ YER! Bu bir Command sınıfı, Controller klasöründe.
│       IRequest<Unit> uygulayan MediatR command nesnesi olması gerekiyor.
│       Application/Features/Mediator/Commands/OrderingCommands/ altına taşınmalı!
│       Muhtemelen aceleyle buraya bırakılmış.
│
└── Services/
    └── UserContext.cs (1.3KB)
        ACIKLAMA: IUserContext implementasyonu. Location.WebApi.UserContext ile aynı yapı.
        JWT'den UserId okur. Store.Shared'e taşınması gereken duplicate kod.
```

---

## 6. Store.DtoLayer/ — Paylaşımlı DTO Kütüphanesi

**Amaç:** Servisler arası ve WebUI-backend arası veri taşıma nesnelerini merkezi yönetir.  
**Bağımlılık:** Hiçbir servise bağımlı değil — diğer projeler buna bağımlıdır.  
**Uyarı:** Bu kütüphane değişirse tüm bağımlı projeler yeniden derlenmeli.

```
Store.DtoLayer/
├── Store.DtoLayer.csproj (407B)
│   ACIKLAMA: Sıfır dış bağımlılık. Sadece .NET runtime. Doğru tasarım.
│
├── CatalogDtos/
│   ├── BrandDtos/
│   │   ├── CreateBrandDto.cs     Marka oluşturma isteği (BrandName, ImageUrl)
│   │   ├── UpdateBrandDto.cs     Marka güncelleme (Id + güncellenecek alanlar)
│   │   ├── GetByIdBrandDto.cs    Tek marka sorgu yanıtı
│   │   └── ResultBrandDto.cs     Liste/detay görünüm için yanıt modeli
│   │
│   ├── CategoryDtos/
│   │   ├── CreateCategoryDto.cs   CategoryName, ParentId (nullable — kök ise null)
│   │   ├── UpdateCategoryDto.cs
│   │   └── ResultCategoryDto.cs  CategoryId, CategoryName, ParentId, SubCategories?
│   │
│   ├── CurrencyDtos/             CurrencyCode, CurrencyName, Symbol alanları
│   ├── InventoryDtos/            GELECEK: Envanter servisi için ayrılmış yer-tutucu.
│   │                             DTO'lar hazır ama implementasyonu henüz yok.
│   ├── MeasurementDtos/          MeasurementName, MeasurementUnit alanları
│   │
│   ├── ProductDetailDtos/        Teknik detay DTO'ları (boyutlar, ağırlık, renk vb.)
│   │
│   ├── ProductDtos/
│   │   ├── CreateProductDto.cs      Ürün oluşturma (tüm zorunlu alanlar)
│   │   ├── CreateProductCommandDto.cs  MediatR CreateProductCommand için ayrı DTO
│   │   │                               (Command ve DTO ayrımı yapılmış)
│   │   ├── GetAllProductQueryDto.cs    Listeleme sorgu parametreleri (filtre, sayfalama?)
│   │   ├── UpdateProductDto.cs         Güncelleme DTO'su
│   │   └── ResultProductDto.cs         Ürün yanıt modeli (WebUI'a giden)
│   │
│   ├── ProductImageDtos/         ImageUrl, DisplayOrder, IsMainImage alanları
│   └── ProductPriceDtos/         PriceType, Amount, DiscountedAmount, StartDate, EndDate
│
└── LocationDtos/
    ├── BranchAddressDtos/    Şube adres DTO'ları (Create, Update, Result)
    ├── BranchDtos/           BranchName, BranchCode, CompanyId alanları
    ├── CityDtos/             CityName, PlateCode, CountryId alanları
    ├── CompanyDtos/          CompanyName, TaxNumber, InvoiceType alanları
    ├── ContactInfoDtos/      Email, PhoneNumber, WebSite alanları
    ├── CountryDtos/          CountryName, CountryCode alanları
    ├── DealerDtos/           DiscountRate, CreditLimit, CurrentBalance alanları
    ├── DistrictDtos/         DistrictName, CityId alanları
    │
    ├── InvoiceTypeDtos/      SORUN! BOŞ KLASÖR.
    │                         InvoiceType enum değerleri için DTO yazılmamış.
    │                         Dropdown listeleri için API endpoint açılacaksa gerekli.
    │
    ├── NeighborhoodDtos/     NeighborhoodName, DistrictId alanları
    │
    ├── PriceTypeDtos/        SORUN! BOŞ KLASÖR.
    │                         PriceType enum değerleri için DTO yazılmamış.
    │                         Fiyat tipi seçimi UI'a sunulacaksa gerekli.
    │
    ├── UserAddressDtos/      ContactFullName, PhoneNumber, AddressDetail, ZipCode,
    │                         NeighborhoodId, CustomerId, IsDefault, IsAnonymized alanları
    └── WarehouseDtos/        WarehouseName, BranchId, Capacity, WarehouseCode alanları
```

---

## 7. Altyapı Dosyası: docker-compose.yml

```
Services/
└── docker-compose.yml (991B)
    ACIKLAMA: Docker Compose altyapı tanım dosyası.
    İçerik: MongoDB, PostgreSQL, MSSQL container tanımları.
    
    SORUN: Sadece veritabanı container'ları var. Uygulama servisleri (Catalog API,
    Location API, Order API, Discount API, IdentityServer, WebUI) için
    container tanımı YOK. Dockerfile'lar yazılmamış.
    
    Tam Docker ortamı için:
    1. Her servis için Dockerfile yazılmalı
    2. docker-compose.yml güncellenmeli (servisler + healthcheck + network)
    3. Environment variables ile appsettings.json'daki secrets dışarıdan inject edilmeli
```

---

## 8. Mimari Bağımlılık Haritası

```
MEVCUT (HATALI) BAĞIMLILIK:
══════════════════════════════════════════
Store.Catalog ──── ProjectReference ────► Store.Location.Domain
                                          (BaseEntity, PriceType...)
                   Bu ilişki Mikroservis prensibine AYKIRIDIR!

OLMASI GEREKEN:
══════════════════════════════════════════
Store.Catalog ─────► Store.Shared (YENİ OLUŞTURULACAK)
Store.Location ────► Store.Shared
Store.Order ───────► Store.Shared
Store.Discount ────► Store.Shared
Store.DtoLayer ────► (bağımsız kalabilir)

Store.Shared içeriği: BaseEntity, IRepository, IUserContext,
                      ortak utilities, common DTOs

DOĞRU BAĞIMLILIKLAR (mevcut):
══════════════════════════════════════════
Location WebApi ──► Location Application ──► Location Domain
Location WebApi ──► Location Persistence ──► Location Domain
Location Persistence ──► Location Application

Order WebApi ──► Order Application ──► Order Domain
Order WebApi ──► Order Persistance ──► Order Domain

Store.WebUI ──► Store.DtoLayer
Store.WebUI ──HTTP──► Catalog API
Store.WebUI ──HTTP──► Location API
Store.WebUI ──HTTP──► Order API
Store.WebUI ──HTTP──► Discount API
Store.WebUI ──HTTP──► IdentityServer (OAuth flow)
```

---

## 9. Dosya Sağlığı ve Teknik Borç Özeti

### Kritik Düzeltmeler

| # | Dosya / Konum | Sorun Türü | Eylem |
|:-:|:-------------|:-----------|:------|
| 1 | `Store.Catalog/Program.cs` | Güvenlik | SSL bypass → #if DEBUG al |
| 2 | `Store.WebUI/Program.cs` | Güvenlik | SSL bypass → #if DEBUG al |
| 3 | `IdentityServer/tempkey.jwk` | Güvenlik | .gitignore ekle, X.509 kullan |
| 4 | `Store.Catalog.csproj` | Mimari | Location.Domain bağımlılığını kopar |
| 5 | `Entities/Product.cs` | Mimari | ID Shadowing — CatalogBaseEntity yaz |
| 6 | `Store.Order.Domain/Entitties/` | Yazım | Entities/ olarak düzelt |
| 7 | `Controllers/AdressesController.cs` | Yazım | AddressesController olarak düzelt |
| 8 | `Controllers/DeleteOrderingCommand.cs` | Mimari | Application/Features'a taşı |
| 9 | `Location.WebApi/ContactInfoController.cs` | Duplicate | Sil |
| 10 | `Location.Domain/Entities/Warehouse.cs` | DBA | Adres alanlarını normalize et |

### Yüksek Öncelikli İyileştirmeler

| # | Dosya / Konum | Sorun | Öneri |
|:-:|:-------------|:------|:------|
| 1 | Tüm appsettings.json | Güvenlik | dotnet user-secrets veya Vault |
| 2 | `Location.Domain/Entities/Dealer.cs` | Mimari | Customer/B2B servisine taşı |
| 3 | `Location.Domain/Entities/PriceType.cs` | Mimari | Catalog/Shared'e taşı |
| 4 | `Location.Application/Features/CQRS/` | Temizlik | Silinmeli |
| 5 | `Location.Application/Features/CQRS_Yedek/` | Temizlik | Silinmeli |
| 6 | `Store.Order.Persistance` (proje adı) | Yazım | Persistence olarak düzelt |
| 7 | `Discount/Entities/Coupon.cs` | DBA | BaseEntity'den türet |
| 8 | `IRepository.cs` (her serviste kopya) | DRY | Store.Shared'e taşı |

### Orta Öncelikli İyileştirmeler

| # | Dosya / Konum | Sorun | Öneri |
|:-:|:-------------|:------|:------|
| 1 | `LocationDtos/InvoiceTypeDtos/` | Boş | DTO'ları yaz veya sil |
| 2 | `LocationDtos/PriceTypeDtos/` | Boş | DTO'ları yaz veya sil |
| 3 | `Areas/Admin/DenemeEkraniController.cs` | Test kodu | Sil |
| 4 | `dart_test.log` (kök) | Yabancı dosya | Sil, .gitignore ekle |
| 5 | `Catalog/Handlers/` | Eksik | Diğer entity'lere CQRS uygula |
| 6 | `Services/docker-compose.yml` | Eksik | Servis Dockerfile'ları ekle |
| 7 | `ContactInfo2Controller.cs` URL | İsimlendirme | /api/contactinfos olarak değiştir |

### Genel Sağlık Göstergeleri

| Metrik | Değer | Durum |
|:-------|:-----:|:-----:|
| Toplam Proje Sayısı | 9 | Normal |
| Clean Architecture Uygulayan | 2/4 servis | Orta |
| Test Projesi Sayısı | 0 | KRİTİK EKSİK |
| Kritik Güvenlik Açığı | 3 | ACİL |
| Yazım Hatası (klasör/dosya) | 4+ | Yüksek |
| Duplicate Kod Bloğu | 4+ | Orta |
| Temizlenecek Klasör | 3 | Orta |
| Boş Klasör (faal) | 4 | Düşük |

---

> **Hazırlayan:** 6 Şapkalı Uzman Ekibi (Antigravity)  
> **Tarih:** 31 Temmuz 2026  
> **Baz:** ProjeAnalizi_2026.07.30.md + Canlı Kaynak Kod İncelemesi  
> **Sonraki Adım:** Kritik güvenlik açıklarının kapatılması için ProjeAnalizi_2026.07.30.md Bölüm 8 yol haritasını takip edin.


### G�ncelleme (2026.08.05): Dealer B2B Servisi Ay�rma
1. Dealer entitysindeki (Store.Location) finansal alanlar (TaxNumber, TaxOffice, DiscountRate, CreditLimit, CurrentBalance) DDD kurallar� gere�i temizlendi.
2. Yeni Store.B2B mikroservisi olu�turuldu.
3. DealerFinancial entity'si ile StoreB2BDb aya�a kald�r�larak bu alanlar ilgili servise (Bounded Context) ta��nd�.
