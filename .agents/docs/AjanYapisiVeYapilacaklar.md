# AJAN YAPISI VE YAPILACAKLAR
# ═══════════════════════════════════════════
# Store.sln için 6 Kişilik Sanal Yazılım Takımı — Ajan Sistemi Tasarım Dokümanı
#
# Hazırlayan: Antigravity
# Tarih: 2 Ekim 2026
# Versiyon: 1.0

---

## 1. MEVCUT AJAN SİSTEMİNİN ANALİZİ

### 1.1 Mevcut Yapı: Tek Süper-Ajan Modeli

Şu anda `Agent_Gravity.md` dosyasında tek bir ajan (Antigravity) tanımlı. Bu ajan, 6 farklı "şapka" takarak aşağıdaki rolleri üstleniyor:

| Şapka | Rol | Mevcut Durum |
|:---|:---|:---|
| 🎩 1 | Proje Yöneticisi | ⚠️ Tetikleyiciler eksik |
| 🎩 2 | İş Analisti | ⚠️ Validasyon şablonları eksik |
| 🎩 3 | Yazılım Mimarı | ✅ En iyi tanımlanmış şapka |
| 🎩 4 | DBA | ⚠️ Sorgu optimizasyon kuralları eksik |
| 🎩 5 | Güvenlik Mühendisi | ⚠️ Tarama/denetim senaryoları eksik |
| 🎩 6 | Test Mühendisi | ❌ Çok yüzeysel — test stratejisi yok |

### 1.2 Mevcut Yapının Güçlü Yanları

1. ✅ **Hafıza sistemi** iyi tasarlanmış (5 doküman, güncelleme kuralları tanımlı)
2. ✅ **Çalışma akışı** (ANLA → RİSK → PLANLA → KODLA → DOĞRULA → RAPORLA) net
3. ✅ **Yasak listesi** açık ve uygulanabilir
4. ✅ **Naming convention** tutarlı (Command/Query/Handler/DTO)
5. ✅ **ADR (Architecture Decision Records)** karar kayıtları tutuluyor

### 1.3 Mevcut Yapının Zayıf Yanları ve Eksiklikleri

#### ❌ Eksik 1: Şapka Aktivasyon Kuralları
Şapkaların ne zaman devreye gireceği tanımlı değil. Örneğin:
- "Migration yazarken DBA şapkası lider olur" gibi bir kural yok
- Hangi görev tipinde hangi şapkanın **öncelikli** olduğu belirsiz

#### ❌ Eksik 2: Çıktı Şablonları
Her şapkanın üretmesi gereken çıktı formatı tanımlı değil:
- Proje Yöneticisi ne rapor eder?
- Test Mühendisi hangi formatta test planı sunar?

#### ❌ Eksik 3: Kod Denetim Otomasyonu
Periyodik code review / audit mekanizması yok:
- Haftalık / sprint sonrası otomatik denetim tetikleyicisi eksik
- Denetim kapsamı ve rapor formatı tanımlı değil

#### ❌ Eksik 4: Ajan Arası İletişim
Alt-ajan (sub-agent) kullanım senaryoları tanımlı değil:
- Büyük görevlerde görev parçalama stratejisi yok
- Paralel çalışma kuralları yok

#### ❌ Eksik 5: Hata Kurtarma Protokolü
Build fail, test fail gibi durumlarda ne yapılacağı tanımlı değil:
- Geri alma (rollback) stratejisi yok
- Hata analiz sırası belirsiz

#### ❌ Eksik 6: Prompt Engineering Yetersizlikleri
- `trigger: always_on` her zaman yükleniyor, ama 13KB boyutundaki dosya her sorguda context window'u dolduruyor
- Şapka detayları çok uzun — özet referans kartları yok
- Durumsal (conditional) kurallar eksik

---

## 2. ÖNERİLEN AJAN YAPISI: 6 KİŞİLİK SANAL TAKIM

### 2.1 Mimari Felsefe

Mevcut "tek ajan, 6 şapka" yapısını **"1 orkestra şefi + 5 uzman ajan"** modeline dönüştürmenizi öneriyorum.

```
┌──────────────────────────────────────────────────────────┐
│                    ORKESTRA ŞEFİ                         │
│              Agent_Gravity (Coordinator)                  │
│         Görevi: Yönlendirme, karar alma, raporlama       │
│                                                          │
│    ┌──────────┬──────────┬──────────┬──────────┬──────┐  │
│    │          │          │          │          │      │  │
│    ▼          ▼          ▼          ▼          ▼      ▼  │
│  🏗️ MİMAR   💼 ANALİST  🗄️ DBA    🔒 GÜVENLİK 🧪 TEST │
│  Architect  Business   Database   Security  Quality │  │
│  Agent      Agent      Agent      Agent     Agent   │  │
│                                                      │  │
└──────────────────────────────────────────────────────────┘
```

Bu yapıda Antigravity her zaman **orkestra şefi** olarak kalır. Uzman ajanlar, Antigravity'nin "kontrollü" alt-personaları olarak çalışır. Ayrı ayrı çalışan bağımsız AI ajanları değil, **aynı ajanın bağlama göre aktive olan uzmanlık modülleri** olarak düşünülmelidir.

### 2.2 Neden Ayrı Dosyalar?

| Sebep | Açıklama |
|:---|:---|
| **Context window optimizasyonu** | 13KB'lik tek dosya yerine, ihtiyaç anında yüklenen 2-3KB'lik modüller |
| **Bakım kolaylığı** | Her uzmanlık alanı bağımsız güncellenebilir |
| **Tetikleyici granülasyon** | Her modülün ne zaman yükleneceği ayrı tanımlanabilir |
| **Prompt kalitesi** | Her uzman için özelleştirilmiş talimat seti |

---

## 3. HER AJANIN DETAYLI TANIMI

### 3.1 🎯 Agent_Gravity (Orkestra Şefi — Coordinator)

**Dosya:** `.agents/rules/Agent_Gravity.md`
**Tetikleyici:** `trigger: always_on` (her zaman yüklü)
**Boyut hedefi:** Maksimum 4-5KB (şu an 13KB — sadeleştirilmeli)

**Sorumlulukları:**
- Gökmen'den gelen talebi analiz et ve doğru uzmana yönlendir
- Görev önceliklendirme (🔴 / 🟡 / 🔵 / ⚪)
- Hafıza dosyalarını güncelleme kararı
- Raporlama ve sonuç sunumu
- Çelişen uzman görüşlerinde son kararı verme

**Önemli Değişiklik:** Mevcut dosyadaki Şapka 1-6 detayları bu dosyadan **çıkarılacak** ve her biri ayrı skill dosyasına taşınacak.

---

### 3.2 🏗️ Yazılım Mimarı Ajanı (Architect Agent)

**Dosya:** `.agents/skills/architect/SKILL.md`
**Tetikleyici:** Yeni servis/endpoint ekleme, mimari karar, refactoring, dependency değişikliği

**Sorumlulukları:**
- Clean Architecture katman kurallarına uyumu denetleme
- CQRS + MediatR yapısını koruma
- Bounded Context sınırlarını gözetme
- Naming convention uyumu
- Dependency direction denetimi (içe doğru bağımlılık)
- Yeni servis/modül tasarımı

**Kontrol Listesi:**
- ☐ Domain katmanında hiçbir NuGet referansı yok mu?
- ☐ Application'dan Infrastructure'a referans yok mu?
- ☐ Controller'dan doğrudan Repository çağrısı yok mu?
- ☐ Başka bir servisin Domain'ine referans yok mu?
- ☐ Store.Shared veya Store.DtoLayer üzerinden mi paylaşılıyor?

**Başvuru Kaynakları:** `.agents/docs/ARCHITECTURE.md`, `.agents/docs/PROJECT_CONTEXT.md`

---

### 3.3 💼 İş Analisti Ajanı (Business Agent)

**Dosya:** `.agents/skills/business_analyst/SKILL.md`
**Tetikleyici:** İş akışı kodlama, validasyon yazma, fiyatlandırma, sipariş/kiralama mantığı

**Sorumlulukları:**
- B2C/B2B iş akışlarını gözetme
- Fiyatlandırma mantıklarını koruma (5 PriceType)
- KVKK uyumluluğunu denetleme
- İş kuralı tekrarını (duplicate business rule) tespit etme
- Yeni iş kurallarını `BUSINESS_RULES.md`'ye kaydetme

**Kontrol Listesi:**
- ☐ Aynı iş kuralı birden fazla yerde mi kodlanıyor?
- ☐ Fiyat validasyonu: DiscountedAmount ≤ Amount mi?
- ☐ Tarih çakışma kontrolü var mı?
- ☐ Soft-delete kuralı uygulanıyor mu?
- ☐ KVKK hassas veriler (IdentityNumber, Phone) şifreli mi?

**Başvuru Kaynağı:** `.agents/docs/BUSINESS_RULES.md`

---

### 3.4 🗄️ Veritabanı Yöneticisi Ajanı (DBA Agent)

**Dosya:** `.agents/skills/dba/SKILL.md`
**Tetikleyici:** Entity ekleme, migration yazma, sorgu tasarımı, index ekleme, schema değişikliği

**Sorumlulukları:**
- MongoDB, PostgreSQL ve Redis en iyi pratiklerini uygulama
- İndeksleme ve sorgu optimizasyonu
- Migration standartlarına uyumu denetleme
- Ortam senkronizasyonu (Local:5432 & Docker:5434) kuralını uygulama
- Veritabanı değişikliğinden önce mevcut schema'yı okuma

**Sorgu Optimizasyon Kuralları:**
- Okuma sorgularında AsNoTracking() kullanılmalı
- Pagination: Skip/Take ile sayfalama zorunlu (büyük tablolar)
- N+1 sorgu problemi tespit edilmeli (foreach içinde DB çağrısı yasak)
- Sadece gerekli kolonlar çekilmeli (Select ile projeksiyon)

**Kontrol Listesi:**
- ☐ Migration hem Local (5432) hem Docker (5434) ortamına uygulandı mı?
- ☐ Yeni entity BaseEntity'den türüyor mu?
- ☐ BaseEntity property'leri tekrar tanımlanmadı mı? (shadowing yasak)
- ☐ Unique index gerekli alanlara konuldu mu?
- ☐ Global Query Filter (IsDeleted) tanımlı mı?
- ☐ DATABASE_MAP.md güncellendi mi?

**Başvuru Kaynağı:** `.agents/docs/DATABASE_MAP.md`

---

### 3.5 🔒 Güvenlik Mühendisi Ajanı (Security Agent)

**Dosya:** `.agents/skills/security/SKILL.md`
**Tetikleyici:** Controller oluşturma, kimlik doğrulama, yetkilendirme, veri işleme, appsettings değişikliği

**Sorumlulukları:**
- JWT / OAuth 2.0 doğrulamasını denetleme
- RBAC rol hiyerarşisini koruma
- Sır yönetimi kurallarını zorlama
- KVKK uyumluluğunu denetleme
- IDOR, XSS, CSRF, SQL Injection kontrolü
- Endpoint güvenlik denetimi ([Authorize] kontrolü)

**KVKK Veri Sınıflandırması:**

| Veri | Hassasiyet | Kural |
|:---|:---|:---|
| IdentityNumber | 🔴 Çok Yüksek | Şifreli saklanmalı |
| PhoneNumber | 🟡 Yüksek | Loglanmamalı |
| Email | 🟡 Yüksek | Loglanmamalı |
| AddressDetail | 🔵 Orta | Anonimleştirilebilir |

**Kontrol Listesi:**
- ☐ Tüm API endpoint'lerinde [Authorize] var mı?
- ☐ [AllowAnonymous] bilinçli ve gerekçeli mi?
- ☐ UserId parametresi dışarıdan mı alınıyor? (IDOR riski!) → JWT Claims'ten alınmalı
- ☐ appsettings.json'da plaintext şifre/connection string yok mu?
- ☐ SSL bypass sadece #if DEBUG içinde mi?
- ☐ Hassas veriler (IdentityNumber, Phone) loglanmıyor mu?
- ☐ Input validation (FluentValidation) tanımlı mı?

---

### 3.6 🧪 Test Mühendisi Ajanı (Quality Agent)

**Dosya:** `.agents/skills/test_engineer/SKILL.md`
**Tetikleyici:** Yeni handler/servis yazıldığında, code review sırasında, refactoring sonrasında

**Sorumlulukları:**
- Test edilebilirlik değerlendirmesi
- Test stratejisi önerisi (unit / integration / e2e)
- Test şablonu sunma
- Code review sırasında test kapsamı denetimi
- Mock/Stub stratejisi

**Öncelikli Test Hedefleri:**
1. 🔴 Fiyatlandırma hesaplama mantığı
2. 🔴 Sipariş oluşturma iş kuralları
3. 🟡 CQRS Handler'lar (Command/Query)
4. 🟡 FluentValidation Validator'lar
5. 🔵 Repository CRUD operasyonları

**Kontrol Listesi:**
- ☐ Dependency Injection kullanılıyor mu?
- ☐ Bağımlılıklar interface üzerinden mi alınıyor?
- ☐ İş mantığı Controller'dan izole mi?
- ☐ Statik metod kullanımı minimize mi?
- ☐ DateTime.Now yerine soyutlama kullanılıyor mu?

---

## 4. ÖNERİLEN .AGENTS KLASÖR YAPISI

```
.agents/
├── rules/
│   └── Agent_Gravity.md          ← Orkestra Şefi (sadeleştirilmiş, ~4KB)
│
├── skills/
│   ├── architect/
│   │   └── SKILL.md              ← Mimari kurallar ve kontrol listeleri
│   ├── business_analyst/
│   │   └── SKILL.md              ← İş kuralları ve validasyon şablonları
│   ├── dba/
│   │   └── SKILL.md              ← DB tasarımı, migration, sorgu optimizasyonu
│   ├── security/
│   │   └── SKILL.md              ← Güvenlik denetimi, KVKK, sır yönetimi
│   └── test_engineer/
│       └── SKILL.md              ← Test stratejisi ve şablonları
│
├── docs/
│   ├── ARCHITECTURE.md           ← Teknik mimari haritası (mevcut)
│   ├── BUSINESS_RULES.md         ← İş kuralları (mevcut)
│   ├── DATABASE_MAP.md           ← Veritabanı topolojisi (mevcut)
│   ├── PROJECT_CONTEXT.md        ← Proje bağlamı (mevcut)
│   ├── PROJECT_STATUS.md         ← Proje durumu (mevcut)
│   └── AjanYapisiVeYapilacaklar.md ← Bu doküman
│
└── plugins/                      ← (Gelecekte — CI/CD, Monitoring vb.)
```

---

## 5. AJAN AKTİVASYON MATRİSİ

| Görev Tipi | Lider Ajan | Destek Ajanlar |
|:---|:---|:---|
| Yeni servis/modül ekleme | 🏗️ Architect | 🗄️ DBA, 🔒 Security |
| Yeni endpoint/controller | 🏗️ Architect | 🔒 Security |
| Entity / migration ekleme | 🗄️ DBA | 🏗️ Architect |
| İş akışı kodlama | 💼 Business Analyst | 🏗️ Architect, 🧪 Test |
| Validasyon yazma | 💼 Business Analyst | 🔒 Security |
| Code review / refactoring | 🏗️ Architect | 🧪 Test, 🗄️ DBA |
| Güvenlik denetimi | 🔒 Security | — |
| Test yazma | 🧪 Test Engineer | — |
| Performans iyileştirme | 🗄️ DBA | 🏗️ Architect |
| appsettings / config değişikliği | 🔒 Security | — |
| Hata düzeltme (bugfix) | 🏗️ Architect | İlgili uzman |
| Dokümantasyon güncelleme | Coordinator (Gravity) | — |

---

## 6. YAPILACAKLAR LİSTESİ

### 6.1 Ajan Sistemi İyileştirmeleri (1-2 Gün)

- [x] `Agent_Gravity.md`'yi sadeleştir (13KB → ~4KB)
  - Şapka detaylarını skill dosyalarına taşı
  - Aktivasyon matrisini ekle
  - Yasak listesini koru (kısa ve net)
- [x] `.agents/skills/` klasörünü oluştur
- [x] `architect/SKILL.md` oluştur
- [x] `business_analyst/SKILL.md` oluştur
- [x] `dba/SKILL.md` oluştur
- [x] `security/SKILL.md` oluştur
- [x] `test_engineer/SKILL.md` oluştur
- [x] Her skill dosyasını YAML frontmatter ile yapılandır

### 6.2 Hafıza Dosyası Güncellemeleri

- [x] `ARCHITECTURE.md` — Favorite servis portu (5227) ve durumunu güncelle (TBD → ✅ Aktif)
- [x] `PROJECT_STATUS.md` — Çözülmüş teknik borçları düzelt (SSL Bypass ve tempkey.jwk çözüldüyse kontrol et)
- [x] `DATABASE_MAP.md` — Order ID tipi tutarsızlığını düzelt (int → Guid olduğu belirtiliyor ama tablo int gösteriyor)
- [x] `BUSINESS_RULES.md` — Kiralama kurallarını detaylandır (Faz 2 öncesinde iş kuralları netleştirilmeli)

### 6.3 Eksik Hafıza Dosyaları (Eklenmeli)

- [x] **`TECH_STACK.md`** — Kullanılan NuGet paketlerinin versiyonları, konfigürasyon detayları
- [x] **`API_CONTRACTS.md`** — Her servisin endpoint listesi, istek/yanıt formatları
- [x] **`CHANGELOG.md`** — Yapılan değişikliklerin tarihsel kaydı (ADR'nin yanında sprint bazlı log)

### 6.4 Yeni Keşfedilen İş ve Sistem Kuralları Entegrasyonu (Eklenecekler)

- [ ] **Ödeme Sistemi:** iyzico/PayTR entegrasyonu için `BUSINESS_RULES.md` ve `ARCHITECTURE.md` güncellenmeli.
- [ ] **Stok ve Lojistik Yönetimi:** Kargo/lojistik entegrasyonu ve stok düşme/artırma iş akışı kuralları tanımlanmalı.
- [ ] **Bildirim Stratejisi:** E-posta, SMS ve push bildirim stratejileri için kural ve altyapı tasarlanmalı.
- [ ] **SEO ve Performans Stratejisi:** Meta tag yönetimi, sitemap, JSON-LD yapıları ile hedef response time/throughput gibi KPI'lar belirlenmeli.
- [ ] **Hata Yönetimi ve API Güvenliği:** ErrorResponse formatları, hata kodları standartlaştırılmalı, versiyonlama (v1, v2) ve Rate Limiting planlanmalı.
- [ ] **Backup / Disaster Recovery:** Veritabanı yedekleme ve felaket kurtarma senaryoları dokümante edilmeli.

---

## 7. AJAN EĞİTİM PRENSİPLERİ

### 7.1 Etkili Prompt Engineering Kuralları

1. **Kısa ve net ol** — Her skill dosyası max 3KB olmalı. Uzun açıklamalar yerine kontrol listeleri kullan.
2. **Somut örnekler ver** — "Clean Architecture uygula" yerine `Domain → Application → Persistence → WebApi` zincirini göster.
3. **Negatif örnekler ekle** — "Yapma" listesi, "Yap" listesi kadar önemli.
4. **Tetikleyici tanımla** — Her skill'in YAML frontmatter'ında `description` alanı, AI'ın doğru zamanda doğru skill'i seçmesi için kritik.
5. **Kontrol listeleri kullan** — Uzun paragraflar yerine ☐ checkbox formatı, hem uygulamayı hem denetimi kolaylaştırır.
6. **Referans ver** — Her skill, ilgili hafıza dosyasına (docs/) referans vermelidir.

### 7.2 Skill Dosyalarında Kaçınılması Gerekenler

- ❌ Aynı kuralı birden fazla dosyaya yazmak (DRY prensibi)
- ❌ Çok genel/soyut ifadeler ("kaliteli kod yaz" gibi)
- ❌ Çok uzun dosyalar (context window tüketimi)
- ❌ Çelişen kurallar (dosyalar arası tutarlılık şart)
- ❌ Tarihsel detaylar (bunlar docs/ altındaki dosyalarda olmalı)

### 7.3 Sürekli İyileştirme Döngüsü

```
  1. KULLAN    → Ajan sistemini günlük işte kullan
  2. GÖZLEMLE  → Hangi durumlarda eksik kaldı? Hangi kural atlandı?
  3. GÜNCELLE  → İlgili skill/rule dosyasını güncelle
  4. TEST ET   → Güncellemenin etkisini izle
```

### 7.4 İleri Düzey Ajan Geliştirme (Next-Gen AI Eğitim Planı)

- **Alt-Ajan (Sub-agent) Orkestrasyonu:** Çok katmanlı görevlerde Antigravity'nin alt ajanları paralel veya sıralı çalıştırma (chain-of-thought ve map-reduce gibi) pratiklerini öğrenmesi sağlanmalıdır.
- **Kendi Kendini Düzeltme (Self-Correction):** Ajanın derleme veya test hatalarından otonom şekilde ders çıkararak kod onarımı yapabilme becerisi geliştirilmelidir.
- **Bağlam Yönetimi:** Mevcut hafıza dokümanlarını (KIs, projeye özel MD dosyaları) proaktif bir şekilde arayarak, sormadan okuma alışkanlığının oturtulması hedeflenmelidir.
- **Güvenlik Simülasyonları:** Güvenlik şapkasının yeteneklerini artırmak amacıyla bilinen OWASP TOP 10 zafiyetlerini (IDOR, XSS, vs.) repoda statik analiz yaparak bulma pratikleri geliştirilmelidir.

---

## 8. ATLANMIŞ / EKSİK NOKTALAR — UYARILAR

### 8.1 Atlanmış Konular

| # | Konu | Neden Önemli |
|:--|:---|:---|
| 1 | **Ödeme sistemi iş kuralları** | BUSINESS_RULES.md'de ödeme ile ilgili hiç kural yok. iyzico/PayTR entegrasyonu planlanıyor mu? |
| 2 | **Stok yönetimi kuralları** | Ürün varyantlarında StockQuantity var ama stok düşme/artırma iş akışı tanımlı değil |
| 3 | **Kargo/lojistik entegrasyonu** | Sipariş sonrası süreç tamamen tanımsız |
| 4 | **Bildirim stratejisi** | E-posta, SMS, push notification planı yok |
| 5 | **SEO stratejisi** | Slug var ama sitemap, meta tag yönetimi, structured data (JSON-LD) eksik |
| 6 | **Performans metrikleri** | Hedef response time, throughput gibi KPI'lar tanımlı değil |
| 7 | **Hata yönetimi standartları** | Global exception middleware var deniyor ama detayları (ErrorResponse formatı, hata kodları) tanımsız |
| 8 | **API versiyonlama stratejisi** | Versiyon yönetimi (v1, v2) planlanmamış |
| 9 | **Rate limiting / throttling** | API güvenliği için istek sınırlandırma yok |
| 10 | **Backup / Disaster Recovery** | Veritabanı yedekleme stratejisi tanımsız |

### 8.2 Tutarsızlıklar

| # | Dosya 1 | Dosya 2 | Tutarsızlık |
|:--|:---|:---|:---|
| 1 | `DATABASE_MAP.md` | `BUSINESS_RULES.md` | Order ID tipi: DATABASE_MAP "int (Identity)" + not olarak Guid deniyor, BUSINESS_RULES "int" diyor. Hangisi geçerli? |
| 2 | `PROJECT_STATUS.md` | Gerçek durum | SSL Bypass "✅ ÇÖZÜLDÜ" ama Faz 1'de "❌ SSL bypass kodlarının #if DEBUG ile sınırlandırılması" hâlâ açık |
| 3 | `ARCHITECTURE.md` | `PROJECT_STATUS.md` | Favorite servisi: ARCHITECTURE'da port "TBD", STATUS'ta "✅ Aktif" — port güncellemesi yapılmalı |

---

## 9. ÖZET VE SONRAKİ ADIMLAR

### Hemen Yapılabilecekler
1. Bu dokümandaki ajan yapısını değerlendir ve onay ver
2. `Agent_Gravity.md`'yi sadeleştirme işlemini başlat
3. Skill dosyalarını oluştur
4. Hafıza dosyalarındaki tutarsızlıkları düzelt

### Karara Bağlanan ve Not Alınan Konular (3 Ekim 2026)
- **Stok & Lojistik:** Karmaşık MRP yerine tamamen **e-ticaret odaklı giriş/çıkış (stok takibi)** şeklinde kurgulanmasına karar verildi. İş kurallarına işlendi.
- **Ödeme Entegrasyonu:** Şimdilik ertelendi/not alındı. Şu an için işlem yapılmayacak.
- **Kimlik Yönetimi Geçişi:** Şimdilik ertelendi/not alındı. Mevcut yapıda devam ediliyor.
- **Mobil Uygulama & API:** Şimdilik ertelendi/not alındı. Şu an için işlem yapılmayacak.

---

> **Bu doküman, Gökmen'in isteği üzerine Store.sln ajan yapısının kapsamlı değerlendirmesi olarak hazırlanmıştır.**
> **Tarih:** 2 Ekim 2026
> **Hazırlayan:** Antigravity
