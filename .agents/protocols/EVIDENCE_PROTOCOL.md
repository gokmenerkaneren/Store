# Evidence Protocol (Kanıt Sunma Kuralları)

## Amaç
Hiçbir ajanın kod/mimari veya güvenlik değişikliği sonrasında salt "Değişikliği başarıyla yaptım" şeklinde bir beyanla görevi kapatması kabul edilemez. Tamamlanan her görev, mutlaka teknik bir **Kanıt (Evidence)** ile desteklenmelidir. Bu kural özellikle Test Engineer, Security ve Architect için zorunludur.

## Kanıt Türleri (Evidence Types)

Görev tamamlandığında veya bir devir (Handoff) yapılırken aşağıdaki kanıt türlerinden uygun olanı rapora veya çıktısına eklenmek zorundadır:

### 1. Build Evidence (Derleme Kanıtı)
Kod yazıldıysa veya proje referansı değiştirildiyse:
- Terminal çıktısı: `dotnet build c:\User\Projects\Store\Store.slnx` -> PASS (0 error)
- Değişen proje/dosyaların başarılı şekilde derlendiğine dair log özeti.

### 2. Test Evidence (Test Kanıtı)
Yeni bir uç nokta (endpoint) veya iş mantığı eklendiyse:
- Yazılan birim veya entegrasyon testinin CLI çıktısı (örn. `dotnet test` -> PASS).
- Test kapsamının başarılı olduğunu gösteren çıktı parçası.

### 3. File State Evidence (Dosya/Kod Kanıtı)
Bir konfigürasyon veya güvenlik kuralı değiştiyse:
- İlgili dosyanın (`appsettings.json`, `.gitignore` vb.) güncellenmiş halinin kontrolü ve teyidi.

### 4. Database Evidence (Veritabanı Kanıtı)
Migration eklendiyse:
- `dotnet ef migrations add` komutunun başarıyla çalıştığına dair CLI çıktısı veya oluşan `.cs` migration dosyasının varlığı.

## Orkestratör (Agent Gravity) Görevi
Agent_Gravity, yukarıdaki kanıtlardan birini sunmayan bir ajanın görevini "Bitmiş" olarak kabul edemez. Kanıtsız dönen ajana eksik kanıtı tamamlaması için yeniden (retry) görev vermelidir.
