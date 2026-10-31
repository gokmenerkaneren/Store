# Evidence Schema (Kanıt Veri Sözleşmesi)

## Format
`EVIDENCE_PROTOCOL` gereği üretilen her kanıt bu sözleşmeye uymalıdır.

```markdown
**Evidence ID:** EVD-YYYYMMDD-XXXX (Örn: EVD-20261003-0017)
**Task ID:** [Bağlı olduğu Task ID]
**Agent ID:** [Kanıtı sunan ajan: AGT-XXX]
**Evidence Type:** [BUILD / TEST / FILE_STATE / DATABASE / LOG]
**Command/Action:** [Örn: `dotnet build Store.slnx` veya `File Check`]
**Expected Result:** [Örn: 0 errors veya Dosyada şifreleme olması]
**Actual Result:** [Örn: 0 errors]
**Status:** [PASS / FAIL]
**Timestamp:** [ISO 8601]
**Artifact/Log Snippet:** 
[Kanıtın çıktısı veya referansı]
```

## Kullanım Kuralı
Bir ajan görevini `COMPLETED` durumuna çekmeden önce, o görev veya handoff için durumu `PASS` olan en az bir Evidence ID sunmak zorundadır.
