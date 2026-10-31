# Agent State Schema (Ajan Durum Veri Sözleşmesi)

## Format
Sistemin herhangi bir anında ajanların durumunu (snapshot) almak için kullanılan yapısal formattır.

```markdown
**Snapshot ID:** AST-YYYYMMDD-XXXX
**Task ID:** [İzlenen Task ID]
**Timestamp:** [ISO 8601]

**Agent States:**
- AGT-GRAVITY : [ACTIVE / WAITING_USER]
- AGT-ARCH    : [COMPLETED]
- AGT-DBA     : [COMPLETED]
- AGT-SEC     : [VERIFYING]
- AGT-QA      : [WAITING_FOR_HANDOFF]
- AGT-BA      : [IDLE]

**Task State:** [VERIFYING]
**Gate State:** [PENDING_USER (Eğer bir Gate açıksa)]
```

## Kullanım Kuralı
Agent_Gravity bu sözleşmeyi kullanarak sistemin anlık fotoğrafını çeker ve ajanların nerede beklediğini (bottleneck) tespit eder. Markdown bir doküman koleksiyonundan çok, bir "State Object" gibi işlev görür.
