# WEEKLY ENGINEERING GOVERNANCE

## Döngü

`OBSERVE → AUDIT → CLASSIFY → PLAN → REMEDIATE → VERIFY → DOCUMENT → GOVERNANCE UPDATE`

## Review Başlıkları

Her hafta projenin ve ajanların sağlığı şu başlıklar altında incelenir:
1. Architecture (Mimari Sağlık)
2. Business Rules (İş Kuralı Tutarlılığı)
3. Database (Şema ve Veri bütünlüğü)
4. Security (Zafiyet taraması)
5. Tests (Kapsam ve Kırılganlık)
6. Technical Debt (Teknik Borç)
7. Documentation Drift (Doküman - Kod uyumsuzluğu)
8. **Agent Performance (Ajan Performansı - YENİ):** `governance/AGENT_METRICS.md` kapsamında yanlış aktivasyon, gereksiz onay (decision gate) sıklığı, hata tekrarları ve ajan iletişim sağlığının ölçümü.

## Agent Sistemi (Governance) Güncellemesi

Bir haftalık review yeni bir governance kuralı veya `SKILL.md` güncellemesi öneriyorsa doğrudan değiştirilmez. 
Kural değişikliği için şu süreç izlenir:
`Proposal` → `Impact Analysis` → `Review` → `User Approval` → `Versioning` (Kuralların versiyonlanması).
