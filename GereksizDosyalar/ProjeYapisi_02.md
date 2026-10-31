# Store Projesi — Mimari ve Organizasyonel Değerlendirme Raporu

> **Belge:** `ProjeYapisi_02.md`
> **Proje:** `Store.sln` — Mikroservis E-Ticaret & Kiralama Platformu
> **Tarih:** Temmuz 2026
> **Hazırlayan:** Kıdemli Proje Yöneticisi, Kıdemli Yazılım Mühendisi & Kıdemli Test Mühendisi (Ortak Konsensüs Raporu)
> **Kapsam:** Çözüm mimarisi, katman yapıları, teknolojik bağımlılıklar, operasyonel süreçler, test edilebilirlik ve risk analizleri.

---

## İÇİNDEKİLER

1. [Giriş ve Yönetici Özeti](#1-giriş-ve-yönetici-özeti)
2. [Kıdemli Yazılım Mühendisi ve Mimar Değerlendirmesi](#2-kıdemli-yazılım-mühendisi-ve-mimar-değerlendirmesi)
   - [2.1 Çözüm (Solution) ve Katman Mimarisi](#21-çözüm-solution-ve-katman-mimarisi)
   - [2.2 Clean Architecture ve CQRS/MediatR Uygulaması](#22-clean-architecture-ve-cqrspaper-uygulaması)
   - [2.3 Kimlik Doğrulama, Yetkilendirme ve API Güvenliği](#23-kimlik-doğrulama-yetkilendirme-ve-api-güvenliği)
   - [2.4 Servisler Arası Entegrasyon ve Haberleşme Desenleri](#24-servisler-arası-entegrasyon-ve-haberleşme-desenleri)
3. [Kıdemli Proje Yöneticisi Değerlendirmesi](#3-kıdemli-proje-yöneticisi-değerlendirmesi)
   - [3.1 Takım Yapıları ve Paralel Geliştirme Kabiliyeti](#31-takım-yapıları-ve-paralel-geliştirme-kabiliyeti)
   - [3.2 Polyglot Persistence Yaklaşımının Maliyet ve Bakım Analizi](#32-polyglot-persistence-yaklaşımının-maliyet-ve-bakım-analizi)
   - [3.3 Teslimat (Time-to-Market) ve Canlıya Geçiş Riskleri](#33-teslimat-time-to-market-ve-canlıya-geçiş-riskleri)
4. [Kıdemli Test Mühendisi Değerlendirmesi](#4-kıdemli-test-mühendisi-değerlendirmesi)
   - [4.1 Test Edilebilirlik Derecesi (Testability Score)](#41-test-edilebilirlik-derecesi-testability-score)
   - [4.2 Test Ortamları ve Dış Bağımlılık Yönetimi](#42-test-ortamları-ve-dış-bağımlılık-yönetimi)
   - [4.3 Entegrasyon ve E2E Test Senaryoları](#43-entegrasyon-ve-e2e-test-senaryoları)
5. [Aksiyon Planı ve Yol Haritası](#5-aksiyon-planı-ve-yol-haritası)

---

## 1. GİRİŞ VE YÖNETİCİ ÖZETİ

Bu rapor, **Store.sln** mikroservis projesinin mevcut kod tabanını, bağımlılık şemalarını ve operasyonel süreçlerini teknik ve yönetsel açılardan değerlendirmektedir. Proje; **ASP.NET Core 8.0**, **MongoDB**, **PostgreSQL**, **MS SQL Server** ve **IdentityServer4** bileşenlerinden oluşan, kiralama ve satış odaklı bir e-ticaret platformudur.

Genel bulgular, platformun servis sınırlarının (bounded contexts) doğru çizildiğini ve modern mimari kalıpların (CQRS, Clean Architecture, Database-per-Service) uygulandığını göstermektedir. Bununla birlikte, servislerin birbirine sıkı sıkıya bağlı HTTP client'lar üzerinden konuşması, veri tutarlılığı riskleri (lack of eventual consistency) ve CI/CD süreçlerindeki yerel yapılandırma bağımlılıkları platformun büyümesi önündeki en büyük teknik borçlar (technical debt) olarak tespit edilmiştir.

---

## 2. KIDEMLİ YAZILIM MÜHENDİSİ VE MİMAR DEĞERLENDİRMESİ

### 2.1 Çözüm (Solution) ve Katman Mimarisi

Çözüm yapısı (`Store.slnx`), servislerin mimari olgunluğuna göre iki farklı şablonda tasarlandığını göstermektedir:

1. **Clean Architecture (Temiz Mimari) Kullanan Servisler (Location, Order):**
   Bu servisler gevşek bağlılık ve iş kurallarının (domain logic) soyutlanması hedeflenerek 4 ana katmana bölünmüştür:
   - **Domain:** Veritabanından bağımsız iş varlıkları (Entity) ve çekirdek enum'ları barındırır.
   - **Application:** CQRS Handler'ları, MediatR nesnelerini, validasyonları ve servis arayüzlerini (interfaces) içerir.
   - **Persistence (Infrastructure):** Veritabanı DbContext'ini, Repositories implementasyonlarını ve EF Core Migration'larını içerir.
   - **WebApi (Presentation):** Controller'ları ve API endpoints sunumunu yapar.
2. **Monolitik Servis Tasarımı Kullanan Servisler (Catalog, Discount, IdentityServer):**
   Tüm katmanların tek bir Web API projesinde toplandığı yapıdır. Katalog (Catalog) servisinde iş kuralları çok karmaşık olmadığından bu yaklaşım kabul edilebilir olsa da, kod tabanı genişledikçe bağımlılık yönetimi zorlaşacaktır.

> [!WARNING]
> **Catalog Servisinde Mimari Tutarsızlık:**
> [`Store.Catalog.csproj`](file:///c:/User/Projects/Store/Services/Catalog/Store.Catalog/Store.Catalog.csproj) dosyası incelendiğinde, projenin [`Store.Location.Domain`](file:///c:/User/Projects/Store/Services/Location/Core/Store.Location.Domain/Store.Location.Domain.csproj) projesine doğrudan bağımlı olduğu görülmüştür. Bu durum mikroservislerin bağımsızlığı prensibini zedelemektedir. Catalog servisi, `BaseEntity` gibi ortak sınıfları Location servisinden almak yerine ortak bir `Core.Shared` kütüphanesinden almalı veya kendi içinde tanımlamalıdır.

---

### 2.2 Clean Architecture ve CQRS/MediatR Uygulaması

Clean Architecture uygulanan servislerde **CQRS (Command Query Responsibility Segregation)** deseni, MediatR kütüphanesi (v11.1) aracılığıyla uygulanmıştır.
- Sorgular (Queries) ve Değişiklikler (Commands) `Features` klasörü altında iş mantığına göre ayrıştırılmıştır.
- Bu yaklaşım Controller sınıfını sadece istek yönlendirici (router) konumuna getirerek inceltmiştir.

**Kod Kalitesi Analizi:**
- **Validasyon:** İsteklerin (DTO) doğrulanması için fluent validation yapısı (`FluentValidation.AspNetCore` v11.3) başarıyla entegre edilmiştir. Bu, iş kuralları tetiklenmeden önce geçersiz isteklerin filtrelenmesini sağlar.
- **Loglama:** Serilog entegrasyonu (`Serilog.AspNetCore` v10.0) yapılarak günlük loglar konsola ve dosyaya yazdırılmaktadır. Ancak logların merkezi bir log sunucusuna (Elasticsearch, Seq vb.) yönlendirilmesi eksiktir.

---

### 2.3 Kimlik Doğrulama, Yetkilendirme ve API Güvenliği

Sistem genelinde güvenli yetkilendirme akışları [`Store.IdentityServer`](file:///c:/User/Projects/Store/IdentityServer/Store.IdentityServer) projesi üzerinden **IdentityServer4** ile yönetilmektedir.

**Client ve İzin Yapılandırması ([`Config.cs`](file:///c:/User/Projects/Store/IdentityServer/Store.IdentityServer/Config.cs)):**
- **StoreVisitorId:** Yalnızca `LocationFullPermission` yetkisine sahiptir. Giriş yapmamış kullanıcıların konum/şube bilgilerini alması için tasarlanmıştır.
- **StoreManagerId:** `LocationFullPermission` ve `DiscountFullPermission` yetkilerine sahiptir.
- **StoreAdminId:** `CatalogFullPermission`, `OrderFullPermission` dahil olmak üzere tüm API yetkilerine ve standart OpenID claim'lerine (Email, Profile, Phone) erişim hakkına sahiptir.

> [!IMPORTANT]
> **SSL Bypass Güvenlik Riski (Geliştirme Ortamı):**
> [`Program.cs (Catalog)`](file:///c:/User/Projects/Store/Services/Catalog/Store.Catalog/Program.cs#L38-L41) ve [`Program.cs (WebUI)`](file:///c:/User/Projects/Store/FrontEnds/Store.WebUI/Program.cs#L18-L22) dosyalarında, SSL sertifikası doğrulamasını tamamen devredışı bırakan `ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true` kodu yer almaktadır.
> Bu ayar yerel geliştirme için kolaylık sağlasa da, canlı ortama (Production) sızma durumunda **Man-in-the-Middle (MitM)** saldırılarına karşı sistemi tamamen savunmasız bırakacaktır. Bu bypass işlemi sadece `#if DEBUG` blokları arasına alınmalı veya canlı profilinde kaldırılmalıdır.

---

### 2.4 Servisler Arası Entegrasyon ve Haberleşme Desenleri

Mikroservisler arası entegrasyonda en kritik bulgu, servislerin birbirine **HTTP (REST API)** üzerinden doğrudan ve senkron olarak bağlanmasıdır.
- [`Store.WebUI`](file:///c:/User/Projects/Store/FrontEnds/Store.WebUI/Program.cs) projesinde HttpClient'lar doğrudan mikroservis URL'lerini hedef almaktadır.
- Servisler arasında asenkron haberleşme (Message Broker - RabbitMQ/Kafka) bulunmamaktadır.
- Sipariş (`Store.Order`) oluştuğunda kiralama veya stok bilgisini (`Store.Catalog`) güncellemek için senkron istekler atılması, ağ gecikmelerini artıracak ve servislerden biri çöktüğünde sipariş sürecinin de tıkanmasına (Cascading Failure) yol açacaktır.

---

## 3. KIDEMLİ PROJE YÖNETİCİSİ DEĞERLENDİRMESİ

### 3.1 Takım Yapıları ve Paralel Geliştirme Kabiliyeti

Projenin mikroservis yapısı, organizasyonel olarak **Feature Teams (Özellik Takımları)** kurulmasına elverişlidir.
- **Katalog Takımı:** Sadece MongoDB ve Catalog servisinden sorumlu olup, veri şeması değişikliği yaparken diğer ekiplerden onay almak zorunda değildir.
- **Sipariş & Ödeme Takımı:** MSSQL ilişkisel veri yapısı ve sipariş akışlarına odaklanabilir.
- **Arayüz Takımı:** Backend servislerinden bağımsız olarak WebUI projesini geliştirebilir.

Bu yapı paralel geliştirme hızını (velocity) maksimize eder. Ancak, servisler arası paylaşılan kütüphanelerin eksikliği (örneğin DTO'ların her projede manuel kopyalanması veya DtoLayer bağımlılığı) ekipler arası bağımlılığı (tight coupling) artırmaktadır.

---

### 3.2 Polyglot Persistence Yaklaşımının Maliyet ve Bakım Analizi

Projede MongoDB, PostgreSQL ve SQL Server'ın aynı anda kullanılması teknolojik esneklik sağlasa da, proje yönetimi açısından ciddi riskler barındırmaktadır:

```
                  +-----------------------------------+
                  |      PROJE OPERASYON MALİYETİ     |
                  +-----------------------------------+
                                    |
            +-----------------------+-----------------------+
            |                                               |
  [İnsan Kaynağı Riski]                           [Altyapı & DevOps Maliyeti]
  - Geliştiricinin 3 farklı DBMS                  - 3 farklı veritabanı yedeği
    ve ORM bilmesi gerekir.                         (Mongo, PgSQL, MSSQL).
  - İşe alım ve oryantasyon                       - Lisanslama, izleme (monitoring)
    süreleri uzar.                                  ve bakım maliyetleri katlanır.
```

- Geliştirme aşamasında MSSQL LocalDB kullanılması bulut geçişlerinde (Azure/AWS/Linux Container) uyumsuzluk yaratacaktır.

---

### 3.3 Teslimat (Time-to-Market) ve Canlıya Geçiş Riskleri

1. **Bağlantı Bilgisi Yönetimi:** Veritabanı ve IdentityServer bağlantıları `appsettings.json` dosyalarında düz metin olarak tutulmaktadır. Canlıya geçiş öncesinde bu yapılandırmaların dış kaynaklardan (Environment Variables veya Secrets Manager) beslenmesi gerekmektedir.
2. **Kademeli Canlıya Geçiş (Blue-Green / Canary):** Servislerin bağımsız deployment'ı mümkündür ancak veritabanı şema değişiklikleri (migration) otomatik yönetilmediği sürece servislerin eşzamanlı güncellenmesi gerekecektir. Bu da kesintisiz canlı dağıtım (zero-downtime deployment) hedefini engeller.

---

## 4. KIDEMLİ TEST MÜHENDİSİ DEĞERLENDİRMESİ

### 4.1 Test Edilebilirlik Derecesi (Testability Score)

Projenin test edilebilirlik potansiyeli **Yüksek (8/10)** olarak değerlendirilmiştir:
- **CQRS & MediatR Avantajı:** İş mantığı (Business Logic) Command ve Query sınıflarında izole olduğu için, Controller'ları taklit etmeye (mock) gerek kalmadan sadece ilgili Handler sınıfları hedef alınarak **Unit Test (Birim Test)** yazılabilir.
- **Clean Architecture Avantajı:** Domain katmanının veritabanı erişim kütüphanelerinden (EF Core, Npgsql) tamamen bağımsız olması, saf iş kurallarının (örneğin indirim hesaplama mantığı veya KVKK veri maskeleme kuralları) karmaşık veri taklitleri gerektirmeden doğrulanmasını sağlar.

---

### 4.2 Test Ortamları ve Dış Bağımlılık Yönetimi

Platformda test otomasyonunu zorlaştıran en büyük unsur dış bağımlılıkların (PostgreSQL, MongoDB, MSSQL, IdentityServer) çokluğudur.

**Test Otomasyon Stratejisi Önerisi:**
- Geliştiricilerin yerel makinelerindeki veritabanı sürümlerinin farklı olması test sonuçlarının tutarsızlığına yol açabilir.
- Çözüm olarak, entegrasyon testlerinde **Testcontainers** (.NET) kütüphanesi kullanılmalıdır. Test çalıştırılmadan önce Docker üzerinde geçici MongoDB ve PostgreSQL container'ları ayağa kaldırılmalı, test bittiğinde bu konteynerler otomatik yok edilmelidir.

---

### 4.3 Entegrasyon ve E2E Test Senaryoları

Test süreçlerinde odaklanılması gereken en kritik 3 senaryo grubu belirlenmiştir:

#### 1. Token Yetkilendirme Testleri (Security / Authorization Assertions)
IdentityServer tarafından üretilen token'ların API yetki sınırlarını doğru koruyup korumadığı otomatik test edilmelidir:
- `StoreVisitorId` ile alınan token ile Order API veya Discount API çağrıldığında **403 Forbidden** veya **401 Unauthorized** dönmelidir.
- `StoreAdminId` ile tüm API endpoints başarılı (**200 OK**) yanıt vermelidir.

#### 2. Sipariş Akışı ve Veri Tutarlılığı E2E Senaryosu
Sipariş verme akışında servisler arası veri doğrulaması yapılmalıdır:
1. `Store.WebUI` üzerinden sipariş oluşturulur (`StoreOrderDb.Orderings` tablosuna yazılır).
2. Sipariş edilen ürünün fiyatı, Catalog servisindeki fiyat ile eşleşiyor mu? (Fiyat değişimi durumu testi).
3. Sipariş sonrasında, sipariş detaylarındaki `ProductId` değerinin Catalog servisinde karşılığı var mı? (Referential Integrity check).

#### 3. KVKK Veri Anonimleştirme Testi
[`UserAddress.cs`](file:///c:/User/Projects/Store/Services/Location/Core/Store.Location.Domain/Entities/UserAddress.cs#L37-L45) dosyasındaki `AnonymizeData()` metodunun doğrulanması:
- Bir kullanıcı hesabını sildiğinde ad, telefon, adres detay bilgilerinin maskelendiği, T.C. Kimlik numarasının null yapıldığı ve `IsAnonymized` flag'inin `true` olduğu doğrulanmalıdır.

---

## 5. AKSİYON PLANI VE YOL HARİTASI

Projenin teknik borçlarını temizlemek ve ölçeklenebilirliği artırmak amacıyla belirlenen aksiyon listesi:

```
  KISA VADE (1-3 Ay)              ORTA VADE (3-6 Ay)              UZUN VADE (6+ Ay)
 ┌──────────────────────┐        ┌──────────────────────┐        ┌──────────────────────┐
 │ • SSL Bypass         │        │ • Docker-Compose ile │        │ • Event-Driven       │
 │   engellenmesi       │        │   lokal altyapı      │        │   mimarinin          │
 │ • appsettings.json   ├───────►│   standardizasyonu   ├───────►│   kurulması          │
 │   şifre temizliği    │        │ • Testcontainers     │        │   (RabbitMQ/Outbox)  │
 │ • Shared Core        │        │   entegrasyonu       │        │ • Merkezi Log        │
 │   kütüphanesi        │        │ • gRPC haberleşme    │        │   Yönetimi (ELK)     │
 └──────────────────────┘        └──────────────────────┘        └──────────────────────┘
```

### Özet Kararlar

1. **Güvenlik Açığı:** Geliştirme ortamındaki SSL doğrulama iptali canlı ortam konfigürasyonundan kesinlikle izole edilmelidir.
2. **Mimari Düzeltme:** `Store.Catalog` projesinin `Store.Location.Domain` bağımlılığı acilen sonlandırılmalı, paylaşılan yapılar bağımsız bir pakete taşınmalıdır.
3. **Senkron Bağlantı Riski:** Sipariş ve Katalog servisleri arasındaki doğrudan HTTP bağlantıları yerine, sipariş oluşturma gibi kritik işlemlerde asenkron kuyruk yapıları mimariye dahil edilmelidir.
4. **Test Yatırımı:** CQRS'in getirdiği test kolaylığından yararlanılarak, özellikle API yetkilendirme ve KVKK anonimleştirme akışları için otomatik entegrasyon testleri yazılmalıdır.
