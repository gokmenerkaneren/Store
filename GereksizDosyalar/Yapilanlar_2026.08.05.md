# 2026.08.05 - Yapılan Değişiklikler ve B2B Servisi Ayrımı

Bugün, Domain Driven Design (DDD) ve Bounded Context kuralları gereği, Store.Location mikroservisinde bulunan ve coğrafi/lokasyon alanları yerine finansal/ticari veriler içeren alanların ayrıştırılması işlemi gerçekleştirilmiştir.

## 1. Store.Location Mikroservisinde Yapılan Temizlik
- **Dealer Entity:** Dealer entity'si içerisindeki TaxNumber, TaxOffice, DiscountRate, CreditLimit, ve CurrentBalance alanları silindi. Sadece bayiye ait temel iletişim ve adres bağımlılıkları bırakıldı.
- **CQRS ve Handler Güncellemeleri:** 
  - CreateDealerCommandHandler ve UpdateDealerCommandHandler içerisinde finansal alan eşleşmeleri ve atamaları kaldırıldı.
  - GetDealerByIdQueryResult ve GetDealerQueryResult nesnelerinden ve bu nesneleri dolduran GetDealerByIdQueryHandler ile GetDealersQueryHandler içerisinden ilgili alanlar temizlendi.
- **DuplicateCheckerHelper Güncellemesi:** TaxNumber alanı silindiği için firma kayıtlarında mükerrer vergi numarası kontrolü kodu bayilerin sadece isim CompanyName bazlı kontrol edilmesine göre sadeleştirildi.
- **Entity Framework Core Migrations:** RemoveDealerFinancialFields adlı migration oluşturuldu. Bu adımda, silinen alanları çağıran veritabanındaki eski w_dealer_full_details adlı PostgreSQL View'ı da hata yaratmaması adına başarılı bir şekilde yeniden düzenlendi ve veritabanı dotnet ef database update ile güncellendi.
- **Unit Test Düzeltmeleri:** Adres refactoringi ile CQRS handler'larına enjekte edilen IRepository<WarehouseAddress> bağımlılığı WarehouseControllerTests.cs dosyasına Mock olarak geçilerek projenin hatasız derlenmesi (0 hata) sağlandı.

## 2. Store.B2B Mikroservisinin Oluşturulması
Bayilerin ticari sınırlarının (Bounded Context) kontrol edileceği yepyeni bir mikroservis ayağa kaldırıldı.
- **Proje Yapısı:** Store.B2B klasörü altında Store.B2B.Domain, Store.B2B.Application, Store.B2B.Persistence ve Store.B2B.WebApi projeleri dotnet new komutları kullanılarak oluşturuldu ve Solution'a (Store.slnx) dahil edildi.
- **Store.B2B.Domain:** İçerisine ID ve tarihleri barındıran BaseEntity eklendi. Ayrıca finansal verileri tutacak olan DealerFinancial Entity'si tasarlandı.
- **Store.B2B.Persistence:** 
  - Gerekli EF Core ve Npgsql paketleri projeye eklendi.
  - B2BContext oluşturuldu. TaxNumber alanının PostgreSQL'de eşsiz (IsUnique()) olması garanti altına alındı.
- **Store.B2B.WebApi:** 
  - ppsettings.json dosyasına PostgreSQL veritabanı olan StoreB2BDb bağlantı bilgileri (ConnectionString) eklendi.
  - Program.cs içerisine B2BContext IoC Container konfigürasyonu yapıldı.
- **Veritabanı Güncellemesi:** InitialB2BDb_20260805 migration'ı oluşturularak PostgreSQL üzerinde StoreB2BDb ayağa kaldırıldı. 

Tüm bu işlemler sonrası mikroservis mimarisine %100 uyumlu, Bounded Context karmaşasından kurtulmuş ve CQRS yapıları temizlenmiş bir proje elde edildi.
