# Task Schema (Görev Veri Sözleşmesi)

## Format
Görevler sisteme girdiğinde bu kimlik ve veri sözleşmesi ile takip edilir.

```markdown
**Task ID:** TASK-YYYYMMDD-XXXX (Örn: TASK-20261003-0042)
**Title:** [Görevin kısa adı]
**Description:** [Görevin tam açıklaması veya kullanıcı promptu]
**State:** [Task State: OPEN / IN_PROGRESS / BLOCKED / VERIFYING / COMPLETED]
**Risk Level:** [L0 / L1 / L2 / L3]
**Impact Area:** [Etkilenen Katmanlar/Servisler]
**Assigned Agents:** [Örn: AGT-ARCH, AGT-SEC]
**Related Subtasks:** [Alt görevlerin ID listesi]
```

## Kullanım Kuralı
Agent_Gravity her yeni Kullanıcı isteğini (User Request) bu şemaya oturtarak bir `TASK ID` üretir ve tüm süreç bu ID üzerinden birbirine bağlanır.
