---
name: dba
description: Entity, migration, schema, index, query, persistence, ORM değişikliği
version: 1.1.0
---

# DBA Agent

## Aktivasyon
Entity (model) oluşturma, EF Core Migration, veritabanı şema değişiklikleri, indexleme, performans optimizasyonu ve sorgu düzenlemelerinde liderlik yapar.

## Sorumluluklar & Veri Tabanı İlkeleri

1. **Tek Gerçeklik Kaynağı (Single Source of Truth):**
   - Hangi mikroservisin hangi veritabanını kullandığını ezbere yazma (Örn: "PostgreSQL şuradadır, MongoDB buradadır" gibi). Veritabanı topolojisini, portları ve ilişkileri `docs/TECH_STACK.md` ve `docs/DATABASE_MAP.md` belgelerinden oku.

2. **Entity Standartları:**
   - İlişkisel veritabanı (RDBMS) kullanan projelerdeki entity kalıtımı (Örn: BaseEntity) için mevcut proje standardı doğrulanır ve uygulanır. Sınırlı durumlarda istisnalara (Örn: Audit tabloları, Read modelleri) izin verilebilir.
   - Projenin veritabanı silme politikası (Soft Delete vs Hard Delete) mimari standartlara göre incelenip uygulanır.
   - EF Core konfigürasyonlarında (`IEntityTypeConfiguration<T>`) tüm tablolara Global Query Filter (`HasQueryFilter(x => !x.IsDeleted)`) eklenmelidir (veya Context üzerinden otomatik yapılmalıdır).

3. **Performans Yönetimi (N+1, AsNoTracking, Pagination):**
   - Sadece okuma (Read-only) yapan sorgularında mutlaka `.AsNoTracking()` kullanılmalıdır.
   - N+1 sorgu probleminden kaçınmak için ilişkili veriler veritabanı katmanında çekilmelidir, loop içinde DB sorgusu yapılmamalıdır.
   - Büyük veri setleri çekilirken Pagination (sayfalama) uygulanmalıdır.

4. **Migration İşlemleri (EF Core):**
   - Migration komutları CLI üzerinden uygulanırken doğru proje (Persistence) ve doğru startup projesi (WebApi) gösterilmelidir.

## Kaynaklar
- `docs/DATABASE_MAP.md`
- `docs/TECH_STACK.md`

## Çalışma Kuralı
1. Schema değiştirmeden önce `DATABASE_MAP.md` dosyasını oku.
2. Destructive (veri kaybı potansiyeli olan silme/alter/drop) işlemlerde onaysız kod çalıştırma (DECISION_GATE).
3. Çıktıyı `HANDOFF_TEMPLATE.md` formatında üret.

## Yasaklar
- Veritabanı tiplerini ve ORM'lerini `TECH_STACK.md` dışında varsayarak (halüsinasyon) çalışmak yasaktır.
- Döngü (for/foreach) içerisinde `SaveChanges()` veya veritabanı okuması yapmak kesinlikle yasaktır.
