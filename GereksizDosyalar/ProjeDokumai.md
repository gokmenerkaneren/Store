# Proje Veri Tabanı Mimarisi ve Analiz Raporu

Bu belge, 'Store' projesinin veri tabanı mimarisini, mikroservislerin bağlantılarını ve içerdiği tabloları çok boyutlu bir yaklaşımla incelemektedir. Rapor; Analiz, Yönetim, Mimari ve Güvenlik perspektiflerinden ele alınmıştır.

## 1. Veri Tabanı ve Tablo Yapısı (Genel Görünüm)

Projede farklı amaçlara hizmet eden, bağımsız veri tabanlarına sahip bir mikroservis mimarisi (Polyglot Persistence) kullanılmaktadır.

| Proje / Servis | Veri Tabanı Adı | Veri Tabanı Tipi | Tablolar / Koleksiyonlar |
| --- | --- | --- | --- |
| **Store.Catalog** | `StoreMD` | MongoDB | Brand, Category, Currency, Measurement, Product, ProductDetail, ProductImage, ProductPrice |
| **Store.Order** | `StoreOrderDb` | MSSQL (LocalDB) | OrdersDetails, Orderings, Addresses |
| **Store.Location** | `StoreLocationDb` | PostgreSQL | Branches, BranchAddresses, Cities, Companies, ContactInfos, Countries, Dealers, Districts, Neighborhoods, UserAddresses, Warehouses |
| **Store.Discount** | `StoreDiscountDb` | MSSQL (LocalDB) | Coupons |
| **Store.IdentityServer** | `StoreIdentityDB` | PostgreSQL | ASP.NET Identity Tabloları (AspNetUsers, AspNetRoles vb.) |

---

## 2. Kıdemli Analist Değerlendirmesi

**Mevcut Durum Analizi:**
Proje, veri domain'lerine göre çok iyi ayrıştırılmış durumdadır. Her servisin kendi verisine sahip olması, "Domain-Driven Design" (DDD) prensiplerine uygun bir analiz sürecinden geçildiğini göstermektedir.
- **Katalog Servisi (Store.Catalog):** Ürün, kategori ve marka gibi sık okunan, dinamik yapıdaki veriler için MongoDB (NoSQL) kullanılması okuma performansını artıracaktır.
- **Sipariş Servisi (Store.Order):** Sipariş süreçleri (OrdersDetails, Orderings) ilişkisel veri bütünlüğüne ihtiyaç duyduğu için MSSQL tercih edilmiş. Bu doğru bir analitik yaklaşımdır.
- **Konum Servisi (Store.Location):** Adres, il, ilçe gibi hiyerarşik yapıdaki veriler için PostgreSQL kullanılmış, coğrafi veriler veya geniş veri setleri için esneklik sağlanmış.

**Öneriler:** Servisler arası veri tutarlılığını (eventual consistency) sağlamak adına RabbitMQ veya Kafka gibi bir mesaj kuyruğu yapısının analiz dokümanlarına eklenmesi gerekmektedir. (Örn: Sipariş oluştuğunda sepetin boşaltılması veya stoktan düşülmesi).

---

## 3. Kıdemli Proje Yöneticisi Değerlendirmesi

**Yönetim ve Süreç Değerlendirmesi:**
Projenin parçalara bölünmüş (mikroservis) mimarisi, farklı geliştirici ekiplerinin bağımsız olarak çalışabilmesine olanak tanımaktadır. Bu, Çevik (Agile) geliştirme süreçleri için büyük bir avantajdır.
- **Dağıtım ve Ölçeklenebilirlik:** Katalog ve Sipariş gibi servislerin trafiği farklı olacaktır. MongoDB tabanlı Katalog servisi hızlıca ölçeklenebilirken, PostgreSQL tabanlı Identity sunucusu ayrı yönetilebilir.
- **Riskler:** Farklı veri tabanı teknolojilerinin (MSSQL, PostgreSQL, MongoDB) aynı projede kullanılması (Polyglot Persistence), operasyon ve DevOps ekibi (DBA dahil) üzerinde bakım, yedekleme ve uzmanlık maliyeti yaratacaktır.

**Öneriler:** CI/CD pipeline'larında farklı veri tabanı migrasyonları için otomasyon süreçlerinin netleştirilmesi ve Docker-Compose veya Kubernetes gibi orkestrasyon araçlarıyla ortam yönetiminin standartlaştırılması elzemdir.

---

## 4. Kıdemli Yazılım Mimarı Değerlendirmesi

**Mimari Yapı ve Teknik İnceleme:**
Mikroservis mimarisinin "Database-per-service" (Her servis için ayrı veri tabanı) deseni başarıyla uygulanmış. Veri bütünlüğünü sağlamak için servislerin birbirlerinin veri tabanlarına doğrudan erişmemesi sağlanmış, bu da sistemin "Loose Coupling" (Gevşek Bağlılık) ilkesini koruduğunu gösterir.

- **Teknoloji Seçimi:**
  - `Store.Catalog`'da MongoDB seçimi, JSON/BSON formatlı zengin ürün verileri için mükemmel.
  - `Store.Location` ve `Store.IdentityServer` için PostgreSQL seçimi, open-source olması ve yüksek hacimli işlemlerdeki başarısı nedeniyle isabetli.
  - `Store.Order` ve `Store.Discount`'ta MSSQL kullanımı, ACID özelliklerinin çok kritik olduğu finansal ve sipariş işlemlerinde tutarlılığı garantiliyor.
- **Veri Erişim (Data Access):** EF Core ve muhtemelen Dapper kullanılmış (Örn: Discount servisinde).

**Öneriler:** MSSQL LocalDB kullanımları şu an geliştirme aşamasında mantıklı olsa da, prodüksiyon (Canlı) ortamı için gerçek MSSQL Server veya Azure SQL instance'larına taşınmalıdır. Servisler arası iletişimde gRPC veya Asenkron mesajlaşma yapısı mimariye dahil edilmelidir.

---

## 5. Kıdemli Güvenlik Uzmanı Değerlendirmesi

**Sistem Güvenliği ve Risk Analizi:**
Tüm kimlik doğrulama süreçleri `Store.IdentityServer` (IdentityServer4 / Duende) üzerine yıkılmış, bu da merkezi ve güvenli bir yetkilendirme (OAuth 2.0 / OpenID Connect) sağlar.

- **Bağlantı Dizeleri (Connection Strings):** Analiz edilen `appsettings.json` dosyalarında şifreler (örneğin PostgreSQL şifreleri) düz metin (plain-text) olarak tutulmaktadır. Ayrıca, `Integrated Security=true` kullanımı (MSSQL için) ortamlarda kimlik ihlallerine yol açabilir.
- **Veri İzolasyonu:** Her servisin kendi veri tabanı olması, Olası bir SQL Injection saldırısında tüm sistemin çökmesi veya verilerin çalınması riskini azaltır (Blast radius küçüktür).

**Öneriler:**
- Hassas veriler, bağlantı dizeleri ve şifreler `appsettings.json` yerine geliştirme ortamında `UserSecrets`, canlı ortamda ise **Azure Key Vault**, **HashiCorp Vault** veya **AWS Secrets Manager** gibi güvenli alanlarda saklanmalıdır.
- IdentityServer üzerindeki PostgreSQL veri tabanının şifrelenmiş bağlantılar (SSL/TLS) üzerinden çalışması (şifreleme in transit) ve veri tabanlarındaki hassas müşteri verilerinin durağan halde şifrelenmesi (encryption at rest) zorunlu kılınmalıdır.
