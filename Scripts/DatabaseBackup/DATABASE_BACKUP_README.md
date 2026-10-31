# STORE.SLN — VERİTABANI YEDEK VE AKTARIM MERKEZİ (DATABASE BACKUP & MIGRATION)
# ==============================================================================
# Tarih: 22 Ekim 2026
# Hazırlayan: Antigravity (DBA & Senior Project Manager)
# Amaç: Store.sln projesinin başka bir bilgisayara taşınması ve veritabanlarının eksiksiz aktarımı

Bu dizin (`C:\User\Projects\Database`), Store.sln e-ticaret platformunun kullandığı tüm veritabanlarının tam şema, ilişki, veri ve konfigürasyon yedeklerini içermektedir.

---

## 1. YEDEK KAPSAMI VE VERİTABANI HARİTASI

| DBMS | Veritabanı Adı | Mikroservis | Port | Yedek Dosyaları / Formatlar |
|:---|:---|:---|:---|:---|
| **PostgreSQL 17/16** | `StoreIdentityDB` | IdentityServer (Auth/Kullanıcılar) | 5432 | `PostgreSQL/StoreIdentityDB.sql`, `.dump` |
| **PostgreSQL 17/16** | `StoreLocationDb` | Location (Adresler, Şehirler, Bayiler) | 5432 | `PostgreSQL/StoreLocationDb.sql`, `.dump` |
| **PostgreSQL 17/16** | `StoreB2BDb` | B2B (Bayi Finansal Verileri) | 5432 | `PostgreSQL/StoreB2BDb.sql`, `.dump` |
| **PostgreSQL 17/16** | `StoreDiscountDb` | Discount (Kuponlar) | 5432 | `PostgreSQL/StoreDiscountDb.sql`, `.dump` |
| **PostgreSQL 17/16** | `StoreOrderDb` | Order (Siparişler & Detaylar) | 5432 | `PostgreSQL/StoreOrderDb.sql`, `.dump` |
| **PostgreSQL 17/16** | `StoreFavoriteDb` | Favorite (Favori Ürünler) | 5432 | `PostgreSQL/StoreFavoriteDb.sql`, `.dump` |
| **PostgreSQL 17/16** | *Tüm Küme (Cluster)* | Tüm Roller ve Veritabanları | 5432 | `PostgreSQL/all_databases_cluster.sql` |
| **MongoDB 7.x/8.x** | `StoreMD` | Catalog (Ürünler, Kategoriler, Fiyatlar) | 27017 | `MongoDB/StoreMD_archive.gz`, `bson_dump/`, `json_exports/` |
| **Redis** | In-Memory DB | Basket (Aktif Sepetler & TTL) | 6379 | `Redis/dump.rdb`, `redis_data.json`, `redis_commands.txt` |

*Not: Ek olarak sistemde bulunan `TeacherJobs` veritabanları da `PostgreSQL/` altında yedeklenmiştir.*

---

## 2. YENİ BİLGİSAYARDA HIZLI KURULUM (ANTIGRAVITY İLE)

Yarın yeni bilgisayara geçtiğinizde Antigravity'ye şu talimatı vermeniz yeterlidir:
> *"C:\User\Projects\Database altındaki yedekleri yeni bilgisayarımdaki veritabanlarına aktar."*

Antigravity doğrudan bu dizindeki `restore-all.ps1` scriptini çalıştırarak veya ilgili servisleri denetleyerek tüm aktarımı dakikalar içinde otomatik yapacaktır.

---

## 3. OTOMATİK GERİ YÜKLEME (POWERSHELL SCRIPTI)

PowerShell üzerinden tek komutla tüm veritabanlarını ayağa kaldırmak için:

```powershell
cd C:\User\Projects\Database
.\restore-all.ps1
```

### Parametreli Çalıştırma Seçenekleri:
```powershell
.\restore-all.ps1 -PgHost localhost -PgPort 5432 -PgUser postgres -PgPassword "sa435558" -MongoUri "mongodb://localhost:27017"
```

---

## 4. MANUEL GERİ YÜKLEME KOMUTLARI

Otomasyon scripti yerine adım adım CLI üzerinden aktarmak isterseniz:

### A. PostgreSQL Aktarımı
```bash
# Şifre ortam değişkeni:
set PGPASSWORD=sa435558

# 1. Seçenek: Tek komutta tüm küme ve kullanıcıları geri yükleme:
psql -U postgres -h localhost -p 5432 -f "C:\User\Projects\Database\PostgreSQL\all_databases_cluster.sql"

# 2. Seçenek: Servis veritabanlarını tek tek geri yükleme:
psql -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreIdentityDB.sql"
psql -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreLocationDb.sql"
psql -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreB2BDb.sql"
psql -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreDiscountDb.sql"
psql -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreOrderDb.sql"
psql -U postgres -h localhost -p 5432 -d postgres -f "C:\User\Projects\Database\PostgreSQL\StoreFavoriteDb.sql"

# Veya Custom format (.dump) üzerinden pg_restore ile:
# pg_restore -U postgres -h localhost -p 5432 -C -d postgres "C:\User\Projects\Database\PostgreSQL\StoreLocationDb.dump"
```

### B. MongoDB (StoreMD) Aktarımı
```bash
# Sıkıştırılmış arşivden tek komutla:
mongorestore --uri="mongodb://localhost:27017" --nsInclude="StoreMD.*" --archive="C:\User\Projects\Database\MongoDB\StoreMD_archive.gz" --gzip --drop

# Veya BSON klasöründen:
mongorestore --uri="mongodb://localhost:27017" --db=StoreMD --drop "C:\User\Projects\Database\MongoDB\bson_dump\StoreMD"
```

### C. Redis Aktarımı
```bash
# 1. Yöntem: dump.rdb dosyasını Redis çalışma dizinine kopyalayıp servisi başlatmak.
# 2. Yöntem: redis-cli üzerinden komut dosyasını pipe etmek:
cat "C:\User\Projects\Database\Redis\redis_commands.txt" | redis-cli
```

---

## 5. DİZİN YAPISI

```
C:\User\Projects\Database\
│
├── README.md                          # Bu dokümantasyon
├── MIGRATION_MANIFEST.md              # SHA256 özetleri ve dosya listesi
├── restore-all.ps1                    # Otomatik tek tıkla geri yükleme scripti
│
├── PostgreSQL\
│   ├── all_databases_cluster.sql      # Tüm postgres cluster ve kullanıcılar
│   ├── StoreIdentityDB.sql / .dump    # IdentityServer veritabanı
│   ├── StoreLocationDb.sql / .dump    # Lokasyon, adresler, bayiler
│   ├── StoreB2BDb.sql / .dump         # B2B finans verileri
│   ├── StoreDiscountDb.sql / .dump    # İndirim kuponları
│   ├── StoreOrderDb.sql / .dump       # Sipariş kayıtları
│   ├── StoreFavoriteDb.sql / .dump    # Favori kayıtları
│   └── TeacherJobs*.sql / .dump       # İlişkili yan veritabanları
│
├── MongoDB\
│   ├── StoreMD_archive.gz             # Tek dosya sıkıştırılmış mongodump arşivi
│   ├── bson_dump\StoreMD\             # Koleksiyon bazlı BSON + metadata.json dosyaları
│   └── json_exports\                  # İnsan tarafından okunabilir ve düzenlenebilir JSON dosyaları
│
├── Redis\
│   ├── dump.rdb                       # Orijinal RDB ikili anlık görüntü
│   ├── redis_data.json                # JSON formatında sepet anahtarları ve içerikleri
│   └── redis_commands.txt             # Çalıştırılabilir SET komutları
│
└── Scripts\
    └── restore-all.ps1                # Yedek geri yükleme script kopyası
```

---

## 6. GEREKSİNİMLER (YENİ BİLGİSAYAR İÇİN)

Yeni bilgisayarda bulunması gereken yazılımlar:
1. **PostgreSQL** 16.x veya 17.x (Varsayılan kullanıcı: `postgres`, şifre: `sa435558`, port: `5432`)
2. **MongoDB Community Server** 7.x veya 8.x + **MongoDB Database Tools** (mongodump/mongorestore) (Port: `27017`)
3. **Redis** (Port: `6379`) veya Redis Docker konteyneri
