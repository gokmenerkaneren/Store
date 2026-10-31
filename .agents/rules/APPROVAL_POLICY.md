# Approval Policy & Risk Matrix (Onay ve Risk Politikası)

## Amaç
Agent Gravity'nin hangi görevleri kendi inisiyatifiyle (Autonomous) yürütebileceğini, hangi görevlerde Kullanıcı Onayına (Approval) başvurması gerektiğini Risk (L0-L3) seviyelerine göre kesin ve deterministik olarak belirlemektir.

## Risk → Gate → Approval Matrisi

| Risk Seviyesi | Kategori / Örnek Senaryo | Otonom İşlem | Kullanıcı Onayı (Gate) |
| :--- | :--- | :---: | :---: |
| **L0** | Dokümantasyon düzeltmeleri (Typo), Klasör yapısı vb. | ✅ | ❌ |
| **L1** | Güvenli Refactoring, Unit Test ekleme, Basit bugfix | ✅ | ❌ |
| **L2** | API Contract (DTO) değişikliği, Endpoint ekleme, DB Index ekleme | ⚠️ (Şartlı) | ⚠️ (İnceleme Gerekir) |
| **L3** | Kimlik/Auth mimarisi değişikliği, Ödeme sistemi kararları | ❌ | ✅ (ZORUNLU) |
| **L3** | Destructive DB Migration (Sütun silme/veri kaybı riski) | ❌ | ✅ (ZORUNLU) |
| **L3** | Yeni teknoloji (Örn: RabbitMQ) ekleme / Bağımlılık Kararı | ❌ | ✅ (ZORUNLU) |

## Uygulama Kuralı (Agent_Gravity İçin)
1. Eğer görev L0 veya L1 kategorisindeyse, ajanlar arasında `HANDOFF` yapılır ve `EVIDENCE` toplandıktan sonra görev kapatılır. Kullanıcı uyarılmaz.
2. L2 riskinde bir görevse, Agent_Gravity değişikliği uygulatır ancak PR (Pull Request) veya final inceleme aşamasında `Gate State: APPROVAL_REQUIRED` oluşturur.
3. L3 riskinde bir görevse, kod/şema değişikliği YAPILMADAN ÖNCE `Gate State: BLOCKED` (USER_APPROVAL_GATE) açılır. Onay gelmeden ajanlar (Architect, DBA vb.) `IMPLEMENTING` aşamasına geçemez.
