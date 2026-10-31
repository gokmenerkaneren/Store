# STORE.SLN — YENİ BİLGİSAYARA TAŞIMA VE VERİTABANI RESTORE REHBERİ (TASIMA.MD)
# ==============================================================================
# Hazırlayan: Agent Gravity (Kıdemli Proje Yöneticisi & Orkestratör) & Uzman Ajanlar (DBA, Architect, Security, Test)
# Tarih: 25 Ekim 2026
# Amaç: Store.sln projesinin ve veritabanlarının sıfır kayıpla yeni bilgisayara taşınması ve tek tıkla ayağa kaldırılması

> [!IMPORTANT]
> **YENİ BİLGİSAYARDA ANTIGRAVITY'YE VERİLECEK TALİMAT:**  
> Yeni bilgisayarınızda Antigravity'yi açtığınızda doğrudan şu cümleyi yazmanız yeterlidir:  
> **"tasima.md dosyasını oku ve projenin tüm veritabanlarını bu rehbere göre ayağa kaldırıp doğrula."**  
> Antigravity bu dokümandaki adımları, bağımlılıkları ve otomasyon scriptini sırasıyla işleterek tüm sistemi çalışır hale getirecektir.

---

## 1. FLASH BELLEKTEN DOSYALARI YERLEŞTİRME

Flash belleğinizde bulunan klasörleri yeni bilgisayarınızdaki şu tam yollara kopyalayınız:

| Flash Bellekteki Klasör | Yeni Bilgisayardaki Hedef Dizin | Açıklama |
|:---|:---|:---|
| `Flash:\Store` | `C:\User\Projects\Store` | Proje kaynak kodları, mikroservisler, UI ve .agents yönetimi |
| `Flash:\Database` | `C:\User\Projects\Database` | Tüm PostgreSQL, MongoDB ve Redis yedekleri, manifestolar ve scriptler |

> [!TIP]
> Yeni bilgisayarda `C:\User\Projects\` dizini henüz yoksa oluşturunuz. Dosya yollarının `C:\User\Projects\Store` ve `C:\User\Projects\Database` olarak birebir korunması, scriptlerin ve servis konfigürasyonlarının parametresiz çalışmasını sağlar.

---

## 2. YENİ BİLGİSAYAR YAZILIM GEREKSİNİMLERİ (ÖN HAZIRLIK)

Veritabanlarının ve projenin sorunsuz çalışabilmesi için yeni bilgisayarda aşağıdaki yazılımların kurulu olması gereklidir:

### 2.1 Temel Geliştirme Ortamı
- **.NET 8.0 SDK** (v8.0.x) — `dotnet --version` ile doğrulanabilir.
- **Git** ve **PowerShell 7 veya Windows PowerShell 5.1+**.

### 2.2 Veritabanı Sunucuları & Araçları
1. **PostgreSQL (16.x veya 17.x):**
   - Kurulum sırasında varsayılan kullanıcı: `postgres`
   - Şifre: `sa435558` (Proje genelinde kullanılan standart geliştirici şifresi)
   - Port: `5432`
   - *Not:* Kurulumda `Command Line Tools` (psql, pg_dump, pg_restore) seçeneğinin işaretli olduğundan emin olun.

2. **MongoDB Community Server (7.x veya 8.x):**
   - Windows Service olarak başlatılmalı (`mongod`). Port: `27017`
   - **MongoDB Database Tools:** `mongorestore` ve `mongodump` araçlarını barındıran paket kurulmalı ve PATH'e eklenmelidir (veya varsayılan `C:\Program Files\MongoDB\Tools\100\bin\` yoluna kurulmalıdır).
   - **mongosh** (MongoDB Shell) kurulu olmalıdır.

3. **Redis:**
   - Windows Service olarak (Port: `6379`) veya WSL / Docker üzerinde çalıştırılmalıdır.

---

## 3. OTOMATİK GERİ YÜKLEME (TEK ADIMDA RESTORE)

Tüm veritabanlarını (IdentityServer kullanıcıları, 51.000 mahallelik lokasyon veritabanı, ürün kataloğu, fiyatlar, fotoğraflar ve sepetler) tek komutla aktarmak için:

1. PowerShell'i **Yönetici olarak (Run as Administrator)** açın.
2. Aşağıdaki komutu çalıştırın:

```powershell
powershell -ExecutionPolicy Bypass -File "C:\User\Projects\Database\restore-all.ps1"
```

### Script Neler Yapar?
- `psql.exe`, `mongorestore.exe` ve `redis-cli.exe` araçlarını sistemde otomatik tespit eder.
- PostgreSQL üzerinde sırasıyla:
  - `StoreIdentityDB` (Kullanıcılar, harici loginler, roller)
  - `StoreLocationDb` (81 il, 973 ilçe, 51.071 mahalle, şubeler, depolar)
  - `StoreB2BDb` (Bayi finans tabloları)
  - `StoreDiscountDb` (Kupon şeması)
  - `StoreFavoriteDb` (Kullanıcı favorileri)
  - `StoreOrderDb` (Siparişler ve detaylar)
  veritabanlarını `DROP/CREATE` güvenlik adımlarıyla eksiksiz oluşturur ve verilerini yükler.
- MongoDB üzerinde:
  - `StoreMD` veritabanını `StoreMD_archive.gz` arşivinden `--drop` güvenlik kuralıyla geri yükler (Products, Categories, Prices, Hangfire dahil 22 koleksiyon).
- Redis üzerinde:
  - Aktif sepet anahtarlarını ve TTL sürelerini geri yükler.

---

## 4. ALTERNATİF / MANUEL GERİ YÜKLEME KOMUTLARI

Eğer script yerine CLI üzerinden adım adım aktarmak isterseniz:

### 4.1 PostgreSQL Manuel Restore
```powershell
$env:PGPASSWORD = "sa435558"

# 1. Seçenek (Önerilen): Bütün küme ve veritabanlarını tek hamlede geri yükleme:
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -h localhost -p 5432 -f "C:\User\Projects\Database\PostgreSQL\all_databases_cluster.sql"

# 2. Seçenek: Veritabanlarını tek tek geri yükleme:
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreIdentityDB.sql"
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreLocationDb.sql"
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreFavoriteDb.sql"
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreB2BDb.sql"
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreDiscountDb.sql"
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreOrderDb.sql"
```

### 4.2 MongoDB (StoreMD) Manuel Restore
```powershell
# Gzip arşivinden tek komutta:
& "C:\Program Files\MongoDB\Tools\100\bin\mongorestore.exe" --uri="mongodb://localhost:27017" --nsInclude="StoreMD.*" --archive="C:\User\Projects\Database\MongoDB\StoreMD_archive.gz" --gzip --drop

# Veya BSON klasöründen:
& "C:\Program Files\MongoDB\Tools\100\bin\mongorestore.exe" --uri="mongodb://localhost:27017" --db=StoreMD --drop "C:\User\Projects\Database\MongoDB\bson_dump\StoreMD"
```

### 4.3 Redis Manuel Restore
```powershell
# Yöntem A: dump.rdb dosyasını Redis servis klasörüne kopyalayıp servisi yeniden başlatmak.
# Yöntem B: Komut dosyasını redis-cli ile işletmek:
Get-Content "C:\User\Projects\Database\Redis\redis_commands.txt" | & "C:\Program Files\Another\Redis-x64-3.0.504\redis-cli.exe" -p 6379
```

---

## 5. AKTARIM SONRASI DOĞRULAMA (SANITY & TEST PROTOKOLÜ)

Restore tamamlandığında verilerin doğruluğunu teyit etmek için aşağıdaki kontrolleri yapabilirsiniz:

### 5.1 PostgreSQL Kontrol Sorgusu (PowerShell)
```powershell
$env:PGPASSWORD = "sa435558"
$psql = "C:\Program Files\PostgreSQL\17\bin\psql.exe"
& $psql -U postgres -h localhost -p 5432 -d StoreIdentityDB -c 'SELECT count(*) AS "Users" FROM \"AspNetUsers\";'
& $psql -U postgres -h localhost -p 5432 -d StoreLocationDb -c 'SELECT count(*) AS "Neighborhoods" FROM \"Neighborhoods\";'
& $psql -U postgres -h localhost -p 5432 -d StoreFavoriteDb -c 'SELECT count(*) AS "Favorites" FROM \"FavoriteItems\";'
```
*Beklenen Sonuç:*
- `AspNetUsers`: 9
- `Neighborhoods`: 51.071
- `FavoriteItems`: 4

### 5.2 MongoDB Kontrol Sorgusu (PowerShell)
```powershell
& "C:\Program Files\mongosh\mongosh.exe" mongodb://localhost:27017/StoreMD --quiet --eval "print('Products:', db.Products.countDocuments(), '| Categories:', db.Categories.countDocuments(), '| Sizes:', db.Sizes.countDocuments())"
```
*Beklenen Sonuç:*
- `Products: 7 | Categories: 32 | Sizes: 59`

---

## 6. PROJEYİ BAŞLATMA VE TEST (SMOKE TEST)

Veritabanları ayağa kalktıktan sonra Store.sln projesini test etmek için:

1. **Solution Derlemesi:**
   ```powershell
   cd C:\User\Projects\Store
   dotnet build Store.sln
   ```
2. **Yerel Başlatma:**
   ```powershell
   powershell -ExecutionPolicy Bypass -File "C:\User\Projects\Store\Scripts\run-local.ps1"
   ```
3. **Endpoint Kontrolleri:**
   - Catalog API: `http://localhost:5000/swagger`
   - IdentityServer: `http://localhost:5001/.well-known/openid-configuration`
   - Location API: `http://localhost:5002/swagger`
   - WebUI: `http://localhost:5010`

---

## 7. SORUN GİDERME (TROUBLESHOOTING)

| Hata / Durum | Olası Neden | Çözüm |
|:---|:---|:---|
| `Password authentication failed for user "postgres"` | Yeni bilgisayardaki PostgreSQL şifresi farklı | PostgreSQL şifresini `sa435558` olarak güncelleyin veya `restore-all.ps1 -PgPassword "yeniSifre"` parametresi verin. |
| `mongorestore is not recognized` | MongoDB Database Tools kurulu değil | MongoDB sitesinden `Database Tools` MSI paketini kurun. |
| `Execution of scripts is disabled on this system` | PowerShell ExecutionPolicy kısıtı | PowerShell'i `-ExecutionPolicy Bypass` parametresi ile çalıştırın. |
| `Port 5432 / 27017 already in use` | Çakışan servisler veya Docker konteynerleri | İlgili portu dinleyen eski process'leri durdurun (`Stop-Service` veya `docker stop`). |

---

> [!NOTE]
> Herhangi bir dosyanın transfer sırasında bozulup bozulmadığını kontrol etmek için `C:\User\Projects\Database\MIGRATION_MANIFEST.md` dosyasındaki SHA-256 hash listesini referans alabilirsiniz.
