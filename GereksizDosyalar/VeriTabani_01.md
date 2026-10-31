# Store Projesi Detaylı Veritabanı Yapısı Raporu

Bu raporda, **Store.sln** mikroservis projesi kapsamında kullanılan veritabanı yönetim sistemleri (DBMS), veritabanları, tablolar ve bu tabloların sütun yapısı detaylı açıklamalarıyla birlikte listelenmiştir.

---

## 1. GENEL BAKIŞ VE KULLANILAN VERİTABANLARI

Proje, mikroservis mimarisinin bir gereği olarak veritabanı bağımsızlığı prensibine (Database-per-Service) uygun tasarlanmıştır. Bu kapsamda **3 farklı Veritabanı Programı (DBMS)** üzerinde çalışan **4 farklı veritabanı** bulunmaktadır:

| Mikroservis | Veritabanı Programı (DBMS) | Veritabanı Adı | Veri Yapısı / Amacı |
| :--- | :--- | :--- | :--- |
| **Catalog** | **MongoDB** | `StoreMD` | Belge Tabanlı (NoSQL) / Ürün, Kategori, Marka vb. |
| **Location** | **PostgreSQL** | `StoreLocationDb` | İlişkisel (RDBMS) / Ülke, Şehir, Şube, Depo, Adres vb. |
| **Discount** | **Microsoft SQL Server** | `StoreDiscountDb` | İlişkisel (RDBMS) / İndirim Kuponları |
| **Order** | **Microsoft SQL Server** | `StoreOrderDb` | İlişkisel (RDBMS) / Sipariş ve Sipariş Detayları |

---

## 2. ORTAK SİSTEM ALANLARI (BASE ENTITIES)

İlişkisel ve NoSQL veritabanlarında kullanılan tabloların/koleksiyonların büyük bir kısmı, ortak takip ve loglama alanlarını barındıran temel sınıflardan türetilmiştir.

### A. Location & Catalog Mikroservisi - `BaseEntity`
Bu servislerde kimlik alanı olarak **GUID** (UUID) kullanılmıştır.

| Sütun / Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **Id** | `Guid` | Tablo bazında benzersiz kimlik (Primary Key). |
| **Notes** | `string?` | Bu kayda ait özel durumlar, açıklamalar veya sistem yöneticisi notları (Maks. 1000 karakter). |
| **IsActive** | `bool` | Kaydın sistemde aktif olarak kullanılıp kullanılmadığını belirtir (Varsayılan: `true`). |
| **IsDeleted** | `bool` | Kaydın silinip silinmediğini belirtir (Soft Delete için, Varsayılan: `false`). |
| **InsertUserId** | `Guid` | Kaydı veritabanına ilk ekleyen kullanıcının veya sistemin benzersiz kimliği. |
| **InsertDate** | `DateTimeOffset` | Kaydın veritabanına eklendiği tarih ve saat (Timezone destekli, Varsayılan: `UtcNow`). |
| **UpdateUserId** | `Guid?` | Kaydı en son güncelleyen kullanıcının benzersiz kimliği (Null olabilir). |
| **UpdateDate** | `DateTimeOffset?` | Kaydın en son güncellendiği tarih ve saat (Zaman dilimi destekli). |
| **DeleteUserId** | `Guid?` | Kaydı soft-delete yapan kullanıcının kimliği (Null olabilir). |
| **DeleteDate** | `DateTimeOffset?` | Kaydın soft-delete yapıldığı tarih ve saat (Zaman dilimi destekli). |

### B. Order Mikroservisi - `BaseEntity`
Order servisinde kimlik alanı olarak **int** (Auto-Increment / Identity) tercih edilmiştir.

| Sütun / Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **Id** | `int` | Tablo bazında benzersiz kimlik (Primary Key - Auto Increment). |
| **Notes** | `string?` | Kayda ait notlar/açıklamalar (Maks. 1000 karakter). |
| **IsActive** | `bool` | Kaydın aktiflik durumu (Varsayılan: `true`). |
| **IsDeleted** | `bool` | Yumuşak silme (Soft Delete) durumu (Varsayılan: `false`). |
| **InsertUserId** | `Guid?` | Kaydı oluşturan kullanıcının kimliği. |
| **InsertDate** | `DateTimeOffset` | Kayıt oluşturulma tarihi (Varsayılan: `UtcNow`). |
| **UpdateUserId** | `Guid?` | Son güncelleyen kullanıcının kimliği. |
| **UpdateDate** | `DateTimeOffset?` | Son güncelleme tarihi. |
| **DeleteUserId** | `Guid?` | Silen kullanıcının kimliği. |
| **DeleteDate** | `DateTimeOffset?` | Silinme tarihi. |

### C. Location Mikroservisi - `AddressBase` (Soyut Adres Şablonu)
Ortak adres alanlarını barındırır. Doğrudan tablo olarak oluşmaz, `BranchAddress` ve `UserAddress` tablolarına kalıtım yoluyla aktarılır.

| Sütun / Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **Title** | `string` | Adres başlığı (Örn: Ev Adresi, Merkez Depo) (Maks. 100 karakter, Zorunlu). |
| **ContactFullName** | `string` | Adresteki teslimat yetkilisinin adı soyadı (Maks. 150 karakter, Zorunlu). |
| **PhoneNumber** | `string` | İletişim telefon numarası (Maks. 30 karakter, Zorunlu). |
| **AddressDetail** | `string` | Sokak, Cadde, Bina No vb. açık adres detayları (Maks. 500 karakter, Zorunlu). |
| **CityId** | `Guid` | Bağlı bulunan Şehir kimliği (Foreign Key -> Cities). |
| **DistrictId** | `Guid` | Bağlı bulunan İlçe kimliği (Foreign Key -> Districts). |
| **NeighborhoodId** | `Guid` | Bağlı bulunan Mahalle kimliği (Foreign Key -> Neighborhoods). |

---

## 3. VERİTABANI DETAYLARI VE TABLOLAR

### 3.1. CATALOG MİKROSERVİSİ (`StoreMD` - MongoDB)
Bu servis MongoDB (NoSQL) kullanır. Alanlar belge (document) şeklinde saklanır. İlgili sınıfların tamamı `BaseEntity` özelliklerini miras alır.

#### A. Koleksiyon: `Brands` (Markalar)
Markalara ait temel kimlik ve tanımlama bilgilerini tutar.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar dahildir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **Code** | `string` | Markanın kısa kodu veya slug yapısı (Örn: `sez-dekor`). |
| **Name** | `string` | Markanın tam adı (Örn: `Sezadan Dekoratif`). |

#### B. Koleksiyon: `Categories` (Kategoriler)
Ürünlerin sınıflandırılması ve hiyerarşik kategori ağacı için kullanılır.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar dahildir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **ParentId** | `Guid?` | Üst kategori kimliği. Ağaç yapısı (hiyerarşi) oluşturmak için kullanılır. |
| **Code** | `string?` | Kategorinin ERP veya entegrasyon sistemlerindeki kısa kodu. |
| **Name** | `string` | Sitede görünecek kategori adı (Örn: `Nişan Konseptleri`). |

#### C. Koleksiyon: `Currencies` (Para Birimleri)
Sistemde kullanılan para birimlerinin bilgilerini tutar.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar dahildir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **Code** | `string` | Uluslararası 3 haneli ISO kodu (Örn: `TRY`, `USD`, `EUR`). |
| **Name** | `string` | Para biriminin tam adı (Örn: `Türk Lirası`, `Amerikan Doları`). |
| **Symbol** | `string` | Para biriminin simgesi (Örn: `₺`, `$`, `€`). |

#### D. Koleksiyon: `Measurements` (Ölçü Birimleri)
Ürün miktarlarını tanımlamada kullanılan birimler.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar dahildir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **Code** | `string` | Ölçü biriminin sistem kodu. |
| **Name** | `string` | Birimin tam adı (Örn: `Kilogram`, `Adet`, `Metre`). |
| **Symbol** | `string` | Birimin kısa sembolü (Örn: `Kg`, `Ad.`, `m`). |

#### E. Koleksiyon: `Products` (Ürün Kartları)
Ürünlere ait temel, mali, lojistik ve SEO verilerini tutar.
* **Kimlik Türü:** MongoDB `ObjectId` (string olarak temsil edilir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **Id** | `string` | Ürünün benzersiz kimliği (MongoDB ObjectId, Primary Key). |
| **ProductCode** | `string` | Şirketin kullandığı Stok/SKU kodu (Örn: `DAFT017-MT02`). |
| **Barcode** | `string?` | Ürünün EAN/UPC barkodu (Depo el terminalleri ve okuyucular için kritik). |
| **Name** | `string` | Ürünün sitedeki adı (Örn: `Alice Arka Fon`). |
| **ShortDescription** | `string?` | Liste sayfalarında görünecek 1-2 cümlelik özet. |
| **Description** | `string?` | Ürün detay sayfasındaki zengin (HTML) açıklama metni. |
| **IsRentalEnabled** | `bool` | Ürün kiralanabilir mi? (Varsayılan: `true`). |
| **IsSaleEnabled** | `bool` | Ürün satın alınabilir mi? (Varsayılan: `true`). |
| **IsPublished** | `bool` | Ürün yayında mı? Taslak ürünleri filtrelemek için kullanılır (Varsayılan: `true`). |
| **TaxRate** | `int` | Ürünün KDV oranı (Örn: `20`, `10`, `1`). Fatura hesaplamaları için zorunludur. |
| **Weight** | `double?` | Ürünün ağırlığı (Kg). Kargo fiyatı hesabı için. |
| **Desi** | `double?` | Hacimsel ağırlık (Desi). Lojistik hesabı için. |
| **Slug** | `string` | Temiz URL bağlantısı (Örn: `alice-arka-fon`). |
| **MetaTitle** | `string?` | Google arama sonuçlarındaki başlık (SEO). |
| **MetaDescription** | `string?` | Google arama sonuçlarındaki özet metin (SEO). |
| **CategoryId** | `string` | Bağlı olduğu kategori (MongoDB ObjectId formatında Guid). |
| **BrandId** | `string?` | Bağlı olduğu marka (MongoDB ObjectId formatında Guid). |
| **MeasurementId** | `string` | Bağlı olduğu birim kimliği (MongoDB ObjectId formatında Guid). |

#### F. Koleksiyon: `ProductDetails` (Ürün Detayları)
Ürünlerin teknik detayları, ebatları ve arama etiketlerini barındırır.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar dahildir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **ProductId** | `Guid` | Bağlı olduğu ürünün Guid kimliği. |
| **Description** | `string` | Teknik özellik açıklaması metni. |
| **Feature** | `string` | Katalogda vurgulanacak özellik (Örn: `Paslanmaz Çelik`). |
| **Dimension** | `string` | En x Boy x Yükseklik gibi ebat bilgileri. |
| **AdditionalInfo** | `string` | Kurulum, montaj veya paket içeriğine dair ek açıklamalar. |
| **Tag** | `string` | Arama algoritmaları için atanmış anahtar kelimeler/etiketler. |

#### G. Koleksiyon: `ProductImages` (Ürün Görselleri)
Ürün görsellerini ve gösterim sıralarını tutar.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar dahildir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **ProductId** | `Guid` | Görselin ait olduğu ürünün Guid kimliği. |
| **ImageUrl** | `string` | CDN veya AWS S3 üzerindeki görsel adresi (URL). |
| **IsMain** | `bool` | Ürünün kapak (ana) fotoğrafı olup olmadığını belirtir. |
| **DisplayOrder** | `int` | Görselin galerideki sıralaması. |
| **AltText** | `string` | Görme engelliler ve SEO için resim alternatif metni. |

#### H. Koleksiyon: `ProductPrices` (Ürün Fiyatları)
Ürünlerin dönemsel, perakende, kiralama, toptan veya bayi bazlı fiyatlarını tutar.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar dahildir)

| Alan Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **ProductId** | `Guid` | Fiyatın ait olduğu ürünün Guid kimliği. |
| **CurrencyId** | `Guid` | Fiyatın para birimi (Zorunlu referans -> Currencies). |
| **PriceType** | `PriceType` (Enum) | Fiyat türü: `Cost(1)` (Maliyet), `Retail(2)` (Perakende), `Rental(3)` (Kiralama), `Wholesale(4)` (Toptan Satış), `Wholerental(5)` (Toptan Kiralama). |
| **PriceListTierId** | `Guid?` | Bayi kademesi (VIP Bayi vb.) kimliği. Null ise genel fiyattır. |
| **MinimumQuantity** | `int` | Fiyatın geçerli olması gereken en az sepet miktarı (Varsayılan: `1`). |
| **Amount** | `decimal` | Geçerli asıl tutar. |
| **DiscountedAmount** | `decimal?` | Varsa indirimli kampanya tutarı. |
| **IsTaxIncluded** | `bool` | KDV'nin fiyata dahil olup olmadığı. |
| **StartDate** | `DateTimeOffset?` | Fiyatın/Kampanyanın geçerlilik başlangıç tarihi. |
| **EndDate** | `DateTimeOffset?` | Fiyatın geçerlilik bitiş tarihi (Null ise süresiz güncel fiyattır). |

---

### 3.2. LOCATION MİKROSERVİSİ (`StoreLocationDb` - PostgreSQL)
Bu mikroservis PostgreSQL kullanır. Bütün varlıklar **GUID** bazlı `BaseEntity` sınıfından türer.

#### A. Tablo: `Countries` (Ülkeler)
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **CountryName** | `VARCHAR(100)` | `Required` | Ülke adı (Örn: `Türkiye`). |
| **CountryCode** | `VARCHAR(5)` | - | Uluslararası ISO kodu (Örn: `TR`). |
| **UPhoneCode** | `VARCHAR(10)` | - | Uluslararası telefon alan kodu (Örn: `+90`). |

#### B. Tablo: `Cities` (Şehirler)
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **CityName** | `VARCHAR(100)` | `Required` | Şehir adı (Örn: `İzmir`). |
| **PlateCode** | `VARCHAR(10)` | - | Şehir plaka kodu (Örn: `35`). |
| **CountryId** | `UUID` | `Foreign Key` | Bağlı olduğu ülke kimliği (-> Countries). |

#### C. Tablo: `Districts` (İlçeler)
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **DistrictName** | `VARCHAR(100)` | `Required` | İlçe adı (Örn: `Karşıyaka`). |
| **CityId** | `UUID` | `Foreign Key` | Bağlı olduğu şehir kimliği (-> Cities). |

#### D. Tablo: `Neighborhoods` (Mahalleler)
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **NeighborhoodName** | `VARCHAR(150)` | `Required` | Mahalle adı (Örn: `Bostanlı Mahallesi`). |
| **PostalCode** | `VARCHAR(20)` | - | Posta kodu (Örn: `35590`). |
| **DistrictId** | `UUID` | `Foreign Key` | Bağlı olduğu ilçe kimliği (-> Districts). |

#### E. Tablo: `Companies` (Şirketler)
Sistemi kullanan firma/şubelerin kurumsal verilerini tutar.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **LegalName** | `VARCHAR(250)` | `Required` | Faturadaki resmi ticari unvanı. |
| **BrandName** | `VARCHAR(250)` | `Required` | Marka adı. |
| **TaxOffice** | `VARCHAR(100)` | - | Şirketin bağlı olduğu Vergi Dairesi. |
| **TaxNumber** | `VARCHAR(50)` | - | Vergi Kimlik Numarası (VKN). |
| **TradeRegistryNumber**| `VARCHAR(100)` | - | Ticaret Sicil Numarası. |
| **MersisNumber** | `VARCHAR(100)` | - | MERSİS numarası. |
| **Website** | `VARCHAR(200)` | - | Kurumsal web sitesi adresi. |
| **LogoUrl** | `VARCHAR(500)` | - | Şirket logosunun URL yolu. |
| **MainContactId** | `UUID?` | `Foreign Key` | Ana iletişim bilgisi kimliği (-> ContactInfos). |

#### F. Tablo: `ContactInfos` (İletişim Bilgileri)
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **Email** | `VARCHAR(150)` | - | İletişim e-posta adresi. |
| **Phone** | `VARCHAR(30)` | - | Sabit telefon numarası. |
| **MobilePhone** | `VARCHAR(30)` | - | Cep telefonu numarası. |
| **AuthorizedPerson** | `VARCHAR(100)` | - | İletişim kurulacak yetkili kişi (Ad Soyad). |

#### G. Tablo: `Branches` (Şubeler)
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **Name** | `VARCHAR(150)` | `Required` | Şube adı (Örn: `Karşıyaka Mağazası`). |
| **BranchType** | `VARCHAR(100)` | - | Şubenin faaliyet türü (Örn: Mağaza, Depo, Üretim). |
| **CompanyId** | `UUID` | `Foreign Key` | Bağlı olduğu şirket kimliği (-> Companies). |
| **ContactInfoId** | `UUID?` | `Foreign Key` | Şube iletişim bilgileri (-> ContactInfos). |
| **GeoLocationCoordinates**| `VARCHAR(100)`| - | Koordinatlar (Enlem,Boylam) (Örn: `38.456,27.123`). |

#### H. Tablo: `BranchAddresses` (Şube Adresleri)
* **Miras:** `AddressBase` (Tüm ortak adres alanlarını barındırır)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **BranchId** | `UUID` | `Foreign Key` | Adresin ait olduğu Şube/Depo kimliği (-> Branches). |
| **LoadingDockInstructions**| `VARCHAR(500)`| - | Lojistik tırlar için yükleme/boşaltma rampası talimatı. |

#### I. Tablo: `Dealers` (Bayiler)
B2B sisteminde toptan işlem yapan müşteri profilleri.
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **CompanyName** | `VARCHAR(250)` | `Required` | Bayinin ticari unvanı. |
| **TaxNumber** | `VARCHAR(50)` | `Required` | Fatura kesimi için vergi numarası. |
| **DiscountRate** | `NUMERIC` | - | Bayiye otomatik uygulanacak iskonto oranı (Örn: `0.15` = %15). |
| **CreditLimit** | `NUMERIC` | - | Açık hesap/veresiye çalışabileceği maks. limit. |
| **CurrentBalance** | `NUMERIC` | - | Bayinin anlık borç/alacak bakiyesi. |
| **ContactInfoId** | `UUID` | `Foreign Key` | İletişim bilgisi kimliği (-> ContactInfos). |

#### J. Tablo: `UserAddresses` (Kullanıcı / Müşteri Adresleri)
* **Miras:** `AddressBase` (Tüm ortak adres alanlarını barındırır)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **CustomerId** | `UUID` | - | Adresin sahibi olan müşterinin kimliği. |
| **InvoiceType** | `INTEGER` | `Enum (1 veya 2)` | Fatura Türü: `Individual(1)` (Bireysel), `Corporate(2)` (Kurumsal). |
| **IdentityNumber** | `VARCHAR(11)` | - | Bireysel faturalar için T.C. Kimlik No (Şifrelenmelidir). |
| **TaxOffice** | `VARCHAR(100)` | - | Kurumsal fatura için vergi dairesi. |
| **TaxNumber** | `VARCHAR(50)` | - | Kurumsal fatura için vergi numarası. |
| **CompanyId** | `UUID?` | `Foreign Key` | Kurumsal adresin bağlı olduğu bayi şirketi (-> Companies). |
| **IsAnonymized** | `BOOLEAN` | - | KVKK gereği adresin anonimleştirilip silinmesi flag'i. |

#### K. Tablo: `Warehouses` (Depolar)
* **Miras:** `BaseEntity` (Guid bazlı ortak alanlar)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **Name** | `VARCHAR(150)` | `Required` | Deponun adı (Örn: `Bornova Lojistik Depo`). |
| **Code** | `VARCHAR(50)` | - | Depo sistem kodu (Örn: `WH-01`). |
| **Address** | `VARCHAR(500)` | - | Deponun açık adresi. |
| **City** | `VARCHAR(100)` | - | Bulunduğu şehir. |
| **Phone** | `VARCHAR(30)` | - | Depo telefon numarası. |
| **IsDefault** | `BOOLEAN` | - | Sistemdeki ana/varsayılan depo mu? |
| **BranchId** | `UUID?` | `Foreign Key` | Deponun bağlı olduğu şube kimliği (-> Branches). |

---

### 3.3. DISCOUNT MİKROSERVİSİ (`StoreDiscountDb` - MS SQL Server)
Bu servis MS SQL Server kullanır. Dapper veritabanı erişim kütüphanesi ile optimize edilmiştir.

#### A. Tablo: `Coupons` (Kuponlar)
Müşterilerin kullanabileceği indirim kuponlarını tanımlar.

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **Id** | `INT` | `PK, Identity(1,1)` | Kuponun benzersiz otomatik artan kimliği. |
| **Code** | `NVARCHAR(MAX)`| `Required` | Kupon kodu (Örn: `YAZ2026`). |
| **Rate** | `INT` | - | İndirim oranı (Örn: %10 indirim için `10`). |
| **IsActive** | `BIT` | - | Kuponun aktif/kullanılabilir olma durumu. |
| **ValidDate** | `DATETIME2` | - | Kuponun son kullanma (geçerlilik) tarihi. |

---

### 3.4. ORDER MİKROSERVİSİ (`StoreOrderDb` - MS SQL Server)
Bu servis sipariş yönetimini gerçekleştirir ve MS SQL Server kullanır. Sınıflar **int** (Identity) bazlı `BaseEntity` yapısını miras alır.

#### A. Tablo: `Addresses` (Sipariş Adresleri)
Sipariş anında kaydedilen kargo/fatura adresini dondurarak saklar.
* **Miras:** `BaseEntity` (int bazlı ortak alanlar dahildir)

| Sütun Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **UserId** | `string` | Adresi kullanan/siparişi veren kullanıcının kimliği. |
| **Country** | `string` | Ülke bilgisi. |
| **City** | `string` | Şehir bilgisi. |
| **District** | `string` | İlçe bilgisi. |
| **Region** | `string` | Bölge/Eyalet bilgisi. |
| **Street** | `string` | Cadde/Sokak bilgisi. |
| **PostalCode** | `string` | Posta kodu. |
| **Detail** | `string` | Apartman no, daire no vb. açık adres detayı. |
| **Phone** | `string` | Alıcı telefon numarası. |

#### B. Tablo: `Orderings` (Sipariş Master Kartı)
Siparişin genel bilgilerini barındırır.
* **Miras:** `BaseEntity` (int bazlı ortak alanlar dahildir)

| Sütun Adı | Veri Tipi | Açıklama / Yorum |
| :--- | :--- | :--- |
| **UserId** | `string` | Siparişi veren kullanıcının sistemdeki kimliği. |
| **TotalPrice** | `decimal` | Siparişin KDV vb. dahil toplam tutarı. |
| **OrderDate** | `DateTime` | Siparişin verildiği tarih ve saat (Varsayılan: `DateTime.Now`). |

#### C. Tablo: `OrdersDetails` (Sipariş Satır/Kalem Detayları)
Sipariş içerisindeki her bir ürünü ve ürünün sipariş anındaki fiyat/miktar bilgilerini tutar.
* **Miras:** `BaseEntity` (int bazlı ortak alanlar dahildir)

| Sütun Adı | Veri Tipi | Kısıtlamalar (Constraint) | Açıklama / Yorum |
| :--- | :--- | :--- | :--- |
| **ProductId** | `string` | Sipariş edilen ürünün kimliği (Catalog servisindeki Product ID ile eşleşir). |
| **ProductPrice**| `decimal` | Ürünün sipariş anındaki birim fiyatı. |
| **ProductAmount**| `int` | Sipariş edilen ürün adeti. |
| **TotalPrice** | `decimal` | Bu kaleme ait toplam tutar (`ProductPrice` * `ProductAmount`). |
| **OrderingId** | `int` | `Foreign Key` | Bağlı olduğu sipariş master kartı kimliği (-> Orderings). |
