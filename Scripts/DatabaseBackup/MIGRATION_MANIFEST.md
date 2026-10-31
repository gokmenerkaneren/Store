# STORE.SLN — MIGRATION MANIFEST & VERIFICATION
# ===============================================
# Tarih: 25 Ekim 2026
# Doğrulama: SHA256 Checksums, Record Counts & Document Counts

Bu dosya, Store.sln projesinin yeni bilgisayara taşınması sırasında yedeklenen tüm veritabanlarının kayıt sayılarını ve SHA-256 kriptografik sağlama özetlerini içerir.

---

## 1. POSTGRESQL VERİTABANI DETAYLARI VE TABLO SAYILARI

- **StoreIdentityDB** (IdentityServer4 Kimlik & Kullanıcı Havuzu):
  - `AspNetUsers`: 9 kayıt
  - `AspNetUserLogins`: 3 harici login (Google, LinkedIn, Facebook)
  - `__EFMigrationsHistory`: 1 kayıt
  - `AspNetRoles`, `AspNetUserClaims`, `AspNetUserRoles`, `AspNetUserTokens`: 0 kayıt
  - Yedek Dosyaları: `PostgreSQL/StoreIdentityDB.sql` & `.dump`

- **StoreLocationDb** (Türkiye Coğrafi Veritabanı, Şubeler, Bayiler):
  - `Neighborhoods`: 51.071 mahalle kaydı
  - `Districts`: 973 ilçe kaydı
  - `Cities`: 81 il kaydı
  - `Countries`: 1 ülke kaydı
  - `Branches`: 4 şube kaydı
  - `Warehouses`: 3 depo kaydı
  - `Dealers`: 2 bayi kaydı
  - `BultenAboneligi`: 3 kayıt
  - `Companies`: 1 kayıt
  - `Mesajlar`: 1 kayıt
  - `__EFMigrationsHistory`: 5 kayıt
  - Yedek Dosyaları: `PostgreSQL/StoreLocationDb.sql` (~9.2 MB) & `.dump` (~2.8 MB)

- **StoreFavoriteDb** (Kullanıcı Favori Ürünleri):
  - `FavoriteItems`: 4 kayıt
  - `__EFMigrationsHistory`: 4 kayıt
  - Yedek Dosyaları: `PostgreSQL/StoreFavoriteDb.sql` & `.dump`

- **StoreB2BDb** (Bayi Finansal Verileri):
  - `DealerFinancials`: 0 kayıt (Şema ve indexler hazır)
  - `__EFMigrationsHistory`: 1 kayıt
  - Yedek Dosyaları: `PostgreSQL/StoreB2BDb.sql` & `.dump`

- **StoreDiscountDb** (Kupon ve İndirimler):
  - `Coupons`: 0 kayıt (Şema ve indexler hazır)
  - `__EFMigrationsHistory`: 1 kayıt
  - Yedek Dosyaları: `PostgreSQL/StoreDiscountDb.sql` & `.dump`

- **StoreOrderDb** (Siparişler ve Detaylar):
  - `Addresses`, `Orderings`, `OrdersDetails`: 0 kayıt (Şema ve indexler hazır)
  - `__EFMigrationsHistory`: 1 kayıt
  - Yedek Dosyaları: `PostgreSQL/StoreOrderDb.sql` & `.dump`

- **TeacherJobs Veritabanları** (Sistemdeki İlişkili Yan DB'ler):
  - `TeacherJobsIdentityDB`, `TeacherJobsLocationDb`, `TeacherJobsB2BDb`: Yedeklendi.

- **PostgreSQL Cluster Dökümü**:
  - `all_databases_cluster.sql`: Tüm postgres kümesi, roller ve izinler (~9.3 MB)

---

## 2. MONGODB (StoreMD) KOLEKSİYON VE DOKÜMAN SAYILARI

| Koleksiyon Adı | Doküman Sayısı | Açıklama |
|:---|:---|:---|
| `Sizes` | 59 | Beden lookup değerleri |
| `ProductImages` | 34 | Ürün fotoğrafları ve galerileri |
| `Categories` | 32 | Hiyerarşik kategori ağacı |
| `Colors` | 30 | Renk lookup değerleri |
| `ProductPrices` | 12 | Fiyat ve para birimi tanımları |
| `PriceTypes` | 8 | Fiyat türleri (Cost, Retail, Rental, vs.) |
| `Products` | 7 | Ana ürün kartları ve gömülü varyantlar |
| `DefaultTrends` | 6 | Trend ürünler vitrini |
| `Currencies` | 5 | Para birimleri (TL, USD, EUR, vs.) |
| `Measurements` | 5 | Ölçü birimleri |
| `InstagramPosts` | 5 | Instagram paylaşım vitrini |
| `DefaultSlider1` | 4 | Ana sayfa slider öğeleri |
| `BestSellers` | 4 | Çok satanlar vitrini |
| `NewProducts` | 4 | Yeni ürünler vitrini |
| `Brands` | 2 | Markalar |
| `hangfire.mongo.schema` | 1 | Hangfire şema versiyonu |
| `hangfire.mongo.server` | 1 | Hangfire sunucu kaydı |
| `ProductDetails` | 0 | Ürün detayları |
| `hangfire.mongo.jobGraph` | 0 | Hangfire kuyrukları |
| `hangfire.mongo.locks` | 0 | Hangfire kilitleri |
| `hangfire.mongo.migrationLock`| 0 | Hangfire migration kilidi |
| `hangfire.mongo.notifications`| 0 | Hangfire bildirimleri |

---

## 3. REDIS (SEPET VERİLERİ)

- `dump.rdb` (1.5 KB), `redis_commands.txt` ve `redis_data.json`
- Toplam 3 adet aktif sepet session'ı yedeklenmiştir:
  - `215eb730-0006-4ff7-8675-98f09803d00f`
  - `6209fa94-bdd8-4e98-a02d-e0331b5b597f`
  - `a8749f62-ebb1-46e8-b9cd-2152847f551a`

---

## 4. SHA-256 DOSYA DOĞRULAMA LİSTESİ

| Dosya Yolu | Boyut | SHA256 Hash |
|:---|:---|:---|
| `restore-all.ps1` | 6.38 KB | `AB23B4F3B58D8E4F6B27AA44146EF59B7040270114468353EE49EF987AA1FB0B` |
| `MongoDB\StoreMD_archive.gz` | 16.42 KB | `B670CC69B0C5254A47E53D32B8706D2CF19CF652A180DF97A07F10E45E8D2930` |
| `MongoDB\bson_dump\StoreMD\BestSellers.bson` | 1.09 KB | `DFEE706788DE453EF83BCB330F6C92258838B416C0D9B0A3206DCE5D19DF58EA` |
| `MongoDB\bson_dump\StoreMD\BestSellers.metadata.json` | 0.17 KB | `BF7C610A3A6FFDDA4B88BD5F329598284DE1942E94DFF7E2405602492DCCBE86` |
| `MongoDB\bson_dump\StoreMD\Brands.bson` | 0.51 KB | `52EB1C122B4D089BA6FDC3B2C98144D9F9DF8D321DE031E37A6D14EBE921BDD7` |
| `MongoDB\bson_dump\StoreMD\Brands.metadata.json` | 0.17 KB | `0CF729E973B913CE639148386EEB1DA851D683A0909A6DA29DFEC4B36D883ED1` |
| `MongoDB\bson_dump\StoreMD\Categories.bson` | 10.42 KB | `8B012F4259A0835B1A848CD1894D581452D1D691A0777B6D8F86EAE5E69C25DF` |
| `MongoDB\bson_dump\StoreMD\Categories.metadata.json` | 0.17 KB | `559196E67DCC92C1D42B05BD2A0FEF11CBA4D45E0DF54CFF326E1EAFCE43CE82` |
| `MongoDB\bson_dump\StoreMD\Colors.bson` | 8.87 KB | `432B2B857077EBDAC1B05707AE4F19E61882FF5BF230B45C423E84F70267EDF3` |
| `MongoDB\bson_dump\StoreMD\Colors.metadata.json` | 0.17 KB | `A899C2CE3A4CE2A6BD039CF98E6494F3C46C96C5B32E794E27C231456A2B3766` |
| `MongoDB\bson_dump\StoreMD\Currencies.bson` | 1.55 KB | `C5CE8BE6BA39B8F20A86CD47AFED36A89A65D65BCAC08EC38096AE684F3B3EB9` |
| `MongoDB\bson_dump\StoreMD\Currencies.metadata.json` | 0.17 KB | `6E9BC8F8F7EE403C53C1431B39D4C09DF0B76378A62F5AF4BE50B2763DE8DB69` |
| `MongoDB\bson_dump\StoreMD\DefaultSlider1.bson` | 1.48 KB | `82C40A3C9A1493010E85D4A9FE17FA102148B0E8FA30327668615A88B7709514` |
| `MongoDB\bson_dump\StoreMD\DefaultSlider1.metadata.json` | 0.18 KB | `CFB13337B246FE98B6B26A4D9420040523C691CE7B4A33B9B7E35237C7DC0E89` |
| `MongoDB\bson_dump\StoreMD\DefaultTrends.bson` | 1.63 KB | `A564F1F41DE9493D9B04D6A9F8E413CFE0ED414E50A4384CC6349940A391BAFE` |
| `MongoDB\bson_dump\StoreMD\DefaultTrends.metadata.json` | 0.18 KB | `CEB62BD75C0CD85F8BAE5FBC5684742E8651FDECE1DFF71686D72AC70924151C` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.jobGraph.bson` | 0 KB | `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.jobGraph.metadata.json` | 0.18 KB | `5FF730B2ACBE7B812A81E991AE496E67D1152F949F7B91979F93F3C1853FF2C5` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.locks.bson` | 0 KB | `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.locks.metadata.json` | 0.28 KB | `2D3ACACED446C894B3FF87AE0CE5AC66497B481B26E55280208D69A5D2C20874` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.migrationLock.bson` | 0 KB | `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.migrationLock.metadata.json` | 0.18 KB | `E30292397AFB90E5B4B481D0DEAF1989AF2BE69ED7ECFB5BCEF9DBC74F3B9D9F` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.notifications.bson` | 0 KB | `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.notifications.metadata.json` | 0.29 KB | `7A76E8DCBD3D7FEF85BD2335DC4A3251B5A0388B983A8D2A11EEB3E69F2104AA` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.schema.bson` | 0.01 KB | `6DCA6965656619859BE625295EF233A6A6A587A13735160BAFC813FA779261E5` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.schema.metadata.json` | 0.18 KB | `6A213611CC63C53E7EA797F5BB90DAD1C3C25F05DF7DF1009CF5E718221ACBE3` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.server.bson` | 0.15 KB | `40FD9D68471D28C5F1DE69EFBC26A1BD6182F880BB41B49550C5CFCE5CD0F370` |
| `MongoDB\bson_dump\StoreMD\hangfire.mongo.server.metadata.json` | 0.29 KB | `C616B961E7E8C05166F35666D03DD73D71CFEE11CD8FF29F7D17D3714C13C1A0` |
| `MongoDB\bson_dump\StoreMD\InstagramPosts.bson` | 1.76 KB | `C9B3D3771DABF153E3807C6CB533A6DBF952C19AE7DD19F5D1BDB078FB119CFD` |
| `MongoDB\bson_dump\StoreMD\InstagramPosts.metadata.json` | 0.18 KB | `165456CE02EEB8DF983C1148A2E75637C2E5468B57E49A7222F626AEE9C68576` |
| `MongoDB\bson_dump\StoreMD\Measurements.bson` | 1.47 KB | `F554F5D22BDEDB35BAAB3B36EE0F3A2E05D23025CF8687F7CAB291A0ACB4494E` |
| `MongoDB\bson_dump\StoreMD\Measurements.metadata.json` | 0.17 KB | `09004B3B03FFD5DB54A09699564B1E1C0A3A0A9D7D2CBAAD5CB98020E6F910BC` |
| `MongoDB\bson_dump\StoreMD\NewProducts.bson` | 1.09 KB | `104FB8220A9D56F9969FB430BC313C62054B438E049C04B16AFB372F59276C9D` |
| `MongoDB\bson_dump\StoreMD\NewProducts.metadata.json` | 0.17 KB | `0F912254F131B107F652E7C37F22B1F7264DAC03C50CC4461E2CA5C0C17348E9` |
| `MongoDB\bson_dump\StoreMD\prelude.json` | 0.05 KB | `0918AA6AEA3606F8273A5F5E56ECADA731D8DDC9012BDC49461D38379984094F` |
| `MongoDB\bson_dump\StoreMD\PriceTypes.bson` | 2.52 KB | `C1417836BE00C29BB5ECAED4B827B2D7981BC5EF6A8E33CDA84C77608CAC6ED6` |
| `MongoDB\bson_dump\StoreMD\PriceTypes.metadata.json` | 0.17 KB | `335346823373249872FA3CDA50E2DD3DB3239B3D116519904CDA64BDA85D7094` |
| `MongoDB\bson_dump\StoreMD\ProductDetails.bson` | 0 KB | `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855` |
| `MongoDB\bson_dump\StoreMD\ProductDetails.metadata.json` | 0.18 KB | `E2CE5FFA2209B0673EB20BDA5AAD3412D058CE85C7B80BDC3A074E133F3B21B0` |
| `MongoDB\bson_dump\StoreMD\ProductImages.bson` | 15.62 KB | `F91C64EBE37CB625A7FAC855FF98A442D7301BBB427971E2429B2C30B6E3E4D2` |
| `MongoDB\bson_dump\StoreMD\ProductImages.metadata.json` | 0.18 KB | `D972509D9F1E8BFA9D9263A88D8ECEAB7651B0C883A9A0BBA4803D0D3B79347A` |
| `MongoDB\bson_dump\StoreMD\ProductPrices.bson` | 6.68 KB | `4D6819D331FA809E6FE6BF04675379BF0944451E7C58644C4F17FE7952172B2F` |
| `MongoDB\bson_dump\StoreMD\ProductPrices.metadata.json` | 0.4 KB | `0C4AC8CE3F548C0AACBA1E96D977B75E69E4A6367E1935AF1ABB5B5FF21F5560` |
| `MongoDB\bson_dump\StoreMD\Products.bson` | 9.15 KB | `61724A884FADC671869D082A3154E20E0474C8752886B70BCF421E68B699CAB8` |
| `MongoDB\bson_dump\StoreMD\Products.metadata.json` | 0.17 KB | `81427ECC43F86BDDFBD62E81CF4DC874DEFB7D715C217E838CD34A221804E94E` |
| `MongoDB\bson_dump\StoreMD\Sizes.bson` | 16.96 KB | `73ABC48B695FB91B9D611C8CB4EC33A56571BF42484DC6857BDBA1D7489A8CA0` |
| `MongoDB\bson_dump\StoreMD\Sizes.metadata.json` | 0.17 KB | `3098E3B94FAAAF5373C51DE95A991CDE3B984718B794A9D8DEF50E3AE61CAF42` |
| `MongoDB\json_exports\BestSellers.json` | 2.12 KB | `DFB87FFB1704C2E0AF2DF27145BEE212B7A763DE77C191854D2BD9D0198DE04D` |
| `MongoDB\json_exports\Brands.json` | 1.01 KB | `B73C49A578BEDC97269912F27F13A68051475E6F7066689109556674C6D90FE3` |
| `MongoDB\json_exports\Categories.json` | 21.87 KB | `21FC2BC7CB0BE62A6C608D4834B5AE5422D6718E90B900F39CE6757B9DBD5921` |
| `MongoDB\json_exports\Colors.json` | 17.54 KB | `463A28D365227B9D6440C2DA45662E2636C24A125C396DA09F9A7CBFF5566468` |
| `MongoDB\json_exports\Currencies.json` | 3.13 KB | `FF66E82165ABABB26C96A76C1CCA69DA400A9E55D1C67DD1D4DD414BD76F61A1` |
| `MongoDB\json_exports\DefaultSlider1.json` | 2.86 KB | `DE71D17E80C1E6F823FD971467BCE0420724846BE716A4096AF0550E1940B2BA` |
| `MongoDB\json_exports\DefaultTrends.json` | 3.17 KB | `C22B2F04F6C399351AF6AA6EE4019E100260E8D46EAA5DE9667CF2E85C9CE30B` |
| `MongoDB\json_exports\hangfire.mongo.jobGraph.json` | 0 KB | `5239A7754876B9A70871C45E41F1BD9ABBD91ED2DE67AF926C1CFCC9C9377D51` |
| `MongoDB\json_exports\hangfire.mongo.locks.json` | 0 KB | `5239A7754876B9A70871C45E41F1BD9ABBD91ED2DE67AF926C1CFCC9C9377D51` |
| `MongoDB\json_exports\hangfire.mongo.migrationLock.json` | 0 KB | `5239A7754876B9A70871C45E41F1BD9ABBD91ED2DE67AF926C1CFCC9C9377D51` |
| `MongoDB\json_exports\hangfire.mongo.notifications.json` | 0 KB | `5239A7754876B9A70871C45E41F1BD9ABBD91ED2DE67AF926C1CFCC9C9377D51` |
| `MongoDB\json_exports\hangfire.mongo.schema.json` | 0.02 KB | `48E2970D56773FD162376ECA06F6C16141EE09436D18C3F66F4B8F2C4A42EFAA` |
| `MongoDB\json_exports\hangfire.mongo.server.json` | 0.23 KB | `99027BD620B34AA539591E61277F13BDE7E5A9518B7BC2629FED69DA9B8B0CC6` |
| `MongoDB\json_exports\InstagramPosts.json` | 3.05 KB | `A761FA37EC0343190D2DCA23C7FF6D52B4B2CF6975D338AB3CCCBE62941D7C18` |
| `MongoDB\json_exports\Measurements.json` | 3.11 KB | `95E69EE491DB095452B60687E671D81B6F7EE24E62ED4578979241D33363ED26` |
| `MongoDB\json_exports\NewProducts.json` | 2.12 KB | `88689D4F21905E7159631FA66E8AB1F8CADF2C29A4DFA33C1A4E1DC076604A9E` |
| `MongoDB\json_exports\PriceTypes.json` | 5.34 KB | `9199EAFCDB08FEFBAB56DA7AEABF5A22300BE24A61FB39D63476F3D72E89DD16` |
| `MongoDB\json_exports\ProductDetails.json` | 0 KB | `5239A7754876B9A70871C45E41F1BD9ABBD91ED2DE67AF926C1CFCC9C9377D51` |
| `MongoDB\json_exports\ProductImages.json` | 27.16 KB | `34CBE3A8262495EF6123035833F56F07E9C372A44475A71555A7D80DC56906CC` |
| `MongoDB\json_exports\ProductPrices.json` | 11.54 KB | `890BD38B818268D0C59582A8F242196256DE53B5BC4A9299F80A019A86147D87` |
| `MongoDB\json_exports\Products.json` | 13.65 KB | `B7966DDC51067E096929E96B067057975C20F1563357441AEF5187BFE59722BB` |
| `MongoDB\json_exports\Sizes.json` | 32.96 KB | `EDB668314E3AF0F039EE2EA6A46D91C61A8ED44C7F9B58DC418C824B0717ED08` |
| `PostgreSQL\all_databases_cluster.sql` | 9340.3 KB | `83446178D5F8F453218BADE23EAEA9A3BEA67B915F2A667EAE0CABEBCB8F22A3` |
| `PostgreSQL\StoreB2BDb.dump` | 3.8 KB | `BAEC4578D97AEF5A8F6437A2DEB3B2292CC05B3BF9D8AC46CEF7A6E3CFF4A12C` |
| `PostgreSQL\StoreB2BDb.sql` | 3.81 KB | `ECAB4AFEBFA44338E5DF59C65B862FECA67286D863AA07B5E4EC0680F6F4CB8E` |
| `PostgreSQL\StoreDiscountDb.dump` | 3.48 KB | `E88074A2BDA06E3A1C5449A91A64119436C7F6E55AA068F6C3D6E542D233E975` |
| `PostgreSQL\StoreDiscountDb.sql` | 3.53 KB | `E11E13BFFCD12FF7D23CC179E667978A202C421A01D15F2DC49479004E8A0CDE` |
| `PostgreSQL\StoreFavoriteDb.dump` | 3.76 KB | `468035DFBF6293D9B3C754B528A1FF9B51AA178963E3656473495594D001756A` |
| `PostgreSQL\StoreFavoriteDb.sql` | 4.48 KB | `E134EF8112AFC2F93D2292641885052AAD2343A415365100F314E8BC274F79A7` |
| `PostgreSQL\StoreIdentityDB.dump` | 17.02 KB | `81EC4284CAF0D0DCA75D77AF067E9D5C55DD9F51A561182A4B6DA1C303FD3FEB` |
| `PostgreSQL\StoreIdentityDB.sql` | 14.53 KB | `8B5710018F22606E9B1C96C2CAF527D983766539384112C605141BA76C90CBC3` |
| `PostgreSQL\StoreLocationDb.dump` | 2796.34 KB | `CC5B46EAD088667D7FE0DA75E22FCDA933CA735B21DE4C27A558FB7DDD720E85` |
| `PostgreSQL\StoreLocationDb.sql` | 9252.16 KB | `37839FB2830BCF6C71BBFC05F8F3A237E6E5FD648221721048F1F37B697AAFEF` |
| `PostgreSQL\StoreOrderDb.dump` | 7.44 KB | `426FC5AC91EEE9E3F2182C25C5B7921E5EABCDCEBAD3C1ED1F53ED12180D33F4` |
| `PostgreSQL\StoreOrderDb.sql` | 6.76 KB | `EA86F5E757F930C316A38FEE604C56EAEE965AEC0ABBD90E6617616B7F0700D3` |
| `PostgreSQL\TeacherJobsB2BDb.dump` | 3.82 KB | `5E08D319D5BB1035FFA3B4A2747BE9160CBAABC6E761B219F9D0A59E82DA9BEA` |
| `PostgreSQL\TeacherJobsB2BDb.sql` | 3.84 KB | `999DDCD214355F69AEFCFC8CF2F1D2308E338E25B3CC72BAC1DBE41C9F6243F9` |
| `PostgreSQL\TeacherJobsIdentityDB.dump` | 15.3 KB | `1536FB14C1ABFF389D8BDBC612FAF1F020208786850C8425F870A7E022990FFC` |
| `PostgreSQL\TeacherJobsIdentityDB.sql` | 11.67 KB | `EC09FCEC24DC66B95F81A885B7A867CBBAE73A4F11250A2F37C0006473068F1E` |
| `PostgreSQL\TeacherJobsLocationDb.dump` | 45.41 KB | `444C60D588362590405CA68EE37F3A152AF170C88B0CB7504FF8742661D745B2` |
| `PostgreSQL\TeacherJobsLocationDb.sql` | 34.18 KB | `E3C651C87CD137AE3AB4E626743C814F387E96FA49101BA7652DD5A26AA5BD59` |
| `Redis\dump.rdb` | 1.5 KB | `3C18601C9D7C5596BD29965C079F8463F545DA12044908BA0B85B1B1144502E2` |
| `Redis\redis_commands.txt` | 2.41 KB | `DF423258A7ECAA67776D7A69DC1A5D42E090F308EA5E5A8512ED8BCD56C37FE1` |
| `Redis\redis_data.json` | 2.68 KB | `ED543C5A59BF21668B55EB2212AB4A266EB9A38FB617F44BE5AEC4C5344EA7A7` |
| `Scripts\backup-all.ps1` | 8.25 KB | `415E1058AF677839C476AAD2906D225921380E66EE07F3E0BD4A56BF30C00611` |
| `Scripts\restore-all.ps1` | 6.38 KB | `AB23B4F3B58D8E4F6B27AA44146EF59B7040270114468353EE49EF987AA1FB0B` |
