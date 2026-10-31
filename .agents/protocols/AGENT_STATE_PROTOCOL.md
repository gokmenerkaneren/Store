# Agent State Protocol (Durum ve Geçiş Mimarisi)

## Amaç
Sistemin kilitlenmeden, "State Machine" mantığı ile deterministik bir şekilde çalışmasıdır. Görevlerin, Ajanların ve Karar Kapılarının (Gate) birbirinden bağımsız kendi durumları olmalıdır.

## 1. Görev Durumları (Task State)
Görevlerin (Task) genel ilerleyişidir:
- `REQUESTED` → `CLASSIFYING` → `IMPLEMENTING` → `VERIFYING` → `COMPLETED` / `FAILED`

## 2. Kapı Durumları (Gate State)
Kullanıcı müdahalesi gerektiren onaysal durumlardır:
- `NO_GATE_ACTIVE` → `PENDING_USER_APPROVAL` → `APPROVED` / `REJECTED`

## 3. Ajan Durumları (Agent State)
Ajanlar yalnızca aşağıdaki durumlarda (state) bulunabilir:
- `IDLE`, `ACTIVATING`, `ANALYZING`, `IMPLEMENTING`, `VERIFYING`, `WAITING_FOR_HANDOFF`, `WAITING_FOR_DECISION`, `BLOCKED`, `FAILED`, `COMPLETED`

## 4. State Transition (Durum Geçiş Matrisi)
Bir durumdan diğerine hangi koşulda geçilebileceği aşağıda tanımlanmıştır:

| Mevcut Durum (Current) | Olay (Event) | Sonraki Durum (Next) | Tetikleyen (Actor) |
| :--- | :--- | :--- | :--- |
| `IDLE` | TASK_ASSIGNED | `ACTIVATING` | Gravity |
| `ACTIVATING` | CONTEXT_READY | `ANALYZING` | Agent |
| `ANALYZING` | PLAN_READY | `IMPLEMENTING` | Agent |
| `IMPLEMENTING` | CODE_READY | `VERIFYING` | Agent |
| `VERIFYING` | EVIDENCE_PASS | `COMPLETED` | Gravity / QA |
| `VERIFYING` | EVIDENCE_FAIL | `FAILED` | Gravity / QA |
| `FAILED` | RETRY_ALLOWED | `ANALYZING` | Gravity |
| `FAILED` | RETRY_LIMIT_REACHED| `BLOCKED` | Gravity |
| `BLOCKED` | USER_DECISION_GIVEN | `ANALYZING` | Gravity (Kullanıcı sonrası) |
| `WAITING_FOR_DECISION`| USER_RESPONSE_GIVEN | `ANALYZING` | Gravity (Kullanıcı sonrası) |

## Uygulama Kuralı
Görevler (`TASK`) ve Ajanlar (`AGENT`) birbirinden bağımsız durum tutar. Örnek: `Task State = IMPLEMENTING` iken, `Architect = COMPLETED`, `DBA = COMPLETED`, `Security = IMPLEMENTING` olabilir. Yalnızca Ajan, matristeki olaylar tetiklendiğinde durum değiştirebilir.
