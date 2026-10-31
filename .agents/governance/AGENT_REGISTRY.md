# Agent Registry (Ajan Sicil ve Kayıt Sistemi)

## Amaç
Agent Governance sistemindeki tüm ajanların resmi tanımlarının, benzersiz kimliklerinin (Canonical IDs) ve versiyonlarının tek merkezden izlenmesidir.

## Ajan Kayıtları (Registry)

| Agent ID | Name | Role / Focus | Status | Version | Skill Path |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **AGT-GRAVITY** | Agent Gravity | Orchestrator, PM | ACTIVE | 1.2.0 | `rules/Agent_Gravity.md` |
| **AGT-ARCH** | Architect | System Design, CQRS, DI | ACTIVE | 1.1.0 | `skills/architect/SKILL.md` |
| **AGT-BA** | Business Analyst| Business Logic, Validation | ACTIVE | 1.1.0 | `skills/business_analyst/SKILL.md` |
| **AGT-DBA** | DBA | Persistence, Schema, Migration| ACTIVE | 1.1.0 | `skills/dba/SKILL.md` |
| **AGT-SEC** | Security | Auth, Encryption, KVKK | ACTIVE | 1.1.0 | `skills/security/SKILL.md` |
| **AGT-QA** | Test Engineer | Quality, Testing, Evidence | ACTIVE | 1.1.0 | `skills/test_engineer/SKILL.md` |

## Kurallar
1. Tüm Handoff (Devir) protokollerinde ve kanıtlarda (Evidence) "Architect" veya "Test" kelimeleri yerine **Agent ID (Örn: AGT-ARCH)** kullanılmalıdır.
2. Sisteme yeni bir uzman ekleneceği zaman önce bu sicil dosyasına kaydedilmeli ve kimlik numarası üretilmelidir.
