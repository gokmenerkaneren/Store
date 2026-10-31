# Gate & Decision Schema (Kapı ve Karar Veri Sözleşmesi)

## Format
Kullanıcı onayına sunulan karar veya blokajlar bu şema ile kimliklendirilir. Mevcut `DECISION_SCHEMA.md`'nin ID atanmış yapısal versiyonudur.

```markdown
**Gate ID:** GATE-YYYYMMDD-XXXX
**Task ID:** [Bağlı olduğu Task ID]
**Type:** [USER_APPROVAL / CONFLICT_RESOLUTION / ESCALATION]
**Status:** [PENDING_USER / APPROVED / REJECTED / RESOLVED]
**Conflict/Issue:** 
[Problemin teknik özeti]
**Expert Deliberation:**
- [AGT-XXX]: [Görüşü]
- [AGT-YYY]: [Görüşü]
**Risks & Impact:**
[Risklerin analizi]
**Gravity's Recommendation:**
[Öneri (veya NO RECOMMENDATION - INSUFFICIENT EVIDENCE)]
**Options:**
1. [Option 1]
2. [Option 2]
```

## Kullanım Kuralı
Kullanıcı (Gökmen) ile iletişime geçilen her Decision Gate bu ID ile loglanmalıdır ki sonradan "GATE-20261003-001 numaralı kararda ne demiştik?" şeklinde geriye dönük izlenebilsin.
