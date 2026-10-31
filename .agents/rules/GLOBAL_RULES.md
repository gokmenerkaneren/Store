# GLOBAL RULES

## Kaynak gerçekliği

1. Proje kaynaklarında bulunmayan bilgi varsayılmaz.
2. Çelişkiler çözülmeden kritik implementasyon yapılmaz.
3. Kod ile doküman uyuşmuyorsa uyuşmazlık raporlanır.

## Mimari

- Bounded Context sınırları korunur.
- Mikroservisler birbirlerinin Domain katmanına doğrudan referans vermez.
- Mevcut Clean Architecture/CQRS kuralları kaynak dokümandaki mevcut mimariyle uyumlu uygulanır.

## Güvenlik

- Secret yönetimi güvenli mekanizma ile yapılır.
- Hassas veriler loglanmaz.
- Authorization varsayılan olarak güvenli tarafta ele alınır.

## Kalite

- Kod değişikliği mümkün olduğunda ilgili testlerle doğrulanır.
- Build/test sonuçları raporlanır.
- Refactoring davranış değişikliği yaratıyorsa ayrıca belirtilir.

## Dokümantasyon

Kod, mimari, iş kuralı, veri modeli veya karar değiştiyse ilgili gerçeklik dosyası güncellenir.
